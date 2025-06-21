# lilToon PCSS Extension Quick Test
Write-Host "lilToon PCSS Extension - Quick Test" -ForegroundColor Cyan

$passed = 0
$failed = 0

# File structure check
$files = @(
    "package.json",
    "README.md",
    "CHANGELOG.md",
    "Runtime\PCSSUtilities.cs",
    "Runtime\VRChatPerformanceOptimizer.cs",
    "Editor\VCCSetupWizard.cs",
    "Shaders\lilToon_PCSS_Extension.shader"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "OK: $file" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "MISSING: $file" -ForegroundColor Red
        $failed++
    }
}

# Package.json version check
if (Test-Path "package.json") {
    $json = Get-Content "package.json" | ConvertFrom-Json
    if ($json.version -eq "1.2.0") {
        Write-Host "OK: Version 1.2.0" -ForegroundColor Green
        $passed++
    } else {
        Write-Host "ERROR: Wrong version" -ForegroundColor Red
        $failed++
    }
}

# Results
$total = $passed + $failed
Write-Host "`nResults: $passed/$total passed" -ForegroundColor Blue

if ($failed -eq 0) {
    Write-Host "ALL TESTS PASSED!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "$failed tests failed" -ForegroundColor Red
    exit 1
} 