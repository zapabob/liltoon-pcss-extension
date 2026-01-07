# Changelog

## [Unreleased] - 2025-12-30
### Added
- PCSS Hub (PC) to simplify setup and apply presets.
- Light presets (Anime / Realistic / Cinematic) for PCSS shadows and hip light rig.
- PhysBone-based light sway with optional MA slider.
- VRChat Light Volumes enable option in Hub.

### Changed
- Moved existing menus to Legacy paths (including GameObject/Assets context menus).
- MA toggle builder: shared parameters and sway slider support.

### Fixed
- PhysBoneEmissiveLightSetup: AdvancedBool + collider base list for latest SDK compile.
## [2.5.0] - 2025-08-08
### Added
- **nHaruka PCSSForVRC莠呈鋤繧ｷ繧ｹ繝・Β**: PhysBone蛻ｶ蠕｡繝ｩ繧､繝医す繧ｹ繝・Β縺ｮ螳悟・螳溯｣・
- **PhysBoneLightController**: 繝ｪ繧｢繝ｫ繧ｿ繧､繝蠖ｱ蛻ｶ蠕｡縲∬ｷ晞屬蛻ｶ蠕｡縲∬・蜍墓､懷・讖溯・
- **ModularAvatarPCSSSetupWizard**: 繝ｯ繝ｳ繧ｯ繝ｪ繝・け繧ｻ繝・ヨ繧｢繝・・繧ｦ繧｣繧ｶ繝ｼ繝・
- **UnifiedPCSSMenuSystem**: 邨ｱ蜷医Γ繝九Η繝ｼ繧ｷ繧ｹ繝・Β
- **繝ｪ繧｢繝ｫ繧ｿ繧､繝蠖ｱ蛻ｶ蠕｡**: 繧｢繝九Γ繝ｻ譏逕ｻ鬚ｨ縺ｮ鬮伜刀雉ｪ縺ｪ蠖ｱ陦ｨ迴ｾ
- **霍晞屬蛻ｶ蠕｡**: 繝励Ξ繧､繝､繝ｼ縺九ｉ縺ｮ霍晞屬縺ｫ蝓ｺ縺･縺上Λ繧､繝医・繝輔ぉ繝ｼ繝牙宛蠕｡
- **遶ｶ蜷郁｣ｽ蜩∽ｺ呈鋤諤ｧ**: 莉悶・PCSS繧ｷ繧ｹ繝・Β縺ｨ縺ｮ莠呈鋤諤ｧ
- **鬮伜ｺｦ縺ｪ繝励Μ繧ｻ繝・ヨ邂｡逅・*: 繧ｫ繧ｹ繧ｿ繝繝励Μ繧ｻ繝・ヨ縺ｮ菴懈・繝ｻ邂｡逅・

### Changed
- 繝舌・繧ｸ繝ｧ繝ｳ繧・.5.0縺ｫ譖ｴ譁ｰ
- package.json縺ｮ萓晏ｭ倬未菫ゅｒ譛譁ｰ迚医↓譖ｴ譁ｰ
- 螳溯｣・Ο繧ｰ縺ｮ閾ｪ蜍穂ｿ晏ｭ俶ｩ溯・繧貞ｼｷ蛹・

## [1.5.10] - 2025-07-04
### Changed
- PhysBoneEmissiveController險ｭ險域晄Φ繝ｻAutoFIX螳悟・蟇ｾ蠢懊・蜻ｽ蜷崎ｦ丞援蠕ｹ蠎輔・MaterialPropertyBlock豢ｻ逕ｨ繝ｻModularAvatar/PhysBone騾｣謳ｺ繝ｪ繝輔ぃ繧ｯ繧ｿ繝ｪ繝ｳ繧ｰ
- 螳溯｣・Ο繧ｰ縺ｫ蝓ｺ縺･縺丞宍蟇・↑繝ｪ繝輔ぃ繧ｯ繧ｿ繝ｪ繝ｳ繧ｰ

## [1.5.9] - 2025-07-03
### Changed
- PhysBoneLightController繧単hysBoneEmissiveController縺ｸ繝ｪ繝輔ぃ繧ｯ繧ｿ繝ｪ繝ｳ繧ｰ
- 繧ｯ繝ｩ繧ｹ蜷阪・繝輔ぃ繧､繝ｫ蜷阪・AddComponentMenu繝ｻ繧ｳ繝｡繝ｳ繝医ｒ荳諡ｬ菫ｮ豁｣
- 蜻ｽ蜷崎ｦ冗ｴ・・Unity繝舌Μ繝・・繧ｷ繝ｧ繝ｳ繝ｻVRChat AutoFIX螳悟・蟇ｾ蠢・
- 繧ｨ繝溘ャ繧ｷ繝門宛蠕｡逕ｨ騾斐↓迚ｹ蛹悶＠縺溯ｨｭ險医∈
- 繝槭ユ繝ｪ繧｢繝ｫ縺ｮ繝舌ャ繧ｯ繧｢繝・・繝ｻ繝ｪ繧ｹ繝医い縺隈UID繝吶・繧ｹ縺ｫ縺ｪ繧翫√ヱ繧ｹ螟画峩繧・Μ繝阪・繝蠕後ｂ豁｣遒ｺ縺ｫ蠕ｩ蜈・庄閭ｽ縲・
- 繝舌ャ繧ｯ繧｢繝・・縺ｯJSON蠖｢蠑上〒菫晏ｭ倥＆繧後∝ｾｩ蜈・凾縺ｯGUID荳閾ｴ縺ｧ繝槭ユ繝ｪ繧｢繝ｫ繧堤音螳壹・
- 繝・け繧ｹ繝√Ε繧・UID縺ｧ邂｡逅・＠縲∝盾辣ｧ蛻・ｌ繧貞､ｧ蟷・↓髦ｲ豁｢縲・

## [1.5.7] - 2025-06-24

### Added / Changed
- MissingMaterialAutoFixer・・ditor諡｡蠑ｵ・峨ｒ霑ｽ蜉: 繧ｷ繝ｼ繝ｳ蜀・・Missing繝槭ユ繝ｪ繧｢繝ｫ讀懷・・・・蜍穂ｿｮ蠕ｩ繧ｦ繧｣繝ｳ繝峨え
- FBX/Prefab繧､繝ｳ繝昴・繝域凾縺ｮ閾ｪ蜍輔Μ繝槭ャ繝玲ｩ溯・繧定ｿｽ蜉・・ssetPostprocessor縺ｫ繧医ｋMissing繧ｹ繝ｭ繝・ヨ閾ｪ蜍戊｣懷ｮ鯉ｼ・
- package.json縺ｮ繝舌・繧ｸ繝ｧ繝ｳ繧・.5.7縺ｫ譖ｴ譁ｰ

## [1.5.5] - 2025-06-24

### Added / Changed
- AutoFIX Prefab蟇ｾ蠢・ Prefab繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ縺ｮ繝槭ユ繝ｪ繧｢繝ｫ閾ｪ蜍穂ｿｮ蠕ｩ譎ゅ√せ繧ｯ繝ｪ繝励ヨ・・onoBehaviour遲会ｼ峨′辟｡蜉ｹ蛹悶＆繧後ｋ迴ｾ雎｡繧帝亟豁｢
- 繝槭ユ繝ｪ繧｢繝ｫ閾ｪ蜍穂ｿｮ蠕ｩ蠑ｷ蛹・ missing繧・ilToon邉ｻ繧ｷ繧ｧ繝ｼ繝繝ｼ繧り・蜍輔〒蠕ｩ譌ｧ繝ｻ陬懷ｮ・
- 繧ｷ繝ｼ繝ｳ/繝励Ο繧ｸ繧ｧ繧ｯ繝亥・菴薙・繝槭ユ繝ｪ繧｢繝ｫ繧剃ｸ諡ｬ菫ｮ蠕ｩ蜿ｯ閭ｽ
- ModularAvatar 1.12.5譛驕ｩ蛹・ MAMaterialSwap/MAPlatformFilter閾ｪ蜍戊ｿｽ蜉縲√・繝ｪ繧ｻ繝・ヨ驕ｩ逕ｨ縺ｮModularAvatar豬∝ｯｾ蠢懊＿uest/PC蛻・ｲ蝉ｾ九ｂ繧ｵ繝昴・繝・

## [1.5.4] - 2025-06-23

### Booth迚医Μ繝ｪ繝ｼ繧ｹ
- Booth蜷代￠繝代ャ繧ｱ繝ｼ繧ｸ縺ｨ縺励※v1.5.4繧偵Μ繝ｪ繝ｼ繧ｹ縲・
- 1.5.3縺ｮ蜈ｨ讖溯・繝ｻ菫ｮ豁｣繧貞性繧縲・
- Runtime/驟堺ｸ九・繧ｹ繧ｯ繝ｪ繝励ヨ繝ｻasmdef繧貞性繧√◆螳悟・迚医・

## [1.5.3] - 2025-06-23

### Booth迚医Μ繝ｪ繝ｼ繧ｹ
- Booth蜷代￠繝代ャ繧ｱ繝ｼ繧ｸ縺ｨ縺励※v1.5.3繧偵Μ繝ｪ繝ｼ繧ｹ縲・
- 1.5.3縺ｮ蜈ｨ讖溯・繝ｻ菫ｮ豁｣繧貞性繧縲・
- Runtime/驟堺ｸ九・繧ｹ繧ｯ繝ｪ繝励ヨ繝ｻasmdef繧貞性繧√◆螳悟・迚医・

## [1.5.8] - 2025-07-03
### Fixed
- PhysBoneLightController: 蜈郁｡瑚｣ｽ蜩∽ｺ呈鋤諤ｧ繝ｻ螳牙ｮ壽ｧ蜷台ｸ翫∝､夜Κ繝ｩ繧､繝域欠螳壹・閾ｪ蜍墓､懷・縺ｮ蝣・欧蛹悶∬ｷ晞屬貂幄｡ｰ繝ｻ繧ｹ繝繝ｼ繧ｸ繝ｳ繧ｰ蛻ｶ蠕｡縺ｮ譛驕ｩ蛹悶√お繝ｩ繝ｼ譎ゅ・閾ｪ蜍慕┌蜉ｹ蛹悶・繝・ヰ繝・げ蠑ｷ蛹・
- Unity 2022.3 LTS/VRChat SDK 譛譁ｰ迚医〒縺ｮ蜍穂ｽ懈､懆ｨｼ繝ｻ譛驕ｩ蛹・
