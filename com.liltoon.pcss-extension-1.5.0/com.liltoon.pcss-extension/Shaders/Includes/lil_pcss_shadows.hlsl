#include "lil_pcss_common.hlsl"

#if defined(VRC_LIGHT_VOLUMES) || defined(VRC_LIGHT_VOLUMES_ENABLED)
    #if __has_include("Packages/com.vrchat.base/Runtime/Shader/VRChat_Shadow.cginc")
        #include "Packages/com.vrchat.base/Runtime/Shader/VRChat_Shadow.cginc"
    #endif
#endif

//----------------------------------------------------------------------------------------------------------------------
// PCSS Configuration
//---------------------------------------------------------------------------------------------------------------------- 