# DeepResearch Report: VRChat Light Volumes latest documentation, new features, and best practices

**Date**: 2025-08-06 13:34:10  
**Research Topic**: VRChat Light Volumes latest documentation, new features, and best practices  
**Research Method**: comprehensive deep research with multi-level analysis  
**Language**: English and Japanese (英語・日本語)

---

## Research Summary

- **Strategy Used**: comprehensive
- **Depth Achieved**: 2/3 levels
- **Sources Analyzed**: 10/10
- **Time Taken**: 67584ms
- **Topics Explored**: 0

## Key Topics



## Detailed Analysis



--- Level 1 Analysis ---
### Research Level 1: Unveiling VRChat Light Volumes

**1. Comprehensive Analysis with Multiple Perspectives**

VRChat Light Volumes are a significant advancement in world lighting, offering a more dynamic and realistic way to illuminate avatars and dynamic objects. This technology, developed by community member REDSIM, replaces the standard Unity Light Probes with a voxel-based system. This means that instead of relying on a few points of light data, the system creates a 3D grid of lighting information, resulting in much more nuanced and accurate lighting on moving objects.

From a **technical standpoint**, VRC Light Volumes work by baking lighting information into these voxel volumes. This pre-calculation allows for high-quality, per-pixel lighting that can even change color in real-time, without the performance cost of traditional real-time lights. The system is compatible with both the Unity Progressive lightmapper and the popular Bakery lightmapper.

From a **creator's perspective**, VRC Light Volumes are relatively easy to set up and offer a powerful tool for enhancing the visual fidelity of their worlds. The ability to have up to 32 light volumes and 128 real-time light sources simultaneously provides a great deal of creative freedom. The system also supports baked shadows for these real-time lights, further increasing realism.

From a **user's perspective**, the difference is immediately noticeable. Avatars appear more naturally integrated into the environment, with lighting and shadows that accurately reflect the surrounding world. This is especially apparent with the recent integration of VRC Light Volumes into popular avatar shaders like Poiyomi and lilToon.

**2. Source Validation and Credibility Assessment**

The primary source of information on VRC Light Volumes is the official GitHub repository maintained by the creator, REDSIM. This is the most credible and up-to-date source, providing detailed documentation, installation guides, and the latest releases. The information is further validated by its presence on the VRChat Package Manager (VPM) Catalog, indicating its acceptance and use within the community.

Tutorials and articles from reputable VRChat content creators and communities, such as the 80 Level article and various YouTube tutorials, provide practical insights and corroborate the information from the official documentation. The widespread adoption by popular shader developers like Poiyomi also lends significant credibility to the technology. While forum discussions on platforms like Reddit offer valuable community perspectives, they should be cross-referenced with official sources for technical accuracy.

**3. Related Topics and Connections**

The development of VRC Light Volumes is intrinsically linked to the broader topic of **real-time rendering and optimization** in virtual reality. The need for performant yet visually impressive lighting is a constant challenge in VR development. VRC Light Volumes can be seen as a community-driven solution to the limitations of Unity's built-in lighting systems for the specific use case of VRChat.

This technology also connects to the concept of **baked lighting** and **lightmaps**, which are fundamental techniques for optimizing lighting in game development. VRC Light Volumes builds upon these principles by extending them to dynamic objects in a more sophisticated way than traditional light probes.

Furthermore, the use of **Udon**, VRChat's scripting language, is relevant as it can be used to control and manipulate lighting within a world, including potentially interacting with VRC Light Volumes.

**4. Current Trends and Future Implications**

The most significant trend is the increasing adoption of VRC Light Volumes by both world creators and avatar shader developers. This is creating a more visually consistent and immersive experience across the platform. The recent release of VRC Light Volumes 2.0, with its major optimizations and new features like Point Light Volumes and baked shadow masks, indicates a continued evolution of the technology.

The future implications are a potential shift in the standard for VRChat world lighting. As more creators adopt this system, there may be an expectation from users for this level of visual fidelity. This could also spur further innovation in community-developed tools and technologies for VRChat. There is also a desire from the community for VRChat to officially integrate or support this system to ensure its longevity and compatibility, especially for mobile platforms.

**5. Contradicting Viewpoints and Debates**

While the reception to VRC Light Volumes has been overwhelmingly positive, there are some underlying debates and considerations. A key point of discussion is the **trade-off between visual quality and performance**. While VRC Light Volumes are highly optimized, they still require processing power, and creators need to be mindful of the overall performance of their worlds, especially for users on lower-end hardware or Quest.

Another point of discussion is the **learning curve**. While the basic setup is straightforward, mastering the more advanced features and achieving optimal results requires a good understanding of lighting principles and Unity's lighting pipeline.

Finally, there's the ongoing debate about **community-made solutions versus official VRChat features**. While the community has proven to be incredibly innovative, reliance on third-party tools can lead to fragmentation and potential compatibility issues with future VRChat updates.

**6. Practical Applications and Recommendations**

For **world creators**, the primary application of VRC Light Volumes is to create more realistic and immersive lighting for avatars and dynamic objects. To get started, it is recommended to:
*   Install VRC Light Volumes through the VRChat Creator Companion.
*   Follow the official documentation on the GitHub page for setup and best practices.
*   Utilize baked lighting for static objects to maximize performance.
*   Use a limited number of real-time lights and leverage the baked shadow features of VRC Light Volumes.
*   Consider providing attribution to the creator to help promote the technology.

For **avatar creators**, the recommendation is to use shaders that are compatible with VRC Light Volumes, such as the latest versions of Poiyomi or lilToon. This will ensure that avatars are lit correctly in worlds that use this system.

For **users**, the best way to experience the benefits of VRC Light Volumes is to seek out worlds that have implemented the technology and to use an avatar with a compatible shader. This will result in a more visually stunning and cohesive experience in VRChat.

--- Level 2 Analysis ---
### Research Level 2: Deeper Dive into VRChat Light Volumes

Building on the foundational understanding that VRChat Light Volumes are a community-developed, voxel-based replacement for Unity's standard light probes, this next level of analysis explores the technical nuances, practical applications, and the broader context of this lighting system within the VRChat ecosystem.

---

#### 1. Comprehensive Analysis with Multiple Perspectives

**Technical Perspective:** VRC Light Volumes, created by community member REDSIM, function by baking lighting data into a 3D texture (a voxel grid) that dynamic objects, like avatars, can sample from in real-time. This provides highly detailed and localized lighting information, a significant step up from Unity's native Light Probe Groups which interpolate lighting from a sparse collection of points. The latest major release, v2.0.0, has evolved the system dramatically. It's no longer just a probe replacement but a comprehensive lighting solution, adding features like up to 128 optimized point, spot, and area lights, baked shadows for these lights, and custom light shapes. This allows for complex and dynamic lighting scenarios that were previously performance-prohibitive with Unity's real-time lights.

**Creator's Perspective (REDSIM):** The official GitHub repository and documentation emphasize ease of use, performance, and expanded creative possibilities. The system is designed to work alongside standard lightmappers like Unity Progressive and the popular community tool Bakery. Installation has been streamlined via the VRChat Creator Companion, indicating a move towards becoming a standard tool for world creators. REDSIM encourages its use by providing documentation for both world and shader developers, fostering a collaborative ecosystem.

**User/Avatar Creator Perspective:** For players and avatar creators, the impact is significant. Avatars in worlds with Light Volumes appear more naturally integrated into the environment, picking up colored light and shadows accurately. This solves a long-standing issue where avatars often looked "photoshopped in" or disconnected from the world's lighting. The adoption of Light Volumes by major avatar shaders like Poiyomi and lilToon, which now support it by default, means many users benefit automatically without needing to adjust their materials.

#### 2. Source Validation and Credibility Assessment

*   **Primary Sources (High Credibility):** The most credible sources are the official **REDSIM GitHub repository (`REDSIM/VRCLightVolumes`)** and the associated documentation. These provide direct, authoritative information on features, installation, and best practices from the developer.
*   **Community & Tool Developer Sources (High Credibility):** Announcements and documentation from major shader developers like **Poiyomi** are also highly credible, as they detail the integration and benefits from an application standpoint. Articles from industry-adjacent publications like **80 Level** offer well-researched summaries of new releases.
*   **VRChat Official Documentation (Medium-High Credibility):** While not specifically documenting this third-party tool, the official VRChat creator docs provide essential context on general lighting and optimization principles, validating the problems that Light Volumes aim to solve (e.g., the high cost of real-time lights).
*   **Academic & Technical Papers (Medium Credibility for Context):** Academic papers on real-time global illumination and lighting in VR provide a theoretical and technical background for why voxel-based solutions are effective. They confirm the importance of realistic lighting for immersion but often focus on non-consumer or research-specific rendering systems.
*   **Community Discussion (Medium-Low Credibility):** Reddit threads and YouTube tutorials are valuable for gauging community sentiment, identifying common issues, and finding practical tips. However, the information can be anecdotal or outdated and should be cross-referenced with primary sources.

#### 3. Related Topics and Connections

*   **Baked vs. Real-time Lighting:** Light Volumes exist in a middle ground. The core environmental lighting is pre-calculated ("baked"), ensuring high performance. However, it provides this baked data to dynamic objects in real-time, creating a dynamic effect without the high cost of traditional real-time lights.
*   **Bakery GPU Lightmapper:** Light Volumes are not a replacement for Bakery but a complement. Bakery excels at creating high-quality static lightmaps for the world geometry. Light Volumes excel at providing lighting for everything *else* (avatars, dynamic props). The two are designed to work together for a complete, high-fidelity lighting solution.
*   **Udon and UdonSharp:** Udon is VRChat's custom scripting language. While Light Volumes' core is shader-based, its integration and potential for dynamic control (e.g., changing light colors at runtime) connect it to the Udon ecosystem, which enables interactive world elements.
*   **Avatar Shaders (Poiyomi/lilToon):** The success of Light Volumes is inextricably linked to its adoption by popular avatar shaders. Without shader support, avatars cannot sample the voxel data, making this collaboration essential for the system's widespread impact.

#### 4. Current Trends and Future Implications

*   **Trend Towards Dynamic Realism:** Light Volumes are part of a broader trend in VRChat moving away from static, flatly lit worlds towards more dynamic, immersive, and visually rich environments. This is driven by community innovation filling gaps left by the official SDK.
*   **Future Platform Integration:** The widespread adoption and proven benefit of Light Volumes could pressure the VRChat team to either officially integrate it or develop a comparable native solution. Unity's own engine development, such as Adaptive Probe Volumes in the Universal Render Pipeline (URP), points to industry recognition of this need, though these are not available in VRChat's current render pipeline.
*   **Performance vs. Fidelity:** As PC hardware improves, the performance overhead of systems like Light Volumes becomes more acceptable, pushing the baseline for visual quality higher. However, this also widens the gap between PC-only worlds and Quest-compatible worlds, where performance constraints are much stricter.

#### 5. Contradicting Viewpoints and Debates

*   **Complexity vs. Benefit:** While powerful, setting up Light Volumes adds another layer of complexity to world creation compared to just using Unity's default light probes. For simpler scenes or creators new to lighting, the added effort might not be seen as worthwhile.
*   **Performance Cost:** While advertised as performant, any advanced lighting system carries a cost. On lower-end systems or in worlds with many avatars, the overhead of sampling the 3D texture and rendering the additional analytic lights could still impact framerates. The debate centers on whether the visual gain is worth a potential performance hit, especially in crowded instances.
*   **Community vs. Official Tools:** The existence and popularity of Light Volumes sparks a debate about VRChat's role in providing creation tools. Some argue that the platform should have developed these features natively long ago. Others believe this kind of community-led innovation is a core strength of the VRChat ecosystem, allowing for faster and more specialized development than a corporate roadmap might.

#### 6. Practical Applications and Recommendations

*   **Best Practices:**
    *   **Installation:** Use the VRChat Creator Companion and REDSIM's VPM listing for the easiest and most reliable installation.
    *   **Collaboration with Bakery:** Use Bakery to bake your static lightmaps for the world geometry, and then use Light Volumes to handle the lighting for dynamic objects and add performant real-time light sources.
    *   **Shader Compatibility:** Ensure avatars use up-to-date versions of compatible shaders (like Poiyomi or lilToon) to take advantage of the system.
    *   **Optimization:** While v2.0 is heavily optimized, creators should still be mindful of the number and complexity of light volumes and real-time lights used, especially for worlds intended to be high-occupancy.
*   **Recommendations:**
    *   **For High-Fidelity Worlds:** For PC-only worlds aiming for maximum visual immersion (e.g., clubs, showcases, social hubs), using Light Volumes is highly recommended. The improvement in how avatars are lit is dramatic and significantly enhances the feeling of presence.
    *   **For Dynamic Lighting Effects:** The new point light features in v2.0 are ideal for creating performant dynamic lighting, such as flashing lights in a club, flickering torches, or other effects that need to affect avatars, without resorting to expensive native real-time lights.
    *   **For Quest Worlds:** The performance implications must be carefully tested. While the system is optimized, Quest hardware is limited. It may be more suitable for smaller, controlled scenes on Quest rather than large, complex environments. Standard light probes remain a safer, albeit less visually impressive, option for Quest optimization.

## Research Methodology

This deep research employed a multi-level analysis approach:

1. **Level 1**: Initial exploration and source identification
2. **Level 2**: Deep dive into key findings and connections  
3. **Level 3+**: Cross-validation and synthesis of insights

The research utilized Google Search grounding for real-time information and source validation.

---

## English Report



--- Level 1 Analysis ---
### Research Level 1: Unveiling VRChat Light Volumes

**1. Comprehensive Analysis with Multiple Perspectives**

VRChat Light Volumes are a significant advancement in world lighting, offering a more dynamic and realistic way to illuminate avatars and dynamic objects. This technology, developed by community member REDSIM, replaces the standard Unity Light Probes with a voxel-based system. This means that instead of relying on a few points of light data, the system creates a 3D grid of lighting information, resulting in much more nuanced and accurate lighting on moving objects.

From a **technical standpoint**, VRC Light Volumes work by baking lighting information into these voxel volumes. This pre-calculation allows for high-quality, per-pixel lighting that can even change color in real-time, without the performance cost of traditional real-time lights. The system is compatible with both the Unity Progressive lightmapper and the popular Bakery lightmapper.

From a **creator's perspective**, VRC Light Volumes are relatively easy to set up and offer a powerful tool for enhancing the visual fidelity of their worlds. The ability to have up to 32 light volumes and 128 real-time light sources simultaneously provides a great deal of creative freedom. The system also supports baked shadows for these real-time lights, further increasing realism.

From a **user's perspective**, the difference is immediately noticeable. Avatars appear more naturally integrated into the environment, with lighting and shadows that accurately reflect the surrounding world. This is especially apparent with the recent integration of VRC Light Volumes into popular avatar shaders like Poiyomi and lilToon.

**2. Source Validation and Credibility Assessment**

The primary source of information on VRC Light Volumes is the official GitHub repository maintained by the creator, REDSIM. This is the most credible and up-to-date source, providing detailed documentation, installation guides, and the latest releases. The information is further validated by its presence on the VRChat Package Manager (VPM) Catalog, indicating its acceptance and use within the community.

Tutorials and articles from reputable VRChat content creators and communities, such as the 80 Level article and various YouTube tutorials, provide practical insights and corroborate the information from the official documentation. The widespread adoption by popular shader developers like Poiyomi also lends significant credibility to the technology. While forum discussions on platforms like Reddit offer valuable community perspectives, they should be cross-referenced with official sources for technical accuracy.

**3. Related Topics and Connections**

The development of VRC Light Volumes is intrinsically linked to the broader topic of **real-time rendering and optimization** in virtual reality. The need for performant yet visually impressive lighting is a constant challenge in VR development. VRC Light Volumes can be seen as a community-driven solution to the limitations of Unity's built-in lighting systems for the specific use case of VRChat.

This technology also connects to the concept of **baked lighting** and **lightmaps**, which are fundamental techniques for optimizing lighting in game development. VRC Light Volumes builds upon these principles by extending them to dynamic objects in a more sophisticated way than traditional light probes.

Furthermore, the use of **Udon**, VRChat's scripting language, is relevant as it can be used to control and manipulate lighting within a world, including potentially interacting with VRC Light Volumes.

**4. Current Trends and Future Implications**

The most significant trend is the increasing adoption of VRC Light Volumes by both world creators and avatar shader developers. This is creating a more visually consistent and immersive experience across the platform. The recent release of VRC Light Volumes 2.0, with its major optimizations and new features like Point Light Volumes and baked shadow masks, indicates a continued evolution of the technology.

The future implications are a potential shift in the standard for VRChat world lighting. As more creators adopt this system, there may be an expectation from users for this level of visual fidelity. This could also spur further innovation in community-developed tools and technologies for VRChat. There is also a desire from the community for VRChat to officially integrate or support this system to ensure its longevity and compatibility, especially for mobile platforms.

**5. Contradicting Viewpoints and Debates**

While the reception to VRC Light Volumes has been overwhelmingly positive, there are some underlying debates and considerations. A key point of discussion is the **trade-off between visual quality and performance**. While VRC Light Volumes are highly optimized, they still require processing power, and creators need to be mindful of the overall performance of their worlds, especially for users on lower-end hardware or Quest.

Another point of discussion is the **learning curve**. While the basic setup is straightforward, mastering the more advanced features and achieving optimal results requires a good understanding of lighting principles and Unity's lighting pipeline.

Finally, there's the ongoing debate about **community-made solutions versus official VRChat features**. While the community has proven to be incredibly innovative, reliance on third-party tools can lead to fragmentation and potential compatibility issues with future VRChat updates.

**6. Practical Applications and Recommendations**

For **world creators**, the primary application of VRC Light Volumes is to create more realistic and immersive lighting for avatars and dynamic objects. To get started, it is recommended to:
*   Install VRC Light Volumes through the VRChat Creator Companion.
*   Follow the official documentation on the GitHub page for setup and best practices.
*   Utilize baked lighting for static objects to maximize performance.
*   Use a limited number of real-time lights and leverage the baked shadow features of VRC Light Volumes.
*   Consider providing attribution to the creator to help promote the technology.

For **avatar creators**, the recommendation is to use shaders that are compatible with VRC Light Volumes, such as the latest versions of Poiyomi or lilToon. This will ensure that avatars are lit correctly in worlds that use this system.

For **users**, the best way to experience the benefits of VRC Light Volumes is to seek out worlds that have implemented the technology and to use an avatar with a compatible shader. This will result in a more visually stunning and cohesive experience in VRChat.

--- Level 2 Analysis ---
### Research Level 2: Deeper Dive into VRChat Light Volumes

Building on the foundational understanding that VRChat Light Volumes are a community-developed, voxel-based replacement for Unity's standard light probes, this next level of analysis explores the technical nuances, practical applications, and the broader context of this lighting system within the VRChat ecosystem.

---

#### 1. Comprehensive Analysis with Multiple Perspectives

**Technical Perspective:** VRC Light Volumes, created by community member REDSIM, function by baking lighting data into a 3D texture (a voxel grid) that dynamic objects, like avatars, can sample from in real-time. This provides highly detailed and localized lighting information, a significant step up from Unity's native Light Probe Groups which interpolate lighting from a sparse collection of points. The latest major release, v2.0.0, has evolved the system dramatically. It's no longer just a probe replacement but a comprehensive lighting solution, adding features like up to 128 optimized point, spot, and area lights, baked shadows for these lights, and custom light shapes. This allows for complex and dynamic lighting scenarios that were previously performance-prohibitive with Unity's real-time lights.

**Creator's Perspective (REDSIM):** The official GitHub repository and documentation emphasize ease of use, performance, and expanded creative possibilities. The system is designed to work alongside standard lightmappers like Unity Progressive and the popular community tool Bakery. Installation has been streamlined via the VRChat Creator Companion, indicating a move towards becoming a standard tool for world creators. REDSIM encourages its use by providing documentation for both world and shader developers, fostering a collaborative ecosystem.

**User/Avatar Creator Perspective:** For players and avatar creators, the impact is significant. Avatars in worlds with Light Volumes appear more naturally integrated into the environment, picking up colored light and shadows accurately. This solves a long-standing issue where avatars often looked "photoshopped in" or disconnected from the world's lighting. The adoption of Light Volumes by major avatar shaders like Poiyomi and lilToon, which now support it by default, means many users benefit automatically without needing to adjust their materials.

#### 2. Source Validation and Credibility Assessment

*   **Primary Sources (High Credibility):** The most credible sources are the official **REDSIM GitHub repository (`REDSIM/VRCLightVolumes`)** and the associated documentation. These provide direct, authoritative information on features, installation, and best practices from the developer.
*   **Community & Tool Developer Sources (High Credibility):** Announcements and documentation from major shader developers like **Poiyomi** are also highly credible, as they detail the integration and benefits from an application standpoint. Articles from industry-adjacent publications like **80 Level** offer well-researched summaries of new releases.
*   **VRChat Official Documentation (Medium-High Credibility):** While not specifically documenting this third-party tool, the official VRChat creator docs provide essential context on general lighting and optimization principles, validating the problems that Light Volumes aim to solve (e.g., the high cost of real-time lights).
*   **Academic & Technical Papers (Medium Credibility for Context):** Academic papers on real-time global illumination and lighting in VR provide a theoretical and technical background for why voxel-based solutions are effective. They confirm the importance of realistic lighting for immersion but often focus on non-consumer or research-specific rendering systems.
*   **Community Discussion (Medium-Low Credibility):** Reddit threads and YouTube tutorials are valuable for gauging community sentiment, identifying common issues, and finding practical tips. However, the information can be anecdotal or outdated and should be cross-referenced with primary sources.

#### 3. Related Topics and Connections

*   **Baked vs. Real-time Lighting:** Light Volumes exist in a middle ground. The core environmental lighting is pre-calculated ("baked"), ensuring high performance. However, it provides this baked data to dynamic objects in real-time, creating a dynamic effect without the high cost of traditional real-time lights.
*   **Bakery GPU Lightmapper:** Light Volumes are not a replacement for Bakery but a complement. Bakery excels at creating high-quality static lightmaps for the world geometry. Light Volumes excel at providing lighting for everything *else* (avatars, dynamic props). The two are designed to work together for a complete, high-fidelity lighting solution.
*   **Udon and UdonSharp:** Udon is VRChat's custom scripting language. While Light Volumes' core is shader-based, its integration and potential for dynamic control (e.g., changing light colors at runtime) connect it to the Udon ecosystem, which enables interactive world elements.
*   **Avatar Shaders (Poiyomi/lilToon):** The success of Light Volumes is inextricably linked to its adoption by popular avatar shaders. Without shader support, avatars cannot sample the voxel data, making this collaboration essential for the system's widespread impact.

#### 4. Current Trends and Future Implications

*   **Trend Towards Dynamic Realism:** Light Volumes are part of a broader trend in VRChat moving away from static, flatly lit worlds towards more dynamic, immersive, and visually rich environments. This is driven by community innovation filling gaps left by the official SDK.
*   **Future Platform Integration:** The widespread adoption and proven benefit of Light Volumes could pressure the VRChat team to either officially integrate it or develop a comparable native solution. Unity's own engine development, such as Adaptive Probe Volumes in the Universal Render Pipeline (URP), points to industry recognition of this need, though these are not available in VRChat's current render pipeline.
*   **Performance vs. Fidelity:** As PC hardware improves, the performance overhead of systems like Light Volumes becomes more acceptable, pushing the baseline for visual quality higher. However, this also widens the gap between PC-only worlds and Quest-compatible worlds, where performance constraints are much stricter.

#### 5. Contradicting Viewpoints and Debates

*   **Complexity vs. Benefit:** While powerful, setting up Light Volumes adds another layer of complexity to world creation compared to just using Unity's default light probes. For simpler scenes or creators new to lighting, the added effort might not be seen as worthwhile.
*   **Performance Cost:** While advertised as performant, any advanced lighting system carries a cost. On lower-end systems or in worlds with many avatars, the overhead of sampling the 3D texture and rendering the additional analytic lights could still impact framerates. The debate centers on whether the visual gain is worth a potential performance hit, especially in crowded instances.
*   **Community vs. Official Tools:** The existence and popularity of Light Volumes sparks a debate about VRChat's role in providing creation tools. Some argue that the platform should have developed these features natively long ago. Others believe this kind of community-led innovation is a core strength of the VRChat ecosystem, allowing for faster and more specialized development than a corporate roadmap might.

#### 6. Practical Applications and Recommendations

*   **Best Practices:**
    *   **Installation:** Use the VRChat Creator Companion and REDSIM's VPM listing for the easiest and most reliable installation.
    *   **Collaboration with Bakery:** Use Bakery to bake your static lightmaps for the world geometry, and then use Light Volumes to handle the lighting for dynamic objects and add performant real-time light sources.
    *   **Shader Compatibility:** Ensure avatars use up-to-date versions of compatible shaders (like Poiyomi or lilToon) to take advantage of the system.
    *   **Optimization:** While v2.0 is heavily optimized, creators should still be mindful of the number and complexity of light volumes and real-time lights used, especially for worlds intended to be high-occupancy.
*   **Recommendations:**
    *   **For High-Fidelity Worlds:** For PC-only worlds aiming for maximum visual immersion (e.g., clubs, showcases, social hubs), using Light Volumes is highly recommended. The improvement in how avatars are lit is dramatic and significantly enhances the feeling of presence.
    *   **For Dynamic Lighting Effects:** The new point light features in v2.0 are ideal for creating performant dynamic lighting, such as flashing lights in a club, flickering torches, or other effects that need to affect avatars, without resorting to expensive native real-time lights.
    *   **For Quest Worlds:** The performance implications must be carefully tested. While the system is optimized, Quest hardware is limited. It may be more suitable for smaller, controlled scenes on Quest rather than large, complex environments. Standard light probes remain a safer, albeit less visually impressive, option for Quest optimization.

---

## 日本語レポート



---

*Report generated by DeepResearch tool on 2025-08-06*