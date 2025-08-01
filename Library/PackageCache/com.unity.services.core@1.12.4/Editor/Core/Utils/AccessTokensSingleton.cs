using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Core.Internal.Serialization;
using Unity.Services.Core.Scheduler.Internal;
using UnityEditor;

namespace Unity.Services.Core.Editor
{
    class AccessTokensSingleton : IAccessTokens
    {
        static IAccessTokens s_Instance;
        static object m_LockObject = new object();

        IGatewayTokens m_GatewayTokens;
        SemaphoreSlim m_Semaphore;
        internal Task<string> GatewayTokenTask;

        internal AccessTokensSingleton(IGatewayTokens gatewayTokens)
        {
            m_GatewayTokens = gatewayTokens;
            m_Semaphore = new SemaphoreSlim(1, 1);
        }

        public static IAccessTokens Instance
        {
            get
            {
                lock (m_LockObject)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new AccessTokensSingleton(GetGatewayTokens());
                    }
                }

                return s_Instance;
            }
        }

        public string GetGenesisToken() => CloudProjectSettings.accessToken;

        public async Task<string> GetServicesGatewayTokenAsync()
        {
            if (GatewayTokenTask != null)
            {
                return await GatewayTokenTask;
            }

            string token = null;
            try
            {
                await m_Semaphore.WaitAsync();
                GatewayTokenTask = m_GatewayTokens.GetGatewayTokenAsync(GetGenesisToken());
                token = await GatewayTokenTask;
            }
            finally
            {
                GatewayTokenTask = null;
                m_Semaphore.Release();
            }

            return token;
        }

        static IGatewayTokens GetGatewayTokens()
        {
            var env = new CloudEnvironmentConfigProvider();
            ITokenExchangeUrls urls;
            if (env.IsStaging())
            {
                urls = new StagingTokenExchangeUrls();
            }
            else
            {
                urls = new ProductionTokenExchangeUrls();
            }

            var serializer = new NewtonsoftSerializer();
            return new GatewayTokens(
                new TokenExchange(urls, serializer), new UtcTimeProvider(), serializer);
        }
    }
}
