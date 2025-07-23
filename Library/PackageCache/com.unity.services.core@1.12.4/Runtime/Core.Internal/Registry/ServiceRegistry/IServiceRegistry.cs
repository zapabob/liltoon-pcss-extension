using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Unity.Services.Core.Internal
{
    interface IServiceRegistry
    {
        void RegisterService<T>([NotNull] T service);
        T GetService<T>();
    }
}
