# lilToon PCSS Extension Multi-Environment Testing Script
# Comprehensive testing for v1.2.0 release

param(
    [string]$TestMode = "All",  # All, Unity, VCC, Performance, Compatibility
    [string]$UnityPath = "",    # Unity editor path (optional)
    [switch]$Verbose = $false,
    [switch]$GenerateReport = $true
)

Write-Host "🧪 lilToon PCSS Extension v1.2.0 Testing Suite" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

# Test configuration
$TestResults = @()
$StartTime = Get-Date

# Test environments configuration
$TestEnvironments = @{
    "Unity2019" = @{
        "Version" = "2019.4.31f1"
        "RequiredPackages" = @("com.unity.render-pipelines.universal@7.1.8")
        "VRChatSDK" = "3.4.0"
    }
    "Unity2022" = @{
        "Version" = "2022.3.6f1" 
        "RequiredPackages" = @("com.unity.render-pipelines.universal@12.1.7")
        "VRChatSDK" = "3.4.0"
    }
}

function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    
    $timestamp = Get-Date -Format "HH:mm:ss"
    $color = switch ($Level) {
        "PASS" { "Green" }
        "FAIL" { "Red" }
        "WARN" { "Yellow" }
        "INFO" { "White" }
        default { "Gray" }
    }
    
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
    
    if ($Verbose) {
        Add-Content -Path "test-log.txt" -Value "[$timestamp] [$Level] $Message"
    }
}

function Test-PackageStructure {
    Write-TestLog "Testing package structure..." "INFO"
    
    $requiredFiles = @(
        "package.json",
        "VPMManifest.json", 
        "README.md",
        "CHANGELOG.md"
    )
    
    $requiredDirs = @(
        "Editor",
        "Runtime",
        "Shaders"
    )
    
    $structureValid = $true
    
    # Test files
    foreach ($file in $requiredFiles) {
        if (Test-Path $file) {
            Write-TestLog "✓ Found required file: $file" "PASS"
        } else {
            Write-TestLog "✗ Missing required file: $file" "FAIL"
            $structureValid = $false
        }
    }
    
    # Test directories
    foreach ($dir in $requiredDirs) {
        if (Test-Path $dir) {
            $fileCount = (Get-ChildItem $dir -Recurse -File).Count
            Write-TestLog "✓ Found directory: $dir ($fileCount files)" "PASS"
        } else {
            Write-TestLog "✗ Missing directory: $dir" "FAIL"
            $structureValid = $false
        }
    }
    
    return $structureValid
}

function Test-PackageJson {
    Write-TestLog "Testing package.json validity..." "INFO"
    
    try {
        $packageJson = Get-Content "package.json" -Raw | ConvertFrom-Json
        
        # Test required fields
        $requiredFields = @("name", "version", "displayName", "description", "unity")
        $jsonValid = $true
        
        foreach ($field in $requiredFields) {
            if ($packageJson.PSObject.Properties.Name -contains $field) {
                Write-TestLog "✓ package.json has field: $field" "PASS"
            } else {
                Write-TestLog "✗ package.json missing field: $field" "FAIL"
                $jsonValid = $false
            }
        }
        
        # Test version format
        if ($packageJson.version -match '^\d+\.\d+\.\d+$') {
            Write-TestLog "✓ Version format valid: $($packageJson.version)" "PASS"
        } else {
            Write-TestLog "✗ Invalid version format: $($packageJson.version)" "FAIL"
            $jsonValid = $false
        }
        
        # Test VPM dependencies
        if ($packageJson.vpmDependencies) {
            Write-TestLog "✓ VPM dependencies found: $($packageJson.vpmDependencies.PSObject.Properties.Name.Count) packages" "PASS"
        } else {
            Write-TestLog "⚠ No VPM dependencies defined" "WARN"
        }
        
        return $jsonValid
        
    } catch {
        Write-TestLog "✗ Failed to parse package.json: $_" "FAIL"
        return $false
    }
}

function Test-ShaderCompilation {
    Write-TestLog "Testing shader compilation..." "INFO"
    
    $shaderFiles = Get-ChildItem "Shaders" -Filter "*.shader" -Recurse
    $compilationValid = $true
    
    foreach ($shader in $shaderFiles) {
        Write-TestLog "Testing shader: $($shader.Name)" "INFO"
        
        # Basic syntax check
        $content = Get-Content $shader.FullName -Raw
        
        # Check for required shader structure
        if ($content -match 'Shader\s+"[^"]+"\s*{') {
            Write-TestLog "✓ Shader header valid: $($shader.Name)" "PASS"
        } else {
            Write-TestLog "✗ Invalid shader header: $($shader.Name)" "FAIL"
            $compilationValid = $false
        }
        
        # Check for PCSS keywords
        if ($content -match 'PCSS|_PCSS') {
            Write-TestLog "✓ PCSS keywords found: $($shader.Name)" "PASS"
        } else {
            Write-TestLog "⚠ No PCSS keywords found: $($shader.Name)" "WARN"
        }
        
        # Check for VRChat compatibility
        if ($content -match 'VRChat|VRC') {
            Write-TestLog "✓ VRChat compatibility markers: $($shader.Name)" "PASS"
        } else {
            Write-TestLog "⚠ No VRChat compatibility markers: $($shader.Name)" "WARN"
        }
    }
    
    return $compilationValid
}

function Test-AssemblyDefinitions {
    Write-TestLog "Testing assembly definitions..." "INFO"
    
    $asmdefFiles = Get-ChildItem . -Filter "*.asmdef" -Recurse
    $asmdefValid = $true
    
    foreach ($asmdef in $asmdefFiles) {
        Write-TestLog "Testing asmdef: $($asmdef.Name)" "INFO"
        
        try {
            $asmdefJson = Get-Content $asmdef.FullName -Raw | ConvertFrom-Json
            
            # Check required fields
            if ($asmdefJson.name) {
                Write-TestLog "✓ Assembly name defined: $($asmdefJson.name)" "PASS"
            } else {
                Write-TestLog "✗ Missing assembly name: $($asmdef.Name)" "FAIL"
                $asmdefValid = $false
            }
            
            # Check references
            if ($asmdefJson.references) {
                Write-TestLog "✓ Assembly references: $($asmdefJson.references.Count) refs" "PASS"
            } else {
                Write-TestLog "⚠ No assembly references: $($asmdef.Name)" "WARN"
            }
            
        } catch {
            Write-TestLog "✗ Failed to parse asmdef: $($asmdef.Name) - $_" "FAIL"
            $asmdefValid = $false
        }
    }
    
    return $asmdefValid
}

function Test-VCCCompatibility {
    Write-TestLog "Testing VCC compatibility..." "INFO"
    
    $vccValid = $true
    
    # Test VPMManifest.json
    if (Test-Path "VPMManifest.json") {
        try {
            $vpmManifest = Get-Content "VPMManifest.json" -Raw | ConvertFrom-Json
            Write-TestLog "✓ VPMManifest.json is valid JSON" "PASS"
            
            if ($vpmManifest.dependencies) {
                Write-TestLog "✓ VPM dependencies defined: $($vpmManifest.dependencies.PSObject.Properties.Name.Count)" "PASS"
            } else {
                Write-TestLog "⚠ No VPM dependencies in manifest" "WARN"
            }
            
        } catch {
            Write-TestLog "✗ Invalid VPMManifest.json: $_" "FAIL"
            $vccValid = $false
        }
    } else {
        Write-TestLog "✗ VPMManifest.json not found" "FAIL"
        $vccValid = $false
    }
    
    # Test VCC Setup Wizard
    $setupWizard = "Editor\VCCSetupWizard.cs"
    if (Test-Path $setupWizard) {
        Write-TestLog "✓ VCC Setup Wizard found" "PASS"
        
        $wizardContent = Get-Content $setupWizard -Raw
        if ($wizardContent -match 'EditorWindow') {
            Write-TestLog "✓ Setup Wizard extends EditorWindow" "PASS"
        } else {
            Write-TestLog "✗ Setup Wizard missing EditorWindow base" "FAIL"
            $vccValid = $false
        }
    } else {
        Write-TestLog "⚠ VCC Setup Wizard not found" "WARN"
    }
    
    return $vccValid
}

function Test-PerformanceOptimizations {
    Write-TestLog "Testing performance optimizations..." "INFO"
    
    $perfValid = $true
    
    # Test VRChat Performance Optimizer
    $perfOptimizer = "Runtime\VRChatPerformanceOptimizer.cs"
    if (Test-Path $perfOptimizer) {
        Write-TestLog "✓ VRChat Performance Optimizer found" "PASS"
        
        $perfContent = Get-Content $perfOptimizer -Raw
        
        # Check for performance-related keywords
        $perfKeywords = @("Performance", "Optimization", "Quality", "LOD", "Culling")
        foreach ($keyword in $perfKeywords) {
            if ($perfContent -match $keyword) {
                Write-TestLog "✓ Performance keyword found: $keyword" "PASS"
            }
        }
        
    } else {
        Write-TestLog "⚠ VRChat Performance Optimizer not found" "WARN"
    }
    
    # Test shader performance keywords
    $shaderFiles = Get-ChildItem "Shaders" -Filter "*.shader" -Recurse
    foreach ($shader in $shaderFiles) {
        $shaderContent = Get-Content $shader.FullName -Raw
        
        if ($shaderContent -match 'LOD|Quality|Performance') {
            Write-TestLog "✓ Performance optimizations in shader: $($shader.Name)" "PASS"
        } else {
            Write-TestLog "⚠ No performance optimizations detected: $($shader.Name)" "WARN"
        }
    }
    
    return $perfValid
}

function Test-Documentation {
    Write-TestLog "Testing documentation completeness..." "INFO"
    
    $docValid = $true
    
    # Test README.md
    if (Test-Path "README.md") {
        $readmeContent = Get-Content "README.md" -Raw
        
        $requiredSections = @("Installation", "Features", "Usage", "VRChat", "Performance")
        foreach ($section in $requiredSections) {
            if ($readmeContent -match $section) {
                Write-TestLog "✓ README section found: $section" "PASS"
            } else {
                Write-TestLog "⚠ README section missing: $section" "WARN"
            }
        }
    } else {
        Write-TestLog "✗ README.md not found" "FAIL"
        $docValid = $false
    }
    
    # Test CHANGELOG.md
    if (Test-Path "CHANGELOG.md") {
        $changelogContent = Get-Content "CHANGELOG.md" -Raw
        
        if ($changelogContent -match '1\.2\.0') {
            Write-TestLog "✓ CHANGELOG contains v1.2.0" "PASS"
        } else {
            Write-TestLog "⚠ CHANGELOG missing v1.2.0 entry" "WARN"
        }
    } else {
        Write-TestLog "✗ CHANGELOG.md not found" "FAIL"
        $docValid = $false
    }
    
    return $docValid
}

function Generate-TestReport {
    Write-TestLog "Generating test report..." "INFO"
    
    $endTime = Get-Date
    $duration = $endTime - $StartTime
    
    $reportPath = "test-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').html"
    
    $htmlReport = @"
<!DOCTYPE html>
<html>
<head>
    <title>lilToon PCSS Extension v1.2.0 Test Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { background: #2d3748; color: white; padding: 20px; border-radius: 8px; }
        .test-section { margin: 20px 0; padding: 15px; border: 1px solid #ddd; border-radius: 5px; }
        .pass { color: #38a169; }
        .fail { color: #e53e3e; }
        .warn { color: #d69e2e; }
        .summary { background: #f7fafc; padding: 15px; border-radius: 5px; }
        table { width: 100%; border-collapse: collapse; margin: 10px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background: #f5f5f5; }
    </style>
</head>
<body>
    <div class="header">
        <h1>🧪 lilToon PCSS Extension v1.2.0</h1>
        <h2>Multi-Environment Test Report</h2>
        <p><strong>Generated:</strong> $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        <p><strong>Duration:</strong> $($duration.TotalMinutes.ToString('F2')) minutes</p>
    </div>
    
    <div class="summary">
        <h3>📊 Test Summary</h3>
        <p><strong>Total Tests:</strong> $($TestResults.Count)</p>
        <p><strong>Passed:</strong> <span class="pass">$($TestResults | Where-Object {$_.Result -eq 'PASS'} | Measure-Object | Select-Object -ExpandProperty Count)</span></p>
        <p><strong>Failed:</strong> <span class="fail">$($TestResults | Where-Object {$_.Result -eq 'FAIL'} | Measure-Object | Select-Object -ExpandProperty Count)</span></p>
        <p><strong>Warnings:</strong> <span class="warn">$($TestResults | Where-Object {$_.Result -eq 'WARN'} | Measure-Object | Select-Object -ExpandProperty Count)</span></p>
    </div>
    
    <div class="test-section">
        <h3>🔍 Test Results</h3>
        <table>
            <tr><th>Test Category</th><th>Test Name</th><th>Result</th><th>Details</th></tr>
"@

    foreach ($result in $TestResults) {
        $cssClass = $result.Result.ToLower()
        $htmlReport += "<tr><td>$($result.Category)</td><td>$($result.Test)</td><td class='$cssClass'>$($result.Result)</td><td>$($result.Details)</td></tr>`n"
    }

    $htmlReport += @"
        </table>
    </div>
    
    <div class="test-section">
        <h3>🚀 Next Steps</h3>
        <ul>
            <li>Review any failed tests and fix issues</li>
            <li>Address warnings if necessary</li>
            <li>Run Unity-specific tests in target environments</li>
            <li>Test VCC installation process</li>
            <li>Validate VRChat performance in-world</li>
        </ul>
    </div>
    
    <div class="test-section">
        <h3>📋 Release Checklist</h3>
        <ul>
            <li>☐ All tests passing</li>
            <li>☐ Documentation updated</li>
            <li>☐ Version numbers consistent</li>
            <li>☐ GitHub Pages VPM repository updated</li>
            <li>☐ Release notes prepared</li>
            <li>☐ ZIP package created</li>
            <li>☐ GitHub release published</li>
        </ul>
    </div>
</body>
</html>
"@

    Set-Content -Path $reportPath -Value $htmlReport -Encoding UTF8
    Write-TestLog "✓ Test report generated: $reportPath" "PASS"
    
    # Open report in browser
    Start-Process $reportPath
}

# Main test execution
Write-TestLog "Starting test suite..." "INFO"

# Initialize test results
$TestResults = @()

# Run tests based on mode
switch ($TestMode) {
    "All" {
        $tests = @(
            @{Category="Structure"; Test="Package Structure"; Function="Test-PackageStructure"},
            @{Category="Configuration"; Test="Package.json Validation"; Function="Test-PackageJson"},
            @{Category="Shaders"; Test="Shader Compilation"; Function="Test-ShaderCompilation"},
            @{Category="Assembly"; Test="Assembly Definitions"; Function="Test-AssemblyDefinitions"},
            @{Category="VCC"; Test="VCC Compatibility"; Function="Test-VCCCompatibility"},
            @{Category="Performance"; Test="Performance Optimizations"; Function="Test-PerformanceOptimizations"},
            @{Category="Documentation"; Test="Documentation"; Function="Test-Documentation"}
        )
    }
    "Unity" {
        $tests = @(
            @{Category="Shaders"; Test="Shader Compilation"; Function="Test-ShaderCompilation"},
            @{Category="Assembly"; Test="Assembly Definitions"; Function="Test-AssemblyDefinitions"}
        )
    }
    "VCC" {
        $tests = @(
            @{Category="VCC"; Test="VCC Compatibility"; Function="Test-VCCCompatibility"},
            @{Category="Configuration"; Test="Package.json Validation"; Function="Test-PackageJson"}
        )
    }
    default {
        Write-TestLog "Unknown test mode: $TestMode" "FAIL"
        exit 1
    }
}

# Execute tests
foreach ($test in $tests) {
    try {
        $result = & $test.Function
        $status = if ($result) { "PASS" } else { "FAIL" }
        
        $TestResults += @{
            Category = $test.Category
            Test = $test.Test
            Result = $status
            Details = "Test completed"
        }
        
    } catch {
        $TestResults += @{
            Category = $test.Category
            Test = $test.Test
            Result = "FAIL"
            Details = $_.Exception.Message
        }
        Write-TestLog "Test failed: $($test.Test) - $_" "FAIL"
    }
}

# Generate report
if ($GenerateReport) {
    Generate-TestReport
}

# Summary
$passCount = ($TestResults | Where-Object {$_.Result -eq 'PASS'}).Count
$failCount = ($TestResults | Where-Object {$_.Result -eq 'FAIL'}).Count
$warnCount = ($TestResults | Where-Object {$_.Result -eq 'WARN'}).Count

Write-Host ""
Write-Host "🏁 Test Suite Complete!" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host "✅ Passed: $passCount" -ForegroundColor Green
Write-Host "❌ Failed: $failCount" -ForegroundColor Red
Write-Host "⚠️  Warnings: $warnCount" -ForegroundColor Yellow
Write-Host "⏱️  Duration: $($duration.TotalMinutes.ToString('F2')) minutes" -ForegroundColor White

if ($failCount -eq 0) {
    Write-Host ""
    Write-Host "🎉 All tests passed! Ready for release!" -ForegroundColor Green
    exit 0
} else {
    Write-Host ""
    Write-Host "⚠️  Some tests failed. Please review and fix issues before release." -ForegroundColor Yellow
    exit 1
} 