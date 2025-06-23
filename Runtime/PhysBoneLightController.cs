using UnityEngine;

#if VRCHAT_SDK_AVAILABLE
using VRC.SDK3.Avatars.Components;
#endif

namespace lilToon.PCSS.Runtime
{
    /// <summary>
    /// Simple light controller for PhysBone integration.
    /// Provides fields used by setup wizards in PCSS packages.
    /// </summary>
    [ExecuteInEditMode]
    public class PhysBoneLightController : MonoBehaviour
    {
#if VRCHAT_SDK_AVAILABLE
        /// <summary>
        /// Optional external light to control when using VRChat PhysBones.
        /// </summary>
        public Light externalLight;
#else
        /// <summary>
        /// Optional external light to control.
        /// </summary>
        public Light externalLight;
#endif
        /// <summary>
        /// Basic initialization used by setup wizards.
        /// Ensures the external light reference is assigned.
        /// </summary>
        public void Initialize()
        {
            if (externalLight == null)
            {
                externalLight = GetComponent<Light>();
            }
        }
    }
}
