# DeepResearch Report: ModularAvatar latest documentation, features, and best practices

**Date**: 2025-08-01 04:52:45  
**Research Topic**: ModularAvatar latest documentation, features, and best practices  
**Research Method**: comprehensive deep research with multi-level analysis  
**Language**: English and Japanese (英語・日本語)

---

## Research Summary

- **Strategy Used**: comprehensive
- **Depth Achieved**: 2/3 levels
- **Sources Analyzed**: 10/10
- **Time Taken**: 81572ms
- **Topics Explored**: 0

## Key Topics



## Detailed Analysis



--- Level 1 Analysis ---
### **Research Level 1: Foundational Overview of Modular Avatar**

**1.1. What is Modular Avatar?**

Modular Avatar is a comprehensive suite of non-destructive tools designed for the Unity game engine, primarily for customizing avatars for the social platform VRChat. Its core philosophy is to allow users to add, modify, or remove components like clothing, accessories, and features to an avatar without permanently altering the avatar's original files. This is achieved by merging components at build time (when the avatar is uploaded to VRChat or enters play mode in Unity), rather than directly in the editor. This "non-destructive" workflow means that assets can be easily removed or updated without needing to manually undo complex changes.

**1.2. Primary Purpose and Target Audience**

The primary purpose of Modular Avatar is to simplify and streamline the VRChat avatar customization process. It makes adding features like new outfits or gimmicks as simple as dragging and dropping a prefab into the Unity hierarchy. The target audience includes:

*   **Avatar Creators:** Who can package their assets (clothing, accessories) to be easily installable by users.
*   **End-Users:** Who want to customize their avatars without needing deep technical knowledge of Unity or VRChat's animation systems.
*   **Developers:** Who can leverage Modular Avatar's underlying framework (NDMF) to create their own compatible tools.

**1.3. Core Features**

Modular Avatar's functionality is delivered through a series of components that can be added to GameObjects in Unity. The key features include:

*   **Armature Merging:** It can intelligently merge the bone structure (armature) of an outfit or accessory with the main avatar's armature. It minimizes the creation of new bones by reusing existing ones where possible.
*   **Animator Merging:** It allows for the merging of multiple animator controllers. This is crucial for adding new expressions, gestures, or toggles for clothing and objects without conflicting with the avatar's base animation controller.
*   **Non-Destructive Workflow:** All modifications are applied at runtime or build time, leaving the original avatar and accessory prefabs untouched. This makes it easy to remove items by simply deleting them from the hierarchy.
*   **Menu System Integration:** It provides tools to easily add new menus and submenus to the VRChat expression menu, allowing for in-game toggling of outfits and props.
*   **Blendshape Syncing:** It can synchronize blendshape values between different meshes, for example, to make an outfit's shape conform to the body's shape changes.
*   **Prefab-based Distribution:** Creators can set up their assets with Modular Avatar components, making installation for the end-user a simple drag-and-drop process.

**1.4. Installation**

The recommended way to install Modular Avatar is through the VRChat Creator Companion (VCC), which is the standard tool for managing VRChat projects. Users can add the Modular Avatar repository to the VCC to easily install and update the package. For pre-release versions, the documentation sometimes recommends alternative community-made package managers due to potential bugs in VCC's dependency resolution.

**1.5. Source Validation and Credibility Assessment**

The primary sources for information on Modular Avatar are its official documentation website and its GitHub repository. These are considered highly credible as they are maintained by the developer of the tool. The VRChat Ask Forum and various YouTube tutorials from experienced VRChat creators also provide valuable and practical insights, though the information may be specific to a particular version or use case. There is a lack of traditional academic or peer-reviewed sources, as Modular Avatar is a community-developed tool for a specific niche application. The information is highly practical and focused on implementation within the VRChat ecosystem. The documentation also exists in other languages, like Chinese, which points to a wide user base. There are also companion libraries available, like "Animator As Code," which extend Modular Avatar's functionality, further cementing its importance in the VRChat creator community.

--- Level 2 Analysis ---
### **Research Level 2: In-Depth Analysis of Modular Avatar**

Building upon the foundational understanding of Modular Avatar, this level delves deeper into its technical features, best practices, and its position within the broader VRChat avatar creation ecosystem.

#### **1. Comprehensive Analysis with Multiple Perspectives**

**1.1. Core Functionality: A Deeper Dive**

Modular Avatar's primary strength lies in its non-destructive workflow, a concept repeatedly emphasized in both official documentation and community guides. This means that instead of directly and permanently altering an avatar's base files, Modular Avatar applies modifications at runtime (when you enter play mode in Unity or upload to VRChat). This is a significant paradigm shift from older, "destructive" methods where merging an outfit or accessory was a one-way process, making it difficult to undo changes or manage multiple configurations.

The system is built on a series of components that you add to your avatar project in Unity. Key components include:

*   **MA Merge Armature:** This is the core of outfit and accessory integration. It intelligently merges the bone structure of a clothing item with the avatar's main armature, reusing existing bones where possible to minimize performance impact.
*   **Animator Merging:** Modular Avatar allows you to split your avatar's animation controller into smaller, more manageable parts. This is invaluable for organizing complex animations for toggles, gestures, and other features.
*   **Menu System:** The tool provides an intuitive way to edit and extend VRChat's expression menus, making it simple to add new toggles and options for your modular components.
*   **Blendshape Syncing:** This feature automatically syncs blendshape values between the avatar's body and clothing. For example, if you have a "breast size" blendshape on your avatar, Modular Avatar can ensure that a shirt's blendshapes adjust accordingly.
*   **Reactive Components:** This is a more advanced feature that allows for the creation of dynamic interactions and toggles with minimal manual animation work. For instance, you can set up a system where equipping one item automatically unequips another.

**1.2. User Perspectives**

*   **For the Beginner:** The "drag-and-drop" nature of Modular Avatar significantly lowers the barrier to entry for avatar customization. The "Setup Outfit" feature, for example, automates much of the initial configuration process. Numerous YouTube tutorials cater to this audience, walking them through the basics of adding clothing, creating toggles, and managing menus.

*   **For the Advanced User/Creator:** Modular Avatar offers a high degree of control and flexibility. Advanced users can manually configure components, create complex animator setups, and leverage reactive components for sophisticated avatar features. The ability to distribute assets as self-contained prefabs with Modular Avatar components already attached is a major boon for creators selling clothing and accessories.

*   **The Developer Perspective:** The underlying framework, NDMF (Non-Destructive Modular Framework), is designed to be extensible, allowing other developers to create their own tools that integrate with Modular Avatar. This fosters a collaborative and innovative ecosystem around avatar creation.

#### **2. Source Validation and Credibility Assessment**

The information gathered comes from a variety of sources, each with its own level of credibility:

*   **Official Documentation (High Credibility):** The official Modular Avatar website and GitHub repository are the most reliable sources for information on the tool's features and installation. The documentation is actively maintained by the developer.
*   **Community Guides and Tutorials (Medium-High Credibility):** Tutorials on platforms like YouTube and forums like the VRChat Ask Forum are generally reliable, especially when they come from experienced creators. However, it's important to be mindful of the date of the content, as Modular Avatar is under active development and older tutorials may not reflect the latest features or best practices.
*   **Forum Discussions (Medium Credibility):** Reddit and other forums can be valuable for gauging community sentiment, discovering niche tips, and understanding common issues. However, the information can be anecdotal and may not always be accurate.
*   **Academic and Peer-Reviewed Sources (Limited Direct Relevance):** While there is a growing body of academic research on avatar customization and its psychological effects, these studies do not directly address Modular Avatar as a specific tool. Their value lies in providing a broader context for the importance of avatar customization in virtual environments.

#### **3. Related Topics and Connections**

*   **Avatar Optimization:** Modular Avatar is often used in conjunction with optimization tools like **Avatar Optimizer**. While Modular Avatar simplifies the process of adding components, it's still crucial to be mindful of performance metrics like polygon count, material slots, and texture memory to ensure your avatar performs well in VRChat.
*   **VRCFury:** This is another popular non-destructive avatar creation tool. While both tools share a similar philosophy, they have different approaches. VRCFury is often described as an "all-in-one" solution, while Modular Avatar is more of a foundational framework that can be extended with other tools.
*   **Blender and 3D Modeling:** While Modular Avatar simplifies the process within Unity, a foundational understanding of 3D modeling concepts and software like Blender is still highly beneficial, especially for tasks like weight painting, UV mapping, and creating custom assets.
*   **Generative AI:** The rise of generative AI tools for 3D model and texture creation presents an interesting future for avatar creation. These tools could potentially automate even more of the asset creation process, allowing users to focus on the creative aspects of avatar design.

#### **4. Current Trends and Future Implications**

*   **The Shift to Non-Destructive Workflows:** The popularity of tools like Modular Avatar and VRCFury indicates a clear trend towards non-destructive workflows in VRChat avatar creation. This approach is more flexible, easier to manage, and less prone to errors than older, destructive methods.
*   **Increased Accessibility:** By lowering the technical barriers to entry, Modular Avatar is empowering a wider range of users to customize their avatars. This is leading to a more diverse and creative avatar ecosystem in VRChat.
*   **Focus on Performance:** As avatars become more complex, there is a growing emphasis on performance optimization. Tools that can help users create performant avatars without sacrificing visual quality will become increasingly important.
*   **The Future of Avatar Identity:** The ongoing evolution of avatar creation tools is intertwined with broader questions of digital identity. As it becomes easier to create and customize avatars, users will have even more opportunities to express themselves and explore different facets of their identity in virtual worlds.

#### **5. Contradicting Viewpoints and Debates**

*   **Modular Avatar vs. VRCFury:** There is an ongoing debate in the community about which tool is "better." The consensus seems to be that both are excellent tools with different strengths. VRCFury is often recommended for beginners due to its all-in-one nature, while Modular Avatar is favored by those who want more control and extensibility.
*   **The "Best" Way to Create Toggles:** While Modular Avatar's built-in toggle creation system is powerful, some users still prefer to create their animations and menus manually for maximum control. The "best" approach often comes down to personal preference and the specific needs of the project.
*   **The Role of Manual Work:** While tools like Modular Avatar automate many tasks, there is still a significant amount of manual work involved in creating a high-quality avatar. Skills in 3D modeling, texturing, and animation remain highly valuable.

#### **6. Practical Applications and Recommendations**

*   **For New Users:**
    *   Start with the official Modular Avatar documentation and tutorials.
    *   Watch beginner-friendly YouTube tutorials to get a visual understanding of the workflow.
    *   Begin with simple projects, like adding a single piece of clothing or creating a basic toggle.
    *   Don't be afraid to experiment, as the non-destructive nature of Modular Avatar makes it easy to undo mistakes.

*   **For Content Creators:**
    *   Structure your assets as prefabs with Modular Avatar components pre-configured for easy installation by your customers.
    *   Provide clear documentation and instructions on how to use your assets with Modular Avatar.
    *   Consider using Modular Avatar's reactive components to create more dynamic and interactive clothing and accessories.

*   **Best Practices:**
    *   **Stay Updated:** Modular Avatar is in active development, so make sure you are using the latest version.
    *   **Keep it Organized:** Use empty GameObjects to group your modular components and keep your hierarchy clean.
    *   **Optimize, Optimize, Optimize:** Always be mindful of performance. Use tools like Avatar Optimizer and follow VRChat's optimization guidelines.
    *   **Test Thoroughly:** Use a tool like Gesture Manager to test your avatar's expressions and toggles in Unity before uploading to VRChat.
    *   **Backup Your Work:** While Modular Avatar is non-destructive, it's always a good practice to back up your Unity project regularly.
    *   **Be Aware of SDK Updates:** VRChat SDK updates can sometimes introduce issues with tools like Modular Avatar. It's a good idea to wait a bit before updating to a new SDK version and to check for any reported compatibility problems.

## Research Methodology

This deep research employed a multi-level analysis approach:

1. **Level 1**: Initial exploration and source identification
2. **Level 2**: Deep dive into key findings and connections  
3. **Level 3+**: Cross-validation and synthesis of insights

The research utilized Google Search grounding for real-time information and source validation.

---

## English Report



--- Level 1 Analysis ---
### **Research Level 1: Foundational Overview of Modular Avatar**

**1.1. What is Modular Avatar?**

Modular Avatar is a comprehensive suite of non-destructive tools designed for the Unity game engine, primarily for customizing avatars for the social platform VRChat. Its core philosophy is to allow users to add, modify, or remove components like clothing, accessories, and features to an avatar without permanently altering the avatar's original files. This is achieved by merging components at build time (when the avatar is uploaded to VRChat or enters play mode in Unity), rather than directly in the editor. This "non-destructive" workflow means that assets can be easily removed or updated without needing to manually undo complex changes.

**1.2. Primary Purpose and Target Audience**

The primary purpose of Modular Avatar is to simplify and streamline the VRChat avatar customization process. It makes adding features like new outfits or gimmicks as simple as dragging and dropping a prefab into the Unity hierarchy. The target audience includes:

*   **Avatar Creators:** Who can package their assets (clothing, accessories) to be easily installable by users.
*   **End-Users:** Who want to customize their avatars without needing deep technical knowledge of Unity or VRChat's animation systems.
*   **Developers:** Who can leverage Modular Avatar's underlying framework (NDMF) to create their own compatible tools.

**1.3. Core Features**

Modular Avatar's functionality is delivered through a series of components that can be added to GameObjects in Unity. The key features include:

*   **Armature Merging:** It can intelligently merge the bone structure (armature) of an outfit or accessory with the main avatar's armature. It minimizes the creation of new bones by reusing existing ones where possible.
*   **Animator Merging:** It allows for the merging of multiple animator controllers. This is crucial for adding new expressions, gestures, or toggles for clothing and objects without conflicting with the avatar's base animation controller.
*   **Non-Destructive Workflow:** All modifications are applied at runtime or build time, leaving the original avatar and accessory prefabs untouched. This makes it easy to remove items by simply deleting them from the hierarchy.
*   **Menu System Integration:** It provides tools to easily add new menus and submenus to the VRChat expression menu, allowing for in-game toggling of outfits and props.
*   **Blendshape Syncing:** It can synchronize blendshape values between different meshes, for example, to make an outfit's shape conform to the body's shape changes.
*   **Prefab-based Distribution:** Creators can set up their assets with Modular Avatar components, making installation for the end-user a simple drag-and-drop process.

**1.4. Installation**

The recommended way to install Modular Avatar is through the VRChat Creator Companion (VCC), which is the standard tool for managing VRChat projects. Users can add the Modular Avatar repository to the VCC to easily install and update the package. For pre-release versions, the documentation sometimes recommends alternative community-made package managers due to potential bugs in VCC's dependency resolution.

**1.5. Source Validation and Credibility Assessment**

The primary sources for information on Modular Avatar are its official documentation website and its GitHub repository. These are considered highly credible as they are maintained by the developer of the tool. The VRChat Ask Forum and various YouTube tutorials from experienced VRChat creators also provide valuable and practical insights, though the information may be specific to a particular version or use case. There is a lack of traditional academic or peer-reviewed sources, as Modular Avatar is a community-developed tool for a specific niche application. The information is highly practical and focused on implementation within the VRChat ecosystem. The documentation also exists in other languages, like Chinese, which points to a wide user base. There are also companion libraries available, like "Animator As Code," which extend Modular Avatar's functionality, further cementing its importance in the VRChat creator community.

--- Level 2 Analysis ---
### **Research Level 2: In-Depth Analysis of Modular Avatar**

Building upon the foundational understanding of Modular Avatar, this level delves deeper into its technical features, best practices, and its position within the broader VRChat avatar creation ecosystem.

#### **1. Comprehensive Analysis with Multiple Perspectives**

**1.1. Core Functionality: A Deeper Dive**

Modular Avatar's primary strength lies in its non-destructive workflow, a concept repeatedly emphasized in both official documentation and community guides. This means that instead of directly and permanently altering an avatar's base files, Modular Avatar applies modifications at runtime (when you enter play mode in Unity or upload to VRChat). This is a significant paradigm shift from older, "destructive" methods where merging an outfit or accessory was a one-way process, making it difficult to undo changes or manage multiple configurations.

The system is built on a series of components that you add to your avatar project in Unity. Key components include:

*   **MA Merge Armature:** This is the core of outfit and accessory integration. It intelligently merges the bone structure of a clothing item with the avatar's main armature, reusing existing bones where possible to minimize performance impact.
*   **Animator Merging:** Modular Avatar allows you to split your avatar's animation controller into smaller, more manageable parts. This is invaluable for organizing complex animations for toggles, gestures, and other features.
*   **Menu System:** The tool provides an intuitive way to edit and extend VRChat's expression menus, making it simple to add new toggles and options for your modular components.
*   **Blendshape Syncing:** This feature automatically syncs blendshape values between the avatar's body and clothing. For example, if you have a "breast size" blendshape on your avatar, Modular Avatar can ensure that a shirt's blendshapes adjust accordingly.
*   **Reactive Components:** This is a more advanced feature that allows for the creation of dynamic interactions and toggles with minimal manual animation work. For instance, you can set up a system where equipping one item automatically unequips another.

**1.2. User Perspectives**

*   **For the Beginner:** The "drag-and-drop" nature of Modular Avatar significantly lowers the barrier to entry for avatar customization. The "Setup Outfit" feature, for example, automates much of the initial configuration process. Numerous YouTube tutorials cater to this audience, walking them through the basics of adding clothing, creating toggles, and managing menus.

*   **For the Advanced User/Creator:** Modular Avatar offers a high degree of control and flexibility. Advanced users can manually configure components, create complex animator setups, and leverage reactive components for sophisticated avatar features. The ability to distribute assets as self-contained prefabs with Modular Avatar components already attached is a major boon for creators selling clothing and accessories.

*   **The Developer Perspective:** The underlying framework, NDMF (Non-Destructive Modular Framework), is designed to be extensible, allowing other developers to create their own tools that integrate with Modular Avatar. This fosters a collaborative and innovative ecosystem around avatar creation.

#### **2. Source Validation and Credibility Assessment**

The information gathered comes from a variety of sources, each with its own level of credibility:

*   **Official Documentation (High Credibility):** The official Modular Avatar website and GitHub repository are the most reliable sources for information on the tool's features and installation. The documentation is actively maintained by the developer.
*   **Community Guides and Tutorials (Medium-High Credibility):** Tutorials on platforms like YouTube and forums like the VRChat Ask Forum are generally reliable, especially when they come from experienced creators. However, it's important to be mindful of the date of the content, as Modular Avatar is under active development and older tutorials may not reflect the latest features or best practices.
*   **Forum Discussions (Medium Credibility):** Reddit and other forums can be valuable for gauging community sentiment, discovering niche tips, and understanding common issues. However, the information can be anecdotal and may not always be accurate.
*   **Academic and Peer-Reviewed Sources (Limited Direct Relevance):** While there is a growing body of academic research on avatar customization and its psychological effects, these studies do not directly address Modular Avatar as a specific tool. Their value lies in providing a broader context for the importance of avatar customization in virtual environments.

#### **3. Related Topics and Connections**

*   **Avatar Optimization:** Modular Avatar is often used in conjunction with optimization tools like **Avatar Optimizer**. While Modular Avatar simplifies the process of adding components, it's still crucial to be mindful of performance metrics like polygon count, material slots, and texture memory to ensure your avatar performs well in VRChat.
*   **VRCFury:** This is another popular non-destructive avatar creation tool. While both tools share a similar philosophy, they have different approaches. VRCFury is often described as an "all-in-one" solution, while Modular Avatar is more of a foundational framework that can be extended with other tools.
*   **Blender and 3D Modeling:** While Modular Avatar simplifies the process within Unity, a foundational understanding of 3D modeling concepts and software like Blender is still highly beneficial, especially for tasks like weight painting, UV mapping, and creating custom assets.
*   **Generative AI:** The rise of generative AI tools for 3D model and texture creation presents an interesting future for avatar creation. These tools could potentially automate even more of the asset creation process, allowing users to focus on the creative aspects of avatar design.

#### **4. Current Trends and Future Implications**

*   **The Shift to Non-Destructive Workflows:** The popularity of tools like Modular Avatar and VRCFury indicates a clear trend towards non-destructive workflows in VRChat avatar creation. This approach is more flexible, easier to manage, and less prone to errors than older, destructive methods.
*   **Increased Accessibility:** By lowering the technical barriers to entry, Modular Avatar is empowering a wider range of users to customize their avatars. This is leading to a more diverse and creative avatar ecosystem in VRChat.
*   **Focus on Performance:** As avatars become more complex, there is a growing emphasis on performance optimization. Tools that can help users create performant avatars without sacrificing visual quality will become increasingly important.
*   **The Future of Avatar Identity:** The ongoing evolution of avatar creation tools is intertwined with broader questions of digital identity. As it becomes easier to create and customize avatars, users will have even more opportunities to express themselves and explore different facets of their identity in virtual worlds.

#### **5. Contradicting Viewpoints and Debates**

*   **Modular Avatar vs. VRCFury:** There is an ongoing debate in the community about which tool is "better." The consensus seems to be that both are excellent tools with different strengths. VRCFury is often recommended for beginners due to its all-in-one nature, while Modular Avatar is favored by those who want more control and extensibility.
*   **The "Best" Way to Create Toggles:** While Modular Avatar's built-in toggle creation system is powerful, some users still prefer to create their animations and menus manually for maximum control. The "best" approach often comes down to personal preference and the specific needs of the project.
*   **The Role of Manual Work:** While tools like Modular Avatar automate many tasks, there is still a significant amount of manual work involved in creating a high-quality avatar. Skills in 3D modeling, texturing, and animation remain highly valuable.

#### **6. Practical Applications and Recommendations**

*   **For New Users:**
    *   Start with the official Modular Avatar documentation and tutorials.
    *   Watch beginner-friendly YouTube tutorials to get a visual understanding of the workflow.
    *   Begin with simple projects, like adding a single piece of clothing or creating a basic toggle.
    *   Don't be afraid to experiment, as the non-destructive nature of Modular Avatar makes it easy to undo mistakes.

*   **For Content Creators:**
    *   Structure your assets as prefabs with Modular Avatar components pre-configured for easy installation by your customers.
    *   Provide clear documentation and instructions on how to use your assets with Modular Avatar.
    *   Consider using Modular Avatar's reactive components to create more dynamic and interactive clothing and accessories.

*   **Best Practices:**
    *   **Stay Updated:** Modular Avatar is in active development, so make sure you are using the latest version.
    *   **Keep it Organized:** Use empty GameObjects to group your modular components and keep your hierarchy clean.
    *   **Optimize, Optimize, Optimize:** Always be mindful of performance. Use tools like Avatar Optimizer and follow VRChat's optimization guidelines.
    *   **Test Thoroughly:** Use a tool like Gesture Manager to test your avatar's expressions and toggles in Unity before uploading to VRChat.
    *   **Backup Your Work:** While Modular Avatar is non-destructive, it's always a good practice to back up your Unity project regularly.
    *   **Be Aware of SDK Updates:** VRChat SDK updates can sometimes introduce issues with tools like Modular Avatar. It's a good idea to wait a bit before updating to a new SDK version and to check for any reported compatibility problems.

---

## 日本語レポート



---

*Report generated by DeepResearch tool on 2025-08-01*