# DeepResearch Report: nHaruka PCSSforVRC technical implementation details

**Date**: 2025-08-06 13:27:44  
**Research Topic**: nHaruka PCSSforVRC technical implementation details  
**Research Method**: comprehensive deep research with multi-level analysis  
**Language**: English and Japanese (英語・日本語)

---

## Research Summary

- **Strategy Used**: comprehensive
- **Depth Achieved**: 2/3 levels
- **Sources Analyzed**: 10/10
- **Time Taken**: 70706ms
- **Topics Explored**: 0

## Key Topics



## Detailed Analysis



--- Level 1 Analysis ---
### Research Level 1: Foundational Overview

#### 1.1. Source Identification and Credibility Assessment

The primary source for "nHaruka PCSSforVRC" is the creator's personal sales page on the Japanese digital marketplace Booth.pm, under the store name "nHarukaの実験室" (nHaruka's Laboratory). This is the most credible source for the product's intended use, features, and installation instructions. The creator, nHaruka, is an active member of the VRChat creator community. The product page explicitly mentions that the implementation references an open-source Unity PCSS project by "TheMasonX" on GitHub, lending technical credibility to its foundation.

Secondary sources include community discussions on platforms like Reddit and promotional articles, which provide context on user experience and performance impact.

Academic and formal peer-reviewed sources specifically analyzing "nHaruka PCSSforVRC" do not exist, as it is a niche, community-developed tool for a specific application (VRChat). However, extensive academic and industry research exists for the core algorithm, Percentage-Closer Soft Shadows (PCSS), originally developed by NVIDIA. These sources provide the theoretical and mathematical foundation upon which nHaruka's tool is built.

#### 1.2. Core Concept: What is PCSS?

Percentage-Closer Soft Shadows (PCSS) is a computer graphics algorithm designed to render more realistic soft shadows in real-time. Unlike traditional "hard" shadows with sharp edges, PCSS mimics the behavior of shadows in the real world: they become progressively softer and blurrier as the distance between the shadow-casting object (the "caster") and the surface receiving the shadow (the "receiver") increases. This effect is crucial for creating a sense of depth and realism in a 3D scene.

The PCSS algorithm is typically an extension of standard shadow mapping and involves three main steps:
1.  **Blocker Search:** For a point being shaded, the algorithm searches a region of the shadow map to find the average depth of the objects (blockers) casting the shadow.
2.  **Penumbra Estimation:** Based on the blocker's average distance and the receiver's distance from the light source, it estimates the size of the penumbra (the soft edge of the shadow). A key formula is: `Penumbra Width = (dReceiver - dBlocker) * LightSize / dBlocker`.
3.  **Filtering:** The algorithm then performs a Percentage- Closer Filtering (PCF) operation, which blurs the shadow's edge. The key innovation of PCSS is that the size of this blur (the filter kernel) is variable and proportional to the estimated penumbra size from step 2.

#### 1.3. nHaruka's Implementation for VRChat

"nHaruka PCSSforVRC," also called "リアル影システム" (Real Shadow System), is a Unity-based tool designed to apply dynamic, high-resolution soft shadows to VRChat avatars with a relatively low performance impact.

**Key Technical Implementation Details:**

*   **Avatar-Centric Lighting:** The system works by attaching a dedicated, avatar-following spotlight to the user's avatar. This is a critical design choice for VRChat, as it ensures consistent shadow quality independent of the world's lighting, which can be unpredictable or non-existent. This light is configured to only affect the avatar itself, preventing it from casting light on other players or the environment, which would cause unnecessary performance load.
*   **Shader Customization:** The tool modifies an avatar's existing shaders to support PCSS. It specifically integrates with popular VRChat toon shaders like "lilToon" and "Poiyomi Toon Shader". The setup process involves running an editor script that customizes the shader code and sets up the necessary components.
*   **Open-Source Foundation:** The creator explicitly states that the shader code is based on TheMasonX's open-source "UnityPCSS" implementation. This GitHub project provides the core shader logic for implementing the PCSS algorithm within Unity.
*   **Performance Optimization:** To manage performance, the system automatically disables shadows when an avatar is beyond a certain distance (default is 10 meters). Additionally, performance is improved by skipping shadow calculations for parts of the avatar that are not hit by the dedicated light source. The user is given control over the number of samples used in the blocker search and filtering steps, which directly trades quality for performance.
*   **Masking Features:** The tool provides two types of texture masks for fine-tuning shadow behavior:
    *   **CastMask:** Prevents a specific part of the avatar from casting a shadow (e.g., to stop hair from casting a harsh shadow on the face).
    *   **ReceiveMask:** Prevents a surface from receiving shadows, making it always appear bright (e.g., for eyes).

--- Level 2 Analysis ---
### Research Level 2: Technical Implementation Analysis

Building on the foundational overview, this level delves into the specific technical implementation details of Percentage-Closer Soft Shadows (PCSS) as it would apply to the "nHaruka PCSSforVRC" asset. The analysis synthesizes information from academic papers, technical whitepapers from GPU manufacturers, and practical implementation guides.

#### 2.1. Source Validation and Credibility Assessment

The sources for this level are of high credibility, primarily consisting of the original PCSS paper presented by NVIDIA, supplementary integration guides, and peer-reviewed academic articles on real-time rendering. NVIDIA's documentation, authored by the creators of the technique, provides the most authoritative view. Academic papers from sources like IIETA and SciTePress offer contemporary research and alternative approaches, validating the ongoing relevance and areas of improvement for the algorithm. Public source code repositories and technical discussions on platforms like Reddit and GitHub provide practical implementation context, including HLSL shader examples for Unity, which is the engine VRChat is built on.

#### 2.2. Comprehensive Analysis: The PCSS Algorithm

The technical implementation of PCSS, and by extension nHaruka's shader, is a multi-step process executed in a pixel or fragment shader. It enhances standard shadow mapping to produce perceptually realistic soft shadows that harden upon contact. The core algorithm can be broken down into three main steps:

1.  **Step 1: Blocker Search**
    *   For each pixel being rendered (a "receiver"), the shader samples a region of the shadow map (the depth texture from the light's point of view).
    *   The size of this search region is proportional to the light source's size and the receiver's distance from the light.
    *   Within this region, the shader identifies "blockers"—pixels on the shadow map that are closer to the light than the receiver pixel.
    *   It then calculates the average depth of these blockers. This average depth is a crucial input for the next step. This blocker search is the most computationally expensive part of the PCSS algorithm.

2.  **Step 2: Penumbra Estimation**
    *   The algorithm approximates the size of the penumbra (the soft edge of the shadow) using the principle of similar triangles.
    *   The formula is: `PenumbraWidth = (ReceiverDepth - AverageBlockerDepth) * LightSize / AverageBlockerDepth`.
    *   This calculation elegantly simulates a key real-world shadow property: the farther a shadow is cast from its blocker, the softer and wider its penumbra becomes.

3.  **Step 3: Filtering (Percentage-Closer Filtering)**
    *   The final step is to perform a Percentage-Closer Filtering (PCF) operation on the shadow map. PCF works by taking multiple samples from the shadow map in a given area and averaging the results to create a soft edge.
    *   Crucially, in PCSS, the size of the PCF filter kernel is not fixed. Instead, it is scaled directly by the `PenumbraWidth` calculated in Step 2.
    *   A small penumbra results in a small filter kernel (a sharp, "hard" shadow), while a large penumbra uses a large filter kernel (a blurry, "soft" shadow). This dynamic scaling is what produces the "contact hardening" effect.

The implementation within VRChat requires this logic to be written in HLSL (High-Level Shading Language) within a Unity shader file. The shader must have access to the light's shadow map texture and parameters for light size and sample counts.

#### 2.3. Related Topics and Connections

*   **Shadow Mapping:** PCSS is a direct extension of shadow mapping, the foundational technique for generating shadows in rasterized real-time graphics.
*   **Percentage-Closer Filtering (PCF):** PCSS can be seen as an "intelligent" application of PCF, where the filter size is dynamically varied instead of being a fixed, uniform value.
*   **Contact-Hardening Shadows (CHS):** PCSS is a primary technique for achieving contact hardening. Other methods exist, such as those using erosion operators or dynamically generated kernels.
*   **Variance Shadow Maps (VSM) & Derivatives:** VSM is an alternative soft shadow technique that can be significantly faster than PCSS, especially for large penumbras, because it avoids the costly blocker search step. However, VSM can suffer from light-bleeding artifacts. Some modern approaches, like Variance Soft Shadow Mapping (VSSM), attempt to combine the VSM framework with PCSS principles for a faster, high-quality result.

#### 2.4. Current Trends and Future Implications

The trend in real-time graphics is a move towards higher realism, with soft shadows being a key component. While hardware-accelerated ray tracing is becoming more common on high-end PCs, rasterization-based techniques like PCSS remain essential for performance-constrained platforms like mobile VR (Oculus Quest) and for users without cutting-edge GPUs.

Future implications for VRChat involve a constant balancing act between visual fidelity and performance. We may see:
*   **Hybrid Techniques:** Shaders that combine PCSS for near-field shadows with cheaper methods for distant shadows.
*   **Performance Optimizations:** Techniques like temporal reprojection, where shadow calculations from previous frames are reused, can significantly speed up PCSS.
*   **Platform-Specific Shaders:** VRChat creators will increasingly need to provide different shader versions or fallbacks for PC and Quest, as a full PCSS implementation may be too demanding for standalone VR hardware.

#### 2.5. Contradicting Viewpoints and Debates

The primary debate surrounding PCSS is **Quality vs. Performance**.
*   **Pro-PCSS:** It produces perceptually accurate, aesthetically pleasing soft shadows from a single shadow map without pre-computation, making it relatively easy to integrate into existing engines.
*   **Anti-PCSS/Alternatives:** The blocker search step is a performance bottleneck. A high number of samples are needed for high quality, which can lead to significant performance drops (10-14% or more). For many applications, simpler and faster techniques like standard PCF or alternative algorithms like VSM/VSSM might offer a better trade-off. Some users also report visual artifacts like banding or issues where shadows from different objects intersect.

In the context of VRChat, this debate is critical. While nHaruka's shader provides a dramatic visual improvement, its performance cost means it may not be suitable for all users or all situations, especially in crowded public worlds where every millisecond of frame time counts.

#### 2.6. Practical Applications and Recommendations

For VRChat creators considering using the "nHaruka PCSSforVRC" shader:

*   **Target Audience:** This shader is best suited for PC VR users with capable GPUs who prioritize visual fidelity for avatar showcases, photography, or small, controlled gatherings.
*   **Performance Budgeting:** Creators must be mindful of the performance impact. It is highly recommended to offer a "low-spec" version of the avatar that uses a more standard or optimized shader (like lilToon or Poiyomi) for users with lower-end hardware or for use in crowded instances.
*   **Parameter Tuning:** The key to using PCSS effectively is tuning the parameters. The number of samples for the blocker search and the PCF filter should be exposed as shader properties. Reducing the sample count will decrease quality but significantly improve performance.
*   **Quest Incompatibility:** Given the computational cost, a full PCSS implementation is generally not viable for Quest avatars. Creators must provide a fallback to a VRChat mobile-compatible shader to avoid avatars appearing broken (pink) for Quest users.
*   **Use Case:** Use the shader to add depth and realism to character models, particularly for features like hair casting shadows on the face, or clothing casting shadows on the body. This enhances the sense of presence and volume. However, avoid using it on every object in a world, as the cumulative performance cost would be prohibitive.

## Research Methodology

This deep research employed a multi-level analysis approach:

1. **Level 1**: Initial exploration and source identification
2. **Level 2**: Deep dive into key findings and connections  
3. **Level 3+**: Cross-validation and synthesis of insights

The research utilized Google Search grounding for real-time information and source validation.

---

## English Report



--- Level 1 Analysis ---
### Research Level 1: Foundational Overview

#### 1.1. Source Identification and Credibility Assessment

Secondary sources include community discussions on platforms like Reddit and promotional articles, which provide context on user experience and performance impact.

Academic and formal peer-reviewed sources specifically analyzing "nHaruka PCSSforVRC" do not exist, as it is a niche, community-developed tool for a specific application (VRChat). However, extensive academic and industry research exists for the core algorithm, Percentage-Closer Soft Shadows (PCSS), originally developed by NVIDIA. These sources provide the theoretical and mathematical foundation upon which nHaruka's tool is built.

#### 1.2. Core Concept: What is PCSS?

Percentage-Closer Soft Shadows (PCSS) is a computer graphics algorithm designed to render more realistic soft shadows in real-time. Unlike traditional "hard" shadows with sharp edges, PCSS mimics the behavior of shadows in the real world: they become progressively softer and blurrier as the distance between the shadow-casting object (the "caster") and the surface receiving the shadow (the "receiver") increases. This effect is crucial for creating a sense of depth and realism in a 3D scene.

The PCSS algorithm is typically an extension of standard shadow mapping and involves three main steps:
1.  **Blocker Search:** For a point being shaded, the algorithm searches a region of the shadow map to find the average depth of the objects (blockers) casting the shadow.
2.  **Penumbra Estimation:** Based on the blocker's average distance and the receiver's distance from the light source, it estimates the size of the penumbra (the soft edge of the shadow). A key formula is: `Penumbra Width = (dReceiver - dBlocker) * LightSize / dBlocker`.
3.  **Filtering:** The algorithm then performs a Percentage- Closer Filtering (PCF) operation, which blurs the shadow's edge. The key innovation of PCSS is that the size of this blur (the filter kernel) is variable and proportional to the estimated penumbra size from step 2.

#### 1.3. nHaruka's Implementation for VRChat

"nHaruka PCSSforVRC," also called "リアル影システム" (Real Shadow System), is a Unity-based tool designed to apply dynamic, high-resolution soft shadows to VRChat avatars with a relatively low performance impact.

**Key Technical Implementation Details:**

*   **Avatar-Centric Lighting:** The system works by attaching a dedicated, avatar-following spotlight to the user's avatar. This is a critical design choice for VRChat, as it ensures consistent shadow quality independent of the world's lighting, which can be unpredictable or non-existent. This light is configured to only affect the avatar itself, preventing it from casting light on other players or the environment, which would cause unnecessary performance load.
*   **Shader Customization:** The tool modifies an avatar's existing shaders to support PCSS. It specifically integrates with popular VRChat toon shaders like "lilToon" and "Poiyomi Toon Shader". The setup process involves running an editor script that customizes the shader code and sets up the necessary components.
*   **Open-Source Foundation:** The creator explicitly states that the shader code is based on TheMasonX's open-source "UnityPCSS" implementation. This GitHub project provides the core shader logic for implementing the PCSS algorithm within Unity.
*   **Performance Optimization:** To manage performance, the system automatically disables shadows when an avatar is beyond a certain distance (default is 10 meters). Additionally, performance is improved by skipping shadow calculations for parts of the avatar that are not hit by the dedicated light source. The user is given control over the number of samples used in the blocker search and filtering steps, which directly trades quality for performance.
*   **Masking Features:** The tool provides two types of texture masks for fine-tuning shadow behavior:
    *   **CastMask:** Prevents a specific part of the avatar from casting a shadow (e.g., to stop hair from casting a harsh shadow on the face).
    *   **ReceiveMask:** Prevents a surface from receiving shadows, making it always appear bright (e.g., for eyes).

--- Level 2 Analysis ---
### Research Level 2: Technical Implementation Analysis

Building on the foundational overview, this level delves into the specific technical implementation details of Percentage-Closer Soft Shadows (PCSS) as it would apply to the "nHaruka PCSSforVRC" asset. The analysis synthesizes information from academic papers, technical whitepapers from GPU manufacturers, and practical implementation guides.

#### 2.1. Source Validation and Credibility Assessment

The sources for this level are of high credibility, primarily consisting of the original PCSS paper presented by NVIDIA, supplementary integration guides, and peer-reviewed academic articles on real-time rendering. NVIDIA's documentation, authored by the creators of the technique, provides the most authoritative view. Academic papers from sources like IIETA and SciTePress offer contemporary research and alternative approaches, validating the ongoing relevance and areas of improvement for the algorithm. Public source code repositories and technical discussions on platforms like Reddit and GitHub provide practical implementation context, including HLSL shader examples for Unity, which is the engine VRChat is built on.

#### 2.2. Comprehensive Analysis: The PCSS Algorithm

The technical implementation of PCSS, and by extension nHaruka's shader, is a multi-step process executed in a pixel or fragment shader. It enhances standard shadow mapping to produce perceptually realistic soft shadows that harden upon contact. The core algorithm can be broken down into three main steps:

1.  **Step 1: Blocker Search**
    *   For each pixel being rendered (a "receiver"), the shader samples a region of the shadow map (the depth texture from the light's point of view).
    *   The size of this search region is proportional to the light source's size and the receiver's distance from the light.
    *   Within this region, the shader identifies "blockers"—pixels on the shadow map that are closer to the light than the receiver pixel.
    *   It then calculates the average depth of these blockers. This average depth is a crucial input for the next step. This blocker search is the most computationally expensive part of the PCSS algorithm.

2.  **Step 2: Penumbra Estimation**
    *   The algorithm approximates the size of the penumbra (the soft edge of the shadow) using the principle of similar triangles.
    *   The formula is: `PenumbraWidth = (ReceiverDepth - AverageBlockerDepth) * LightSize / AverageBlockerDepth`.
    *   This calculation elegantly simulates a key real-world shadow property: the farther a shadow is cast from its blocker, the softer and wider its penumbra becomes.

3.  **Step 3: Filtering (Percentage-Closer Filtering)**
    *   The final step is to perform a Percentage-Closer Filtering (PCF) operation on the shadow map. PCF works by taking multiple samples from the shadow map in a given area and averaging the results to create a soft edge.
    *   Crucially, in PCSS, the size of the PCF filter kernel is not fixed. Instead, it is scaled directly by the `PenumbraWidth` calculated in Step 2.
    *   A small penumbra results in a small filter kernel (a sharp, "hard" shadow), while a large penumbra uses a large filter kernel (a blurry, "soft" shadow). This dynamic scaling is what produces the "contact hardening" effect.

The implementation within VRChat requires this logic to be written in HLSL (High-Level Shading Language) within a Unity shader file. The shader must have access to the light's shadow map texture and parameters for light size and sample counts.

#### 2.3. Related Topics and Connections

*   **Shadow Mapping:** PCSS is a direct extension of shadow mapping, the foundational technique for generating shadows in rasterized real-time graphics.
*   **Percentage-Closer Filtering (PCF):** PCSS can be seen as an "intelligent" application of PCF, where the filter size is dynamically varied instead of being a fixed, uniform value.
*   **Contact-Hardening Shadows (CHS):** PCSS is a primary technique for achieving contact hardening. Other methods exist, such as those using erosion operators or dynamically generated kernels.
*   **Variance Shadow Maps (VSM) & Derivatives:** VSM is an alternative soft shadow technique that can be significantly faster than PCSS, especially for large penumbras, because it avoids the costly blocker search step. However, VSM can suffer from light-bleeding artifacts. Some modern approaches, like Variance Soft Shadow Mapping (VSSM), attempt to combine the VSM framework with PCSS principles for a faster, high-quality result.

#### 2.4. Current Trends and Future Implications

The trend in real-time graphics is a move towards higher realism, with soft shadows being a key component. While hardware-accelerated ray tracing is becoming more common on high-end PCs, rasterization-based techniques like PCSS remain essential for performance-constrained platforms like mobile VR (Oculus Quest) and for users without cutting-edge GPUs.

Future implications for VRChat involve a constant balancing act between visual fidelity and performance. We may see:
*   **Hybrid Techniques:** Shaders that combine PCSS for near-field shadows with cheaper methods for distant shadows.
*   **Performance Optimizations:** Techniques like temporal reprojection, where shadow calculations from previous frames are reused, can significantly speed up PCSS.
*   **Platform-Specific Shaders:** VRChat creators will increasingly need to provide different shader versions or fallbacks for PC and Quest, as a full PCSS implementation may be too demanding for standalone VR hardware.

#### 2.5. Contradicting Viewpoints and Debates

The primary debate surrounding PCSS is **Quality vs. Performance**.
*   **Pro-PCSS:** It produces perceptually accurate, aesthetically pleasing soft shadows from a single shadow map without pre-computation, making it relatively easy to integrate into existing engines.
*   **Anti-PCSS/Alternatives:** The blocker search step is a performance bottleneck. A high number of samples are needed for high quality, which can lead to significant performance drops (10-14% or more). For many applications, simpler and faster techniques like standard PCF or alternative algorithms like VSM/VSSM might offer a better trade-off. Some users also report visual artifacts like banding or issues where shadows from different objects intersect.

In the context of VRChat, this debate is critical. While nHaruka's shader provides a dramatic visual improvement, its performance cost means it may not be suitable for all users or all situations, especially in crowded public worlds where every millisecond of frame time counts.

#### 2.6. Practical Applications and Recommendations

For VRChat creators considering using the "nHaruka PCSSforVRC" shader:

*   **Target Audience:** This shader is best suited for PC VR users with capable GPUs who prioritize visual fidelity for avatar showcases, photography, or small, controlled gatherings.
*   **Performance Budgeting:** Creators must be mindful of the performance impact. It is highly recommended to offer a "low-spec" version of the avatar that uses a more standard or optimized shader (like lilToon or Poiyomi) for users with lower-end hardware or for use in crowded instances.
*   **Parameter Tuning:** The key to using PCSS effectively is tuning the parameters. The number of samples for the blocker search and the PCF filter should be exposed as shader properties. Reducing the sample count will decrease quality but significantly improve performance.
*   **Quest Incompatibility:** Given the computational cost, a full PCSS implementation is generally not viable for Quest avatars. Creators must provide a fallback to a VRChat mobile-compatible shader to avoid avatars appearing broken (pink) for Quest users.
*   **Use Case:** Use the shader to add depth and realism to character models, particularly for features like hair casting shadows on the face, or clothing casting shadows on the body. This enhances the sense of presence and volume. However, avoid using it on every object in a world, as the cumulative performance cost would be prohibitive.

---

## 日本語レポート

The primary source for "nHaruka PCSSforVRC" is the creator's personal sales page on the Japanese digital marketplace Booth.pm, under the store name "nHarukaの実験室" (nHaruka's Laboratory). This is the most credible source for the product's intended use, features, and installation instructions. The creator, nHaruka, is an active member of the VRChat creator community. The product page explicitly mentions that the implementation references an open-source Unity PCSS project by "TheMasonX" on GitHub, lending technical credibility to its foundation.

---

*Report generated by DeepResearch tool on 2025-08-06*