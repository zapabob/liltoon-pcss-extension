# lilToon PCSS Extension

Advanced soft-shadow and emissive-control tooling for lilToon, Poiyomi, Modular Avatar, and VRChat creator workflows.

[![GitHub Release](https://img.shields.io/github/v/release/zapabob/liltoon-pcss-extension?style=for-the-badge&logo=github)](https://github.com/zapabob/liltoon-pcss-extension/releases)
[![VPM](https://img.shields.io/badge/VPM-ready-27e1ff?style=for-the-badge&logo=vrchat)](https://zapabob.github.io/liltoon-pcss-extension/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)](LICENSE)

## Engineering Evidence Card

| Field | Current public evidence |
| --- | --- |
| Runtime surface | Unity C# runtime components for PCSS utilities, lilToon compatibility, Poiyomi integration, VRC Light Volumes integration, Modular Avatar control, and VRChat performance optimization |
| Package surface | VPM repository metadata, GitHub Pages distribution docs, release packaging scripts, and Unity package export helpers |
| Repro command | Add `https://zapabob.github.io/liltoon-pcss-extension/index.json` to VCC/VPM, install the package, then validate in a Unity avatar or world project |
| Metrics to inspect | Unity compile status, avatar performance rank, material backup/restore behavior, shader variant count, and VRChat SDK validation output |
| Safety/compatibility | Avoids destructive shared-material edits through MaterialPropertyBlock patterns and keeps editor-only logic outside runtime builds |
| Limitations | Final visual quality and runtime cost depend on avatar materials, shader variant selection, Unity version, VRChat SDK version, and target platform |

## What This Repository Demonstrates

- Unity editor/runtime engineering for creator tools.
- VRChat-focused compatibility work across lilToon, Poiyomi, Modular Avatar, VRC Light Volumes, and PhysBones.
- VPM/VCC packaging and GitHub Pages distribution.
- Defensive material-handling patterns for safer avatar workflows.
- Documentation-heavy release management through `_docs/`, `docs/`, and package metadata.

## Quick Install

Add this VPM repository in VRChat Creator Companion:

```text
https://zapabob.github.io/liltoon-pcss-extension/index.json
```

Or browse the published documentation:

- [GitHub Pages package docs](https://zapabob.github.io/liltoon-pcss-extension/)
- [VPM repository JSON](https://zapabob.github.io/liltoon-pcss-extension/index.json)
- [Releases](https://github.com/zapabob/liltoon-pcss-extension/releases)

## Repository Map

| Path | Purpose |
| --- | --- |
| `Runtime/` | Unity C# runtime components and integration helpers |
| `Editor/` | Unity editor tooling and package UI |
| `Shaders/` | Shader assets and shader documentation |
| `Assets/` | Unity asset layout and package-facing docs |
| `docs/` | GitHub Pages and VPM distribution docs |
| `_docs/` | Implementation and release logs |
| `vpm.json` | VPM repository metadata |
| `create-unitypackage.ps1` / `export-package.ps1` | Package export helpers |

## Review Notes for Portfolio Readers

This repository is not an AI model repository. It is included in the AI engineering portfolio as evidence of adjacent production engineering: Unity tool packaging, runtime safety constraints, creator-facing UX, compatibility management, and deployment documentation for a real interactive platform.

## License

MIT. See [LICENSE](LICENSE).
