using UnityEngine;

namespace Unity.Services.Core.Internal
{
    static class UnityServicesInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void CreateStaticInstance()
        {

            var corePackageRegistry = new CorePackageRegistry();
            var coreRegistry = new CoreRegistry(corePackageRegistry.Registry);

            CorePackageRegistry.Instance = corePackageRegistry;
            CoreRegistry.Instance = coreRegistry;
            var coreMetrics = new CoreMetrics();
            var coreDiagnostics = new CoreDiagnostics();

            UnityServices.Instance = new UnityServicesInternal(coreRegistry, coreMetrics, coreDiagnostics);
            UnityServices.InstantiationCompletion?.TrySetResult(null);
            CoreMetrics.Instance = coreMetrics;
            CoreDiagnostics.Instance = coreDiagnostics;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static async void EnableServicesInitializationAsync()
        {
            var instance = (UnityServicesInternal)UnityServices.Instance;
            await instance.EnableInitializationAsync();
        }

    }
}
