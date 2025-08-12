# Material Recovery System Samples

This sample demonstrates the advanced missing material auto-fixer system for VRChat avatar development.

## Features Demonstrated

### Advanced Material Backup System
- Complete material property backup including shader properties, textures, and material settings
- JSON-based backup format with structured data
- GUID-based asset tracking for reliable material reference management
- Property serialization for colors, vectors, floats, and textures

### Intelligent Restore System
- Smart restoration from backup with property validation
- Path-based material mapping using GameObject paths
- Fallback mechanisms for automatic material creation
- Silent restore mode for background operations

### VRChat Avatar Integration
- Automatic detection and handling of VRChat avatars
- Component scanning for comprehensive renderer analysis
- Material slot validation and missing material detection
- Full Unity Undo system integration

### User Interface Features
- Modern UI design with emoji icons and clear sections
- Real-time statistics display
- Backup information and file management
- Scrollable interface for large projects

## Usage Examples

### Basic Material Recovery
1. Open the Missing Material AutoFixer window (Tools > lilToon PCSS > Missing Material AutoFixer)
2. Click "Scan for Missing Materials" to detect issues
3. Click "Auto Fix Missing Materials" to resolve problems
4. Use "Create Backup Before Fix" for safety

### Advanced Backup Management
1. Create backups before making changes
2. Restore from backups when needed
3. Monitor backup information in the UI
4. Use silent restore for background operations

### Auto-Import Material Remapping
- Automatic material assignment during FBX/Prefab import
- Seamless integration with Unity's import pipeline
- Batch processing for multiple materials

## Technical Details

### Backup Format
The system uses a structured JSON format for material backups:
```json
{
  "avatarName": "MyAvatar",
  "backupTime": "2025-01-24 10:30:00",
  "version": "2.3.0",
  "materials": [
    {
      "materialName": "MyMaterial",
      "materialGUID": "guid_here",
      "shaderName": "lilToon",
      "properties": {...},
      "textureGUIDs": {...}
    }
  ]
}
```

### Error Handling
- Comprehensive try-catch blocks for all operations
- Asset validation and verification
- Graceful handling of backup corruption
- Clear user feedback and error messages

### Performance Optimizations
- Efficient asset loading with minimal memory footprint
- Batch operations to reduce Unity API calls
- Optimized loops and cached references
- Proper garbage collection and disposal

## Integration with VCC

This system is fully compatible with VRChat Creator Companion (VCC) and follows semantic versioning principles for reliable package management.

## Support

For issues and questions, please refer to the main package documentation or create an issue in the repository. 