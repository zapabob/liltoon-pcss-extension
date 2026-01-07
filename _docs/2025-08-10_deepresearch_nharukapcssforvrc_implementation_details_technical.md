# DeepResearch Report: nHarukaのPCSSForVRC implementation details technical specifications

**Date**: 2025-08-10 18:43:06  
**Research Topic**: nHarukaのPCSSForVRC implementation details technical specifications  
**Research Method**: comprehensive deep research with multi-level analysis  
**Language**: English and Japanese (英語・日本語)

---

## Research Summary

- **Strategy Used**: comprehensive
- **Depth Achieved**: 1/3 levels
- **Sources Analyzed**: 5/5
- **Time Taken**: 23359ms
- **Topics Explored**: 0

## Key Topics



## Detailed Analysis



--- Level 1 Analysis ---
I have initiated the research analysis on "nHarukaのPCSSForVRC implementation details technical specifications." My initial search has yielded highly relevant information, primarily from the product's official BOOTH page, which appears to be directly from the creator, nHaruka.

**Research Level 1 Summary:**

**Key Implementation Details and Technical Specifications (from Source):**

*   **Core Technology:** nHaruka's PCSS For VRC is a customized implementation of Percentage-Closer Soft Shadows (PCSS) specifically adapted for VRChat avatars. PCSS is a graphics technique that dynamically adjusts the softness of shadows based on the distance between the light source, occluder, and receiver.
*   **Objective:** The system aims to render high-resolution, real-time shadows on VRChat avatars with a relatively low performance impact, thereby enhancing the avatar's sense of three-dimensionality and presence.
*   **Technical Foundation:** The underlying shaders are developed with reference to the open-source project `TheMasonX/UnityPCSS` on GitHub.
*   **Shader Compatibility:** Integration requires the use of either `lilToon` (v1.7.2 or newer) or `Poiyomi Pro/Toon` (v9.0.54 or newer) shaders, which are then customized for PCSS and embedded into the avatar.
*   **Lighting System:**
    *   The system minimizes the influence of global world lighting within VRChat.
    *   It primarily utilizes a dedicated spotlight that is set up on the avatar and follows its movement, ensuring consistent lighting and shadow effects regardless of the VRChat world environment.
    *   The spotlight is configured to illuminate the avatar's head.
    *   Directional control of the spotlight is possible via PhysBone.
    *   Users can adjust the light's intensity and color (hue and saturation) through the avatar's ExpressionMenu.
    *   An option exists to synchronize the avatar's light color with the world's ambient light.
*   **Performance Optimizations:**
    *   Designed for efficiency, the system automatically disables shadows when the avatar is beyond a default distance of 10 meters from the viewer.
    *   Version 4.1.0 introduced `CullingMatrixOverride` and `BoundingSphereOverride` to enhance performance by overriding Unity's default light and shadow influence ranges.
    *   Recommendations for optimal performance include avatar optimization (e.g., material consolidation, lower polygon counts) and specific settings for `Shadow Distance`, `Test Samplers` (<=12), and `Filter Samplers` (<=24).
    *   A "ShadowCastAddon" enables casting shadows onto world objects with minimal performance overhead due to its screen-space implementation.
*   **Masking Capabilities:**
    *   **CastMask:** Allows users to prevent specific avatar parts from casting shadows (e.g., preventing shadows from appearing around the eyes).
    *   **ReceiveMask:** Enables users to prevent certain avatar areas from receiving shadows, ensuring they remain consistently bright (e.g., for artistic lighting adjustments).
    *   Masks are created using white textures, with desired masked areas painted black, and then applied through material properties.
*   **Setup and Compatibility:** The system includes a one-click setup tool. It requires Unity 2022.3.22f1, VRChatSDK Avatar 3.7.2 or later, and the specified `lilToon` or `Poiyomi` shader versions. It is not compatible with VRChat Quest avatars.
*   **Version Notes:** Version 5.0.0 of the system deprecated an earlier NGSS (Next Generation Soft Shadows) implementation.

**Source Validation and Credibility Assessment (Level 1):**

*   **Source (nHaruka's BOOTH page):** This is considered a highly credible primary source as it directly provides detailed technical specifications, features, and setup instructions from the creator. The information is recent, with updates mentioned as late as November 2024 for an addon.
*   **Sources, (NVIDIA PCSS documentation):** These are foundational technical documents from NVIDIA, providing the theoretical basis for the PCSS algorithm. While highly credible for the core technology, they are older (2005) and thus not within the "last 5 years" for the specific VRChat implementation. They are crucial for understanding the underlying principles.
*   **Sources, (General Soft Shadow/PCSS explanations):** These sources offer general explanations of soft shadows and PCSS concepts. Source is from 2018, falling within the 5-year window. They contribute to a broader understanding of the technology.
*   **Source (TikTok video):** This recent (March 2024) user-generated content confirms many details from Source and provides a practical, user-oriented perspective on nHaruka's system. It adds to the credibility by showing real-world usage.
*   **Other Sources (,,,,,,):** These sources discuss general VRChat shadow issues, avatar optimization, or related assets. While relevant to the broader VRChat context, they do not directly provide specific implementation details for nHaruka's PCSS.

**Initial Assessment for Academic/Peer-Reviewed Sources:**
Direct academic or peer-reviewed sources specifically detailing "nHaruka's PCSS For VRC" are unlikely to exist, as it's a highly specialized application within a game engine and social VR platform. The most authoritative technical details come from the creator's product page. However, the underlying PCSS algorithm itself is well-documented in academic/technical papers (e.g., NVIDIA's original work). For deeper academic analysis, I will focus on the principles of PCSS and related real-time shadow techniques, connecting them to nHaruka's practical application.

**Next Steps (Research Level 2):**
I will now proceed to Research Level 2, focusing on:
1.  A deeper dive into the core PCSS algorithm using the NVIDIA sources to understand its principles (blocker search, penumbra estimation, PCF).
2.  Investigating the `TheMasonX/UnityPCSS` GitHub repository, as it's referenced as the basis for nHaruka's shaders, to gain more granular technical insights.
3.  Broadening the search for recent academic papers on real-time soft shadow techniques in VR/game environments to identify current trends and potential future implications, even if they don't specifically mention VRChat or nHaruka.
4.  Searching for user discussions, reviews, or technical breakdowns of nHaruka's PCSS For VRC to gather diverse perspectives and identify any common issues or debates.

## Research Methodology

This deep research employed a multi-level analysis approach:

1. **Level 1**: Initial exploration and source identification
2. **Level 2**: Deep dive into key findings and connections  
3. **Level 3+**: Cross-validation and synthesis of insights

The research utilized Google Search grounding for real-time information and source validation.

---

## English Report



--- Level 1 Analysis ---
I have initiated the research analysis on "nHarukaのPCSSForVRC implementation details technical specifications." My initial search has yielded highly relevant information, primarily from the product's official BOOTH page, which appears to be directly from the creator, nHaruka.

**Research Level 1 Summary:**

**Key Implementation Details and Technical Specifications (from Source):**

*   **Core Technology:** nHaruka's PCSS For VRC is a customized implementation of Percentage-Closer Soft Shadows (PCSS) specifically adapted for VRChat avatars. PCSS is a graphics technique that dynamically adjusts the softness of shadows based on the distance between the light source, occluder, and receiver.
*   **Objective:** The system aims to render high-resolution, real-time shadows on VRChat avatars with a relatively low performance impact, thereby enhancing the avatar's sense of three-dimensionality and presence.
*   **Technical Foundation:** The underlying shaders are developed with reference to the open-source project `TheMasonX/UnityPCSS` on GitHub.
*   **Shader Compatibility:** Integration requires the use of either `lilToon` (v1.7.2 or newer) or `Poiyomi Pro/Toon` (v9.0.54 or newer) shaders, which are then customized for PCSS and embedded into the avatar.
*   **Lighting System:**
    *   The system minimizes the influence of global world lighting within VRChat.
    *   It primarily utilizes a dedicated spotlight that is set up on the avatar and follows its movement, ensuring consistent lighting and shadow effects regardless of the VRChat world environment.
    *   The spotlight is configured to illuminate the avatar's head.
    *   Directional control of the spotlight is possible via PhysBone.
    *   Users can adjust the light's intensity and color (hue and saturation) through the avatar's ExpressionMenu.
    *   An option exists to synchronize the avatar's light color with the world's ambient light.
*   **Performance Optimizations:**
    *   Designed for efficiency, the system automatically disables shadows when the avatar is beyond a default distance of 10 meters from the viewer.
    *   Version 4.1.0 introduced `CullingMatrixOverride` and `BoundingSphereOverride` to enhance performance by overriding Unity's default light and shadow influence ranges.
    *   Recommendations for optimal performance include avatar optimization (e.g., material consolidation, lower polygon counts) and specific settings for `Shadow Distance`, `Test Samplers` (<=12), and `Filter Samplers` (<=24).
    *   A "ShadowCastAddon" enables casting shadows onto world objects with minimal performance overhead due to its screen-space implementation.
*   **Masking Capabilities:**
    *   **CastMask:** Allows users to prevent specific avatar parts from casting shadows (e.g., preventing shadows from appearing around the eyes).
    *   **ReceiveMask:** Enables users to prevent certain avatar areas from receiving shadows, ensuring they remain consistently bright (e.g., for artistic lighting adjustments).
    *   Masks are created using white textures, with desired masked areas painted black, and then applied through material properties.
*   **Setup and Compatibility:** The system includes a one-click setup tool. It requires Unity 2022.3.22f1, VRChatSDK Avatar 3.7.2 or later, and the specified `lilToon` or `Poiyomi` shader versions. It is not compatible with VRChat Quest avatars.
*   **Version Notes:** Version 5.0.0 of the system deprecated an earlier NGSS (Next Generation Soft Shadows) implementation.

**Source Validation and Credibility Assessment (Level 1):**

*   **Source (nHaruka's BOOTH page):** This is considered a highly credible primary source as it directly provides detailed technical specifications, features, and setup instructions from the creator. The information is recent, with updates mentioned as late as November 2024 for an addon.
*   **Sources, (NVIDIA PCSS documentation):** These are foundational technical documents from NVIDIA, providing the theoretical basis for the PCSS algorithm. While highly credible for the core technology, they are older (2005) and thus not within the "last 5 years" for the specific VRChat implementation. They are crucial for understanding the underlying principles.
*   **Sources, (General Soft Shadow/PCSS explanations):** These sources offer general explanations of soft shadows and PCSS concepts. Source is from 2018, falling within the 5-year window. They contribute to a broader understanding of the technology.
*   **Source (TikTok video):** This recent (March 2024) user-generated content confirms many details from Source and provides a practical, user-oriented perspective on nHaruka's system. It adds to the credibility by showing real-world usage.
*   **Other Sources (,,,,,,):** These sources discuss general VRChat shadow issues, avatar optimization, or related assets. While relevant to the broader VRChat context, they do not directly provide specific implementation details for nHaruka's PCSS.

**Initial Assessment for Academic/Peer-Reviewed Sources:**
Direct academic or peer-reviewed sources specifically detailing "nHaruka's PCSS For VRC" are unlikely to exist, as it's a highly specialized application within a game engine and social VR platform. The most authoritative technical details come from the creator's product page. However, the underlying PCSS algorithm itself is well-documented in academic/technical papers (e.g., NVIDIA's original work). For deeper academic analysis, I will focus on the principles of PCSS and related real-time shadow techniques, connecting them to nHaruka's practical application.

**Next Steps (Research Level 2):**
I will now proceed to Research Level 2, focusing on:
1.  A deeper dive into the core PCSS algorithm using the NVIDIA sources to understand its principles (blocker search, penumbra estimation, PCF).
2.  Investigating the `TheMasonX/UnityPCSS` GitHub repository, as it's referenced as the basis for nHaruka's shaders, to gain more granular technical insights.
3.  Broadening the search for recent academic papers on real-time soft shadow techniques in VR/game environments to identify current trends and potential future implications, even if they don't specifically mention VRChat or nHaruka.
4.  Searching for user discussions, reviews, or technical breakdowns of nHaruka's PCSS For VRC to gather diverse perspectives and identify any common issues or debates.

---

## 日本語レポート



---

*Report generated by DeepResearch tool on 2025-08-10*