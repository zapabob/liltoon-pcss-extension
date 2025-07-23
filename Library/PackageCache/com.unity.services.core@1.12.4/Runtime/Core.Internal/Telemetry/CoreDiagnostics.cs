using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal
{
    class CoreDiagnostics
    {
        internal const string CorePackageName = "com.unity.services.core";

        internal const string CircularDependencyDiagnosticName = "circular_dependency";

        internal const string CorePackageInitDiagnosticName = "core_package_init";

        internal const string OperateServicesInitDiagnosticName = "operate_services_init";

        internal const string ProjectConfigTagName = "project_config";

        public static CoreDiagnostics Instance { get; internal set; }

        public IDictionary<string, string> CoreTags { get; }
            = new Dictionary<string, string>();

        internal IDiagnosticsComponentProvider DiagnosticsComponentProvider { get; set; }

        internal IDiagnostics Diagnostics { get; set; }

        public void SetProjectConfiguration(string serializedProjectConfig)
        {
            // do nothing
        }

        public void SendCircularDependencyDiagnostics(Exception exception)
        {
            // do nothing
        }

        public void SendCorePackageInitDiagnostics(Exception exception)
        {
            // do nothing
        }

        public void SendOperateServicesInitDiagnostics(Exception exception)
        {
            // do nothing
        }

        internal async Task SendCoreDiagnosticsAsync(string diagnosticName, Exception exception)
        {
            // do nothing
            await Task.CompletedTask;
        }

        static void OnSendFailed(Task failedSendTask)
        {
            // do nothing
        }

        internal async Task<IDiagnostics> GetOrCreateDiagnosticsAsync()
        {
            if (Diagnostics is null)
            {
                var factory = await DiagnosticsComponentProvider.CreateDiagnosticsComponents();
                Diagnostics = factory.Create(CorePackageName);
            }

            return Diagnostics;
        }
    }
}
