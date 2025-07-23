using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Services.Core.Configuration.Editor;
using Unity.Services.Core.Internal;
using UnityEditor;
using UnityEditor.Build;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Services.Core.Editor
{
    class OperatePackageVersionConfigProvider : IConfigurationProvider
    {
        internal const string PackageVersionKeyFormat = "{0}.version";
        internal const string PackageInitializerNamesKeyFormat = "{0}.initializer-assembly-qualified-names";
        internal const char PackageInitializerNamesSeparator = ';';
        internal const string AllPackageNamesKey = "com.unity.services.core.all-package-names";
        internal const char AllPackageNamesSeparator = ';';

        static readonly OperatePackageVersionConfigProvider k_EditorInstance = new OperatePackageVersionConfigProvider
        {
            m_OperatePackages = CreateOperatePackagesConfigsForProject()
        };

        IEnumerable<PackageConfig> m_OperatePackages;

        int IOrderedCallback.callbackOrder { get; }

        void IConfigurationProvider.OnBuildingConfiguration(ConfigurationBuilder builder)
        {
            var operatePackages = BuildPipeline.isBuildingPlayer
                ? CreateOperatePackagesConfigsForProject()
                : k_EditorInstance.m_OperatePackages;

            ProvidePackageConfigs(builder, operatePackages);
        }

        static IEnumerable<PackageConfig> CreateOperatePackagesConfigsForProject()
            => CreatePackageConfigs(TypeCache.GetTypesDerivedFrom<IInitializablePackage>());

        internal static IEnumerable<PackageConfig> CreatePackageConfigs(IList<Type> packageTypes)
        {
            var packageInfoWithInitializers = new Dictionary<PackageInfo, List<Type>>(
                packageTypes.Count, new PackageInfoNameComparer());
            foreach (var packageType in packageTypes)
            {
                var packageInfo = PackageInfo.FindForAssembly(packageType.Assembly);
                if (packageInfo is null)
                    continue;

                if (packageInfoWithInitializers.TryGetValue(packageInfo, out var initializers))
                {
                    initializers.Add(packageType);
                }
                else
                {
                    packageInfoWithInitializers[packageInfo] = new List<Type>
                    {
                        packageType,
                    };
                }
            }

            return packageInfoWithInitializers.Select(x => new PackageConfig(x.Key, x.Value));
        }

        internal static void ProvidePackageConfigs(
            ConfigurationBuilder builder, IEnumerable<PackageConfig> operatePackages)
        {
            var allPackageNameBuilder = new StringBuilder();
            foreach (var packageInfo in operatePackages)
            {
                if (allPackageNameBuilder.Length > 0)
                {
                    allPackageNameBuilder.Append(AllPackageNamesSeparator);
                }

                allPackageNameBuilder.Append(packageInfo.Name);
                CreatePackageConfig(PackageVersionKeyFormat, packageInfo.Version);
                var joinedPackageInitializerNames = string.Join(
                    PackageInitializerNamesSeparator.ToString(), packageInfo.InitializerNames);
                CreatePackageConfig(PackageInitializerNamesKeyFormat, joinedPackageInitializerNames);

                void CreatePackageConfig(string keyFormat, string value)
                {
                    var configKey = string.Format(keyFormat, packageInfo.Name);
                    builder.SetString(configKey, value, true);
                }
            }

            builder.SetString(AllPackageNamesKey, allPackageNameBuilder.ToString());
        }
    }
}
