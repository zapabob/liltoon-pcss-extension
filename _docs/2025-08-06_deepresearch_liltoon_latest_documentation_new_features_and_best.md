# DeepResearch Report: liltoon latest documentation, new features, and best practices

**Date**: 2025-08-06 13:35:21  
**Research Topic**: liltoon latest documentation, new features, and best practices  
**Research Method**: comprehensive deep research with multi-level analysis  
**Language**: English and Japanese (英語・日本語)

---

## Research Summary

- **Strategy Used**: comprehensive
- **Depth Achieved**: 2/3 levels
- **Sources Analyzed**: 10/10
- **Time Taken**: 138246ms
- **Topics Explored**: 0

## Key Topics



## Detailed Analysis



--- Level 1 Analysis ---
## Level 1 Research Analysis: lilToon Latest Documentation, New Features, and Best Practices

### 1. Comprehensive Analysis

**Source Validation and Credibility Assessment:**

The primary sources for lilToon are its official GitHub repository, its BOOTH.pm page (a popular Japanese digital marketplace), and the VRChat Creator Companion (VCC) which is the official tool for managing VRChat projects. The GitHub repository provides the most up-to-date changelogs and releases, making it a highly credible source for new features. The BOOTH.pm page is the main distribution platform and provides a good overview of the shader's features. Community-driven sources like Reddit, YouTube, and other forums offer valuable insights into best practices, user experience, and comparisons with other shaders, though they should be cross-referenced for accuracy. Academic and peer-reviewed sources on specific shaders like lilToon are scarce. However, research on non-photorealistic rendering (NPR) and toon shading in virtual reality provides a broader context for understanding the technical challenges and principles behind lilToon.

**Comprehensive Analysis with Multiple Perspectives:**

*   **Developer Perspective:** The developer of lilToon focuses on providing a feature-rich, easy-to-use, and performant toon shader for avatars, primarily for VRChat. The frequent updates on GitHub demonstrate a commitment to bug fixes, new features, and optimization. The shader is designed to be highly customizable, allowing for a wide range of artistic expression.

*   **User/Creator Perspective:** Users generally praise lilToon for its ease of use and high-quality results out-of-the-box. It is often compared to another popular shader, Poiyomi, with lilToon being considered more beginner-friendly while Poiyomi is seen as having more advanced and extensive features. Many creators use a combination of both shaders on their avatars to leverage the strengths of each. The availability of pre-made material settings and tutorials on platforms like Booth and YouTube further enhances its accessibility.

*   **Technical/Academic Perspective:** From a technical standpoint, lilToon is a non-photorealistic rendering (NPR) shader designed for real-time applications like VR. A key challenge in this area is achieving stereoscopic coherence, where the shading appears correct from the slightly different perspectives of each eye in a VR headset. While not explicitly stated in the lilToon documentation, its popularity in VRChat suggests it handles this well. The shader's optimization features, which automatically disable unused functions to reduce performance overhead, are a significant technical advantage.

### 2. Related Topics and Connections

*   **Poiyomi Toon Shader:** This is the most frequently mentioned alternative to lilToon. The two are often compared, with users weighing the trade-offs between lilToon's ease of use and Poiyomi's extensive feature set. There are even tools developed by the community to convert materials between the two shaders, highlighting their prevalence in the VRChat community.

*   **VRChat and Avatar Creation:** lilToon is deeply integrated into the VRChat avatar creation ecosystem. It is available through the official VRChat Creator Companion, and many tutorials and assets are specifically designed for use with lilToon in VRChat.

*   **Non-Photorealistic Rendering (NPR):** lilToon is a practical application of NPR techniques. Academic research in NPR explores the challenges of creating stylized rendering in real-time and in VR, providing a theoretical foundation for the features seen in shaders like lilToon.

*   **Unity Shader Development:** lilToon is built on Unity's shader pipeline. Understanding the basics of Unity's ShaderLab, HLSL, and Shader Graph can provide a deeper appreciation for how lilToon functions.

### 3. Current Trends and Future Implications

*   **Ease of Use and Accessibility:** The trend in avatar shaders is towards user-friendliness without sacrificing power. lilToon exemplifies this with its intuitive interface and presets. This trend is likely to continue, with shaders becoming even more accessible to non-technical users.

*   **Performance Optimization:** As VRChat and other social VR platforms become more complex, performance optimization is crucial. lilToon's automatic optimization features are a key selling point and a trend that will likely be adopted by other shaders. The creator of lilToon has also released a separate, non-destructive mesh simplifier tool, further emphasizing the importance of optimization.

*   **Cross-Platform Compatibility:** With the rise of standalone VR headsets like the Meta Quest, cross-platform compatibility is increasingly important. Shaders that can perform well on both PC and mobile VR will have a significant advantage. While not a primary focus of this research, it's a critical consideration for the future of VRChat shaders.

*   **Advanced Lighting and Rendering Features:** The introduction of features like VRC Light Volumes, which allow for more realistic lighting in VRChat worlds, will drive the development of shaders that can take advantage of these new technologies. lilToon's support for various lighting models in Unity suggests it is well-positioned to adapt to these changes.

### 4. Contradicting Viewpoints and Debates

There are no direct "contradicting viewpoints" in the sense of factual disputes. However, there is a healthy debate and difference in preference within the community, primarily centered on **lilToon vs. Poiyomi Toon Shader**:

*   **lilToon:** Praised for its user-friendly interface and high-quality results with minimal tweaking. It's often recommended for beginners or those who want a quick, polished look.
*   **Poiyomi Toon Shader:** Valued for its vast array of features and deep customization options, appealing to users who want granular control over every aspect of their avatar's appearance. Some find its interface to be more complex.

This isn't a matter of one being definitively better, but rather a choice based on the user's skill level and desired outcome. Many experienced creators use both shaders on different parts of the same avatar to get the best of both worlds.

### 5. Practical Applications and Recommendations

*   **For Beginners:** lilToon is highly recommended for those new to VRChat avatar creation. Its intuitive interface, presets, and the wealth of tutorials available make it an excellent starting point.

*   **For Experienced Creators:** While lilToon is easy to use, it also offers a deep feature set for those willing to explore it. The ability to create custom presets can streamline workflows for creators who work on multiple avatars. For highly specific or complex effects, creators might consider using Poiyomi in conjunction with lilToon.

*   **Best Practices:**
    *   **Start with Presets:** Use the built-in presets as a starting point and then customize from there.
    *   **Optimize Your Shaders:** Take advantage of lilToon's automatic optimization features to ensure your avatar performs well in VRChat.
    *   **Use Masks for Effects:** For effects like glitter or emissions, use texture masks to control where the effect is applied. This is more performant than using separate materials.
    *   **Stay Updated:** The developer of lilToon releases frequent updates with new features and bug fixes. Regularly check the GitHub repository or the VRChat Creator Companion for the latest version.
    *   **Leverage Community Resources:** Explore tutorials on YouTube and discussions on Reddit to learn new techniques and get help with any issues you encounter.

--- Level 2 Analysis ---
## Level 2 Research Analysis: lilToon Latest Documentation, New Features, and Best Practices

Building upon the foundational understanding, this level delves into specific technical features, community-driven best practices, and the broader context of real-time rendering in which lilToon operates.

### 1. Comprehensive Analysis with Multiple Perspectives

**a. The Artist/Creator Perspective: New Expressive Features**
Recent versions of lilToon have introduced a suite of features that expand creative possibilities. Analysis of the official changelogs reveals additions such as a backlight function for more dramatic rim lighting, properties for "VR Parallax Strength" on MatCaps to enhance perceived depth, and additional UV modes for secondary and tertiary colors. A significant feature is the "Flipbook" or texture sheet animation capability, allowing creators to add animated GIFs or effects directly onto a material, which can be controlled via animation frames. For finer control over appearance, features like post-contrast for glitter and the ability to use vertex color to define outline normals have been added. These tools empower artists to achieve a more bespoke and dynamic anime aesthetic without needing to write custom shader code.

**b. The Technical Artist/Optimizer Perspective: Performance and Workflow**
lilToon is designed for performance, a critical factor in social VR platforms like VRChat. A key feature is its automatic shader modification system; the editor intelligently strips out unused features from the code before compilation, minimizing the final shader's load. Best practices within the community emphasize disabling any feature toggles not in use within the lilToon inspector. For advanced optimization, texture atlasing—combining multiple textures into a single file—is highly recommended to reduce draw calls, a primary performance bottleneck. Furthermore, lilToon's inspector has been made significantly faster in recent updates, and a function was added to automatically convert variables to constants during VRChat avatar builds, further optimizing performance.

**c. The Academic/Researcher Perspective: A Practical Application of NPR**
From an academic standpoint, lilToon is a sophisticated, real-time implementation of Non-Photorealistic Rendering (NPR), specifically cel shading. Cel shading traditionally simplifies lighting into discrete bands of color to mimic hand-drawn animation cels. lilToon advances on basic cel shading by incorporating techniques discussed in research, such as view-dependent effects (MatCaps) and stylized highlights. While most academic papers focus on the foundational algorithms of NPR or outline detection, lilToon represents a mature, user-friendly tool that packages these complex graphics concepts into an accessible interface for a mass audience.

### 2. Source Validation and Credibility Assessment

*   **Primary Sources (High Credibility):** The official lilToon GitHub repository (lilxyzw/lilToon) and its accompanying online documentation are the most credible sources for changelogs, features, and installation instructions. The BOOTH.pm page is the official distribution point for the free and paid versions. The VRChat Creator Companion (VCC) catalog is also a primary source for official distribution.
*   **Secondary Sources (Medium-High Credibility):** Community tutorials (often on YouTube or blogs like Qiita) from established VRChat creators provide credible, practical advice and best practices. These are valuable for understanding real-world application but may occasionally contain subjective or outdated information.
*   **Tertiary/Academic Sources (High but Indirect Credibility):** Peer-reviewed papers on NPR, cel shading, and real-time rendering from sources like ACM SIGGRAPH, SciTePress, and university computer science departments offer high credibility on the underlying principles. They do not mention lilToon directly but validate the scientific and technical foundations of its features.

### 3. Related Topics and Connections

*   **Non-Photorealistic Rendering (NPR):** lilToon is a sub-field of NPR, which encompasses any rendering style not aiming for photorealism.
*   **Shader Programming (HLSL):** lilToon is written in ShaderLab, C#, and HLSL (High-Level Shading Language), the language used for writing shaders in Unity. It provides a high-level interface that abstracts the complex HLSL code from the end-user.
*   **VRChat Performance Optimization:** The use and configuration of lilToon are intrinsically linked to VRChat's avatar performance ranking system. Creators must balance visual features with performance metrics like draw calls, shader complexity, and VRAM usage to ensure their avatars are not blocked by default for other users.
*   **Texture Packing:** An optimization technique where multiple grayscale maps (like smoothness, metallic, ambient occlusion) are stored in the R, G, B, and A channels of a single texture. This is a best practice for use with lilToon to reduce texture memory and sampling overhead.

### 4. Current Trends and Future Implications

*   **Trend - All-in-One "Uber-Shaders":** lilToon is part of a trend of comprehensive shaders that include a vast array of features (outlines, multiple lighting effects, fur, refraction, etc.) in a single package. This contrasts with the older approach of using many small, single-purpose shaders.
*   **Trend - User-Friendliness and Automation:** The focus on intuitive inspectors, presets, and automated optimization (like stripping unused code) is making advanced avatar creation more accessible to non-technical users.
*   **Implication - Cross-Platform Demands:** As VR platforms like VRChat push for greater mobile/standalone VR (e.g., Meta Quest) compatibility, the demand for highly scalable and performant shaders like lilToon will grow. Its ability to disable features to fit strict performance budgets is a key advantage.
*   **Implication - Procedural and Animated Materials:** The inclusion of features like flipbook animations points towards a future where more procedural and animated effects are integrated directly into the shader, reducing reliance on other components and complex animator setups.

### 5. Contradicting Viewpoints and Debates

The most prominent debate in the VRChat community is **lilToon vs. Poiyomi Toon Shader**. This is less about one being definitively superior and more about a difference in design philosophy:
*   **lilToon:** Often praised for its ease of use, high performance out-of-the-box, and ability to create a clean, authentic anime look with minimal tweaking. It is generally considered more "plug-and-play."
*   **Poiyomi Toon Shader:** Often seen as more powerful and feature-rich, offering deeper customization for complex and unique visual effects. This flexibility can come with a steeper learning curve and a higher potential performance cost if not managed carefully.
Some users even adopt a hybrid approach, using lilToon for organic surfaces like skin and hair, and Poiyomi for clothing or special effects that require its unique features.

### 6. Practical Applications and Recommendations

*   **For Beginners:** Start by using the built-in presets. After applying a material, go to the "Rendering" and "Advanced" tabs in the lilToon inspector and disable any features you are not using (e.g., if you don't need glitter, fur, or refraction, turn them off).
*   **For Performance:** Always use the VRChat SDK's tools to check your avatar's performance rank before uploading. To optimize, reduce the number of materials on your avatar by using texture atlasing. Within lilToon, after you have finalized your settings, use the "Lock" function in the inspector. This creates a new, optimized shader variant that has permanently removed all the unused features, reducing compile time and performance overhead.
*   **For Advanced Effects:** To create effects like having eyebrows render over hair, use the Stencil settings in the "Advanced" tab. By setting the hair material to write to the stencil buffer and the eyebrow material to read from it, you can control the rendering order to achieve the desired layering effect, similar to how layers work in 2D art software.
*   **General Best Practice:** Avoid using real-time lights on avatars in VRChat whenever possible, as they are extremely performance-intensive. Instead, use lilToon's built-in lighting controls, such as the "Lower brightness limit," to ensure your avatar is visible in dark worlds without using costly emission effects.

## Research Methodology

This deep research employed a multi-level analysis approach:

1. **Level 1**: Initial exploration and source identification
2. **Level 2**: Deep dive into key findings and connections  
3. **Level 3+**: Cross-validation and synthesis of insights

The research utilized Google Search grounding for real-time information and source validation.

---

## English Report



--- Level 1 Analysis ---
## Level 1 Research Analysis: lilToon Latest Documentation, New Features, and Best Practices

### 1. Comprehensive Analysis

**Source Validation and Credibility Assessment:**

**Comprehensive Analysis with Multiple Perspectives:**

*   **Developer Perspective:** The developer of lilToon focuses on providing a feature-rich, easy-to-use, and performant toon shader for avatars, primarily for VRChat. The frequent updates on GitHub demonstrate a commitment to bug fixes, new features, and optimization. The shader is designed to be highly customizable, allowing for a wide range of artistic expression.

*   **User/Creator Perspective:** Users generally praise lilToon for its ease of use and high-quality results out-of-the-box. It is often compared to another popular shader, Poiyomi, with lilToon being considered more beginner-friendly while Poiyomi is seen as having more advanced and extensive features. Many creators use a combination of both shaders on their avatars to leverage the strengths of each. The availability of pre-made material settings and tutorials on platforms like Booth and YouTube further enhances its accessibility.

*   **Technical/Academic Perspective:** From a technical standpoint, lilToon is a non-photorealistic rendering (NPR) shader designed for real-time applications like VR. A key challenge in this area is achieving stereoscopic coherence, where the shading appears correct from the slightly different perspectives of each eye in a VR headset. While not explicitly stated in the lilToon documentation, its popularity in VRChat suggests it handles this well. The shader's optimization features, which automatically disable unused functions to reduce performance overhead, are a significant technical advantage.

### 2. Related Topics and Connections

*   **Poiyomi Toon Shader:** This is the most frequently mentioned alternative to lilToon. The two are often compared, with users weighing the trade-offs between lilToon's ease of use and Poiyomi's extensive feature set. There are even tools developed by the community to convert materials between the two shaders, highlighting their prevalence in the VRChat community.

*   **VRChat and Avatar Creation:** lilToon is deeply integrated into the VRChat avatar creation ecosystem. It is available through the official VRChat Creator Companion, and many tutorials and assets are specifically designed for use with lilToon in VRChat.

*   **Non-Photorealistic Rendering (NPR):** lilToon is a practical application of NPR techniques. Academic research in NPR explores the challenges of creating stylized rendering in real-time and in VR, providing a theoretical foundation for the features seen in shaders like lilToon.

*   **Unity Shader Development:** lilToon is built on Unity's shader pipeline. Understanding the basics of Unity's ShaderLab, HLSL, and Shader Graph can provide a deeper appreciation for how lilToon functions.

### 3. Current Trends and Future Implications

*   **Ease of Use and Accessibility:** The trend in avatar shaders is towards user-friendliness without sacrificing power. lilToon exemplifies this with its intuitive interface and presets. This trend is likely to continue, with shaders becoming even more accessible to non-technical users.

*   **Performance Optimization:** As VRChat and other social VR platforms become more complex, performance optimization is crucial. lilToon's automatic optimization features are a key selling point and a trend that will likely be adopted by other shaders. The creator of lilToon has also released a separate, non-destructive mesh simplifier tool, further emphasizing the importance of optimization.

*   **Cross-Platform Compatibility:** With the rise of standalone VR headsets like the Meta Quest, cross-platform compatibility is increasingly important. Shaders that can perform well on both PC and mobile VR will have a significant advantage. While not a primary focus of this research, it's a critical consideration for the future of VRChat shaders.

*   **Advanced Lighting and Rendering Features:** The introduction of features like VRC Light Volumes, which allow for more realistic lighting in VRChat worlds, will drive the development of shaders that can take advantage of these new technologies. lilToon's support for various lighting models in Unity suggests it is well-positioned to adapt to these changes.

### 4. Contradicting Viewpoints and Debates

There are no direct "contradicting viewpoints" in the sense of factual disputes. However, there is a healthy debate and difference in preference within the community, primarily centered on **lilToon vs. Poiyomi Toon Shader**:

*   **lilToon:** Praised for its user-friendly interface and high-quality results with minimal tweaking. It's often recommended for beginners or those who want a quick, polished look.
*   **Poiyomi Toon Shader:** Valued for its vast array of features and deep customization options, appealing to users who want granular control over every aspect of their avatar's appearance. Some find its interface to be more complex.

This isn't a matter of one being definitively better, but rather a choice based on the user's skill level and desired outcome. Many experienced creators use both shaders on different parts of the same avatar to get the best of both worlds.

### 5. Practical Applications and Recommendations

*   **For Beginners:** lilToon is highly recommended for those new to VRChat avatar creation. Its intuitive interface, presets, and the wealth of tutorials available make it an excellent starting point.

*   **For Experienced Creators:** While lilToon is easy to use, it also offers a deep feature set for those willing to explore it. The ability to create custom presets can streamline workflows for creators who work on multiple avatars. For highly specific or complex effects, creators might consider using Poiyomi in conjunction with lilToon.

*   **Best Practices:**
    *   **Start with Presets:** Use the built-in presets as a starting point and then customize from there.
    *   **Optimize Your Shaders:** Take advantage of lilToon's automatic optimization features to ensure your avatar performs well in VRChat.
    *   **Use Masks for Effects:** For effects like glitter or emissions, use texture masks to control where the effect is applied. This is more performant than using separate materials.
    *   **Stay Updated:** The developer of lilToon releases frequent updates with new features and bug fixes. Regularly check the GitHub repository or the VRChat Creator Companion for the latest version.
    *   **Leverage Community Resources:** Explore tutorials on YouTube and discussions on Reddit to learn new techniques and get help with any issues you encounter.

--- Level 2 Analysis ---
## Level 2 Research Analysis: lilToon Latest Documentation, New Features, and Best Practices

Building upon the foundational understanding, this level delves into specific technical features, community-driven best practices, and the broader context of real-time rendering in which lilToon operates.

### 1. Comprehensive Analysis with Multiple Perspectives

**a. The Artist/Creator Perspective: New Expressive Features**
Recent versions of lilToon have introduced a suite of features that expand creative possibilities. Analysis of the official changelogs reveals additions such as a backlight function for more dramatic rim lighting, properties for "VR Parallax Strength" on MatCaps to enhance perceived depth, and additional UV modes for secondary and tertiary colors. A significant feature is the "Flipbook" or texture sheet animation capability, allowing creators to add animated GIFs or effects directly onto a material, which can be controlled via animation frames. For finer control over appearance, features like post-contrast for glitter and the ability to use vertex color to define outline normals have been added. These tools empower artists to achieve a more bespoke and dynamic anime aesthetic without needing to write custom shader code.

**b. The Technical Artist/Optimizer Perspective: Performance and Workflow**
lilToon is designed for performance, a critical factor in social VR platforms like VRChat. A key feature is its automatic shader modification system; the editor intelligently strips out unused features from the code before compilation, minimizing the final shader's load. Best practices within the community emphasize disabling any feature toggles not in use within the lilToon inspector. For advanced optimization, texture atlasing—combining multiple textures into a single file—is highly recommended to reduce draw calls, a primary performance bottleneck. Furthermore, lilToon's inspector has been made significantly faster in recent updates, and a function was added to automatically convert variables to constants during VRChat avatar builds, further optimizing performance.

**c. The Academic/Researcher Perspective: A Practical Application of NPR**
From an academic standpoint, lilToon is a sophisticated, real-time implementation of Non-Photorealistic Rendering (NPR), specifically cel shading. Cel shading traditionally simplifies lighting into discrete bands of color to mimic hand-drawn animation cels. lilToon advances on basic cel shading by incorporating techniques discussed in research, such as view-dependent effects (MatCaps) and stylized highlights. While most academic papers focus on the foundational algorithms of NPR or outline detection, lilToon represents a mature, user-friendly tool that packages these complex graphics concepts into an accessible interface for a mass audience.

### 2. Source Validation and Credibility Assessment

*   **Primary Sources (High Credibility):** The official lilToon GitHub repository (lilxyzw/lilToon) and its accompanying online documentation are the most credible sources for changelogs, features, and installation instructions. The BOOTH.pm page is the official distribution point for the free and paid versions. The VRChat Creator Companion (VCC) catalog is also a primary source for official distribution.
*   **Secondary Sources (Medium-High Credibility):** Community tutorials (often on YouTube or blogs like Qiita) from established VRChat creators provide credible, practical advice and best practices. These are valuable for understanding real-world application but may occasionally contain subjective or outdated information.
*   **Tertiary/Academic Sources (High but Indirect Credibility):** Peer-reviewed papers on NPR, cel shading, and real-time rendering from sources like ACM SIGGRAPH, SciTePress, and university computer science departments offer high credibility on the underlying principles. They do not mention lilToon directly but validate the scientific and technical foundations of its features.

### 3. Related Topics and Connections

*   **Non-Photorealistic Rendering (NPR):** lilToon is a sub-field of NPR, which encompasses any rendering style not aiming for photorealism.
*   **Shader Programming (HLSL):** lilToon is written in ShaderLab, C#, and HLSL (High-Level Shading Language), the language used for writing shaders in Unity. It provides a high-level interface that abstracts the complex HLSL code from the end-user.
*   **VRChat Performance Optimization:** The use and configuration of lilToon are intrinsically linked to VRChat's avatar performance ranking system. Creators must balance visual features with performance metrics like draw calls, shader complexity, and VRAM usage to ensure their avatars are not blocked by default for other users.
*   **Texture Packing:** An optimization technique where multiple grayscale maps (like smoothness, metallic, ambient occlusion) are stored in the R, G, B, and A channels of a single texture. This is a best practice for use with lilToon to reduce texture memory and sampling overhead.

### 4. Current Trends and Future Implications

*   **Trend - All-in-One "Uber-Shaders":** lilToon is part of a trend of comprehensive shaders that include a vast array of features (outlines, multiple lighting effects, fur, refraction, etc.) in a single package. This contrasts with the older approach of using many small, single-purpose shaders.
*   **Trend - User-Friendliness and Automation:** The focus on intuitive inspectors, presets, and automated optimization (like stripping unused code) is making advanced avatar creation more accessible to non-technical users.
*   **Implication - Cross-Platform Demands:** As VR platforms like VRChat push for greater mobile/standalone VR (e.g., Meta Quest) compatibility, the demand for highly scalable and performant shaders like lilToon will grow. Its ability to disable features to fit strict performance budgets is a key advantage.
*   **Implication - Procedural and Animated Materials:** The inclusion of features like flipbook animations points towards a future where more procedural and animated effects are integrated directly into the shader, reducing reliance on other components and complex animator setups.

### 5. Contradicting Viewpoints and Debates

The most prominent debate in the VRChat community is **lilToon vs. Poiyomi Toon Shader**. This is less about one being definitively superior and more about a difference in design philosophy:
*   **lilToon:** Often praised for its ease of use, high performance out-of-the-box, and ability to create a clean, authentic anime look with minimal tweaking. It is generally considered more "plug-and-play."
*   **Poiyomi Toon Shader:** Often seen as more powerful and feature-rich, offering deeper customization for complex and unique visual effects. This flexibility can come with a steeper learning curve and a higher potential performance cost if not managed carefully.
Some users even adopt a hybrid approach, using lilToon for organic surfaces like skin and hair, and Poiyomi for clothing or special effects that require its unique features.

### 6. Practical Applications and Recommendations

*   **For Beginners:** Start by using the built-in presets. After applying a material, go to the "Rendering" and "Advanced" tabs in the lilToon inspector and disable any features you are not using (e.g., if you don't need glitter, fur, or refraction, turn them off).
*   **For Performance:** Always use the VRChat SDK's tools to check your avatar's performance rank before uploading. To optimize, reduce the number of materials on your avatar by using texture atlasing. Within lilToon, after you have finalized your settings, use the "Lock" function in the inspector. This creates a new, optimized shader variant that has permanently removed all the unused features, reducing compile time and performance overhead.
*   **For Advanced Effects:** To create effects like having eyebrows render over hair, use the Stencil settings in the "Advanced" tab. By setting the hair material to write to the stencil buffer and the eyebrow material to read from it, you can control the rendering order to achieve the desired layering effect, similar to how layers work in 2D art software.
*   **General Best Practice:** Avoid using real-time lights on avatars in VRChat whenever possible, as they are extremely performance-intensive. Instead, use lilToon's built-in lighting controls, such as the "Lower brightness limit," to ensure your avatar is visible in dark worlds without using costly emission effects.

---

## 日本語レポート

The primary sources for lilToon are its official GitHub repository, its BOOTH.pm page (a popular Japanese digital marketplace), and the VRChat Creator Companion (VCC) which is the official tool for managing VRChat projects. The GitHub repository provides the most up-to-date changelogs and releases, making it a highly credible source for new features. The BOOTH.pm page is the main distribution platform and provides a good overview of the shader's features. Community-driven sources like Reddit, YouTube, and other forums offer valuable insights into best practices, user experience, and comparisons with other shaders, though they should be cross-referenced for accuracy. Academic and peer-reviewed sources on specific shaders like lilToon are scarce. However, research on non-photorealistic rendering (NPR) and toon shading in virtual reality provides a broader context for understanding the technical challenges and principles behind lilToon.

---

*Report generated by DeepResearch tool on 2025-08-06*