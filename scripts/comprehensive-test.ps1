# lilToon PCSS Extension - 包括的テストスイート
# All-in-One Edition v1.2.0

param(
    [string]$UnityPath = "",
    [string]$ProjectPath = "",
    [switch]$GenerateReport = $true,
    [string]$OutputPath = "test-results"
)

$ErrorActionPreference = "Continue"

# カラー出力関数
function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    
    $colors = @{
        "Red" = [ConsoleColor]::Red
        "Green" = [ConsoleColor]::Green
        "Yellow" = [ConsoleColor]::Yellow
        "Blue" = [ConsoleColor]::Blue
        "Cyan" = [ConsoleColor]::Cyan
        "White" = [ConsoleColor]::White
    }
    
    Write-Host $Message -ForegroundColor $colors[$Color]
}

# テスト結果を格納する配列
$TestResults = @()

function Add-TestResult {
    param([string]$Category, [string]$TestName, [string]$Status, [string]$Details = "")
    
    $TestResults += [PSCustomObject]@{
        Category = $Category
        TestName = $TestName
        Status = $Status
        Details = $Details
        Timestamp = Get-Date
    }
}

# メイン実行
Write-ColorOutput "🚀 lilToon PCSS Extension - 包括的テストスイート開始" "Cyan"
Write-ColorOutput "All-in-One Edition v1.2.0" "Blue"
Write-ColorOutput "=" * 60 "Blue"

$startTime = Get-Date

# プロジェクトパス設定
if (-not $ProjectPath) {
    $ProjectPath = Get-Location
}

Write-ColorOutput "プロジェクトパス: $ProjectPath" "Blue"

# 出力ディレクトリ作成
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# 1. ファイル構造チェック
Write-ColorOutput "`n📁 1. ファイル構造チェック" "Cyan"
$requiredFiles = @(
    "package.json",
    "README.md",
    "CHANGELOG.md",
    "Runtime\lilToon.PCSS.Runtime.asmdef",
    "Runtime\LilToonCompatibilityManager.cs",
    "Runtime\PCSSUtilities.cs",
    "Runtime\VRChatPerformanceOptimizer.cs",
    "Runtime\VRCLightVolumesIntegration.cs",
    "Editor\lilToon.PCSS.Editor.asmdef",
    "Editor\VCCSetupWizard.cs",
    "Editor\LilToonPCSSShaderGUI.cs",
    "Editor\VRChatExpressionMenuCreator.cs",
    "Shaders\lilToon_PCSS_Extension.shader",
    "Shaders\Poiyomi_PCSS_Extension.shader"
)

$missingFiles = @()
foreach ($file in $requiredFiles) {
    $fullPath = Join-Path $ProjectPath $file
    if (Test-Path $fullPath) {
        Write-ColorOutput "✅ $file" "Green"
        Add-TestResult "FileStructure" $file "Passed"
    } else {
        Write-ColorOutput "❌ $file (Missing)" "Red"
        $missingFiles += $file
        Add-TestResult "FileStructure" $file "Failed" "File missing"
    }
}

if ($missingFiles.Count -eq 0) {
    Write-ColorOutput "✅ ファイル構造チェック完了" "Green"
} else {
    Write-ColorOutput "❌ $($missingFiles.Count) 個のファイルが不足" "Red"
}

# 2. package.json 検証
Write-ColorOutput "`n📦 2. package.json 検証" "Cyan"
try {
    $packageJson = Get-Content "package.json" | ConvertFrom-Json
    
    # 必須フィールドチェック
    $requiredFields = @("name", "displayName", "version", "description")
    foreach ($field in $requiredFields) {
        if ($packageJson.PSObject.Properties.Name -contains $field) {
            Write-ColorOutput "✅ $field フィールド存在" "Green"
            Add-TestResult "PackageJson" $field "Passed"
        } else {
            Write-ColorOutput "❌ $field フィールド不足" "Red"
            Add-TestResult "PackageJson" $field "Failed" "Required field missing"
        }
    }
    
    # バージョンチェック
    if ($packageJson.version -eq "1.2.0") {
        Write-ColorOutput "✅ バージョン 1.2.0 確認" "Green"
        Add-TestResult "PackageJson" "Version Check" "Passed"
    } else {
        Write-ColorOutput "❌ バージョン不正: $($packageJson.version)" "Red"
        Add-TestResult "PackageJson" "Version Check" "Failed" "Version mismatch"
    }
    
    # All-in-One Edition チェック
    if ($packageJson.displayName -like "*All-in-One Edition*") {
        Write-ColorOutput "✅ All-in-One Edition 表記確認" "Green"
        Add-TestResult "PackageJson" "All-in-One Edition" "Passed"
    } else {
        Write-ColorOutput "❌ All-in-One Edition 表記なし" "Red"
        Add-TestResult "PackageJson" "All-in-One Edition" "Failed" "Missing All-in-One branding"
    }
    
} catch {
    Write-ColorOutput "❌ package.json 読み込みエラー: $($_.Exception.Message)" "Red"
    Add-TestResult "PackageJson" "File Reading" "Failed" $_.Exception.Message
}

# 3. アセンブリ定義チェック
Write-ColorOutput "`n🔧 3. アセンブリ定義チェック" "Cyan"
$asmdefFiles = @(
    "Runtime\lilToon.PCSS.Runtime.asmdef",
    "Editor\lilToon.PCSS.Editor.asmdef"
)

foreach ($asmdef in $asmdefFiles) {
    $fullPath = Join-Path $ProjectPath $asmdef
    if (Test-Path $fullPath) {
        try {
            $asmdefContent = Get-Content $fullPath | ConvertFrom-Json
            
            if ($asmdefContent.name -and $asmdefContent.references) {
                Write-ColorOutput "✅ $asmdef 形式正常" "Green"
                Add-TestResult "AssemblyDefinition" $asmdef "Passed"
            } else {
                Write-ColorOutput "❌ $asmdef 必須フィールド不足" "Red"
                Add-TestResult "AssemblyDefinition" $asmdef "Failed" "Missing required fields"
            }
        } catch {
            Write-ColorOutput "❌ $asmdef 読み込みエラー" "Red"
            Add-TestResult "AssemblyDefinition" $asmdef "Failed" "JSON parse error"
        }
    }
}

# 4. シェーダーファイルチェック
Write-ColorOutput "`n🎨 4. シェーダーファイルチェック" "Cyan"
$shaderFiles = @(
    "Shaders\lilToon_PCSS_Extension.shader",
    "Shaders\Poiyomi_PCSS_Extension.shader"
)

foreach ($shader in $shaderFiles) {
    $fullPath = Join-Path $ProjectPath $shader
    if (Test-Path $fullPath) {
        $content = Get-Content $fullPath -Raw
        
        # PCSS関連プロパティチェック
        $pcssProperties = @("_PCSSEnabled", "_PCSSLightSize", "_PCSSFilterRadius")
        $foundProperties = 0
        
        foreach ($prop in $pcssProperties) {
            if ($content -like "*$prop*") {
                $foundProperties++
            }
        }
        
        if ($foundProperties -eq $pcssProperties.Count) {
            Write-ColorOutput "✅ $shader PCSS プロパティ完備" "Green"
            Add-TestResult "Shaders" $shader "Passed"
        } else {
            Write-ColorOutput "❌ $shader PCSS プロパティ不足 ($foundProperties/$($pcssProperties.Count))" "Red"
            Add-TestResult "Shaders" $shader "Failed" "Missing PCSS properties"
        }
    }
}

# 5. 統合テスト
Write-ColorOutput "`n🔗 5. 統合テスト" "Cyan"

# VCC統合テスト
Write-ColorOutput "VCC統合チェック..." "Yellow"
if (Test-Path "Editor\VCCSetupWizard.cs") {
    $vccContent = Get-Content "Editor\VCCSetupWizard.cs" -Raw
    if ($vccContent -like "*All-in-One Edition*") {
        Write-ColorOutput "✅ VCC All-in-One Edition 対応" "Green"
        Add-TestResult "Integration" "VCC All-in-One" "Passed"
    } else {
        Write-ColorOutput "❌ VCC All-in-One Edition 未対応" "Red"
        Add-TestResult "Integration" "VCC All-in-One" "Failed" "Missing All-in-One branding"
    }
}

# lilToon互換性テスト
Write-ColorOutput "lilToon互換性チェック..." "Yellow"
if (Test-Path "Runtime\LilToonCompatibilityManager.cs") {
    $compatContent = Get-Content "Runtime\LilToonCompatibilityManager.cs" -Raw
    if ($compatContent -like "*DetectLilToonVersion*" -and $compatContent -like "*SynchronizeMaterial*") {
        Write-ColorOutput "✅ lilToon互換性機能完備" "Green"
        Add-TestResult "Integration" "lilToon Compatibility" "Passed"
    } else {
        Write-ColorOutput "❌ lilToon互換性機能不足" "Red"
        Add-TestResult "Integration" "lilToon Compatibility" "Failed" "Missing compatibility features"
    }
}

# 6. パフォーマンステスト
Write-ColorOutput "`n⚡ 6. パフォーマンステスト" "Cyan"

# ファイルサイズチェック
$sizeLimit = 5MB
$totalSize = 0

Get-ChildItem -Path $ProjectPath -Recurse -File | ForEach-Object {
    if ($_.Extension -in @('.cs', '.shader', '.json', '.md')) {
        $totalSize += $_.Length
    }
}

if ($totalSize -lt $sizeLimit) {
    $sizeMB = [math]::Round($totalSize/1MB, 2)
    $limitMB = [math]::Round($sizeLimit/1MB, 2)
    Write-ColorOutput "✅ パッケージサイズ: ${sizeMB}MB (制限: ${limitMB}MB)" "Green"
    Add-TestResult "Performance" "Package Size" "Passed" "${sizeMB}MB"
} else {
    $sizeMB = [math]::Round($totalSize/1MB, 2)
    Write-ColorOutput "❌ パッケージサイズ超過: ${sizeMB}MB" "Red"
    Add-TestResult "Performance" "Package Size" "Failed" "Size limit exceeded"
}

# 7. ドキュメントチェック
Write-ColorOutput "`n📚 7. ドキュメントチェック" "Cyan"
$docFiles = @("README.md", "CHANGELOG.md")

foreach ($doc in $docFiles) {
    if (Test-Path $doc) {
        $content = Get-Content $doc -Raw
        if ($content.Length -gt 100) {
            Write-ColorOutput "✅ $doc 内容充実" "Green"
            Add-TestResult "Documentation" $doc "Passed"
        } else {
            Write-ColorOutput "❌ $doc 内容不足" "Red"
            Add-TestResult "Documentation" $doc "Failed" "Insufficient content"
        }
    } else {
        Write-ColorOutput "❌ $doc 不存在" "Red"
        Add-TestResult "Documentation" $doc "Failed" "File missing"
    }
}

# テスト完了
$endTime = Get-Date
$totalDuration = $endTime - $startTime

Write-ColorOutput "`n🏁 テスト完了" "Cyan"
Write-ColorOutput "=" * 60 "Blue"

# 結果サマリー
$passedTests = ($TestResults | Where-Object { $_.Status -eq "Passed" }).Count
$failedTests = ($TestResults | Where-Object { $_.Status -eq "Failed" }).Count
$totalTests = $TestResults.Count

Write-ColorOutput "`n📊 テスト結果サマリー" "Cyan"
Write-ColorOutput "総テスト数: $totalTests" "Blue"
Write-ColorOutput "成功: $passedTests" "Green"
Write-ColorOutput "失敗: $failedTests" "Red"

if ($totalTests -gt 0) {
    $successRate = [math]::Round(($passedTests/$totalTests)*100, 1)
    Write-ColorOutput "成功率: ${successRate}%" "Blue"
}

$durationMinutes = [math]::Round($totalDuration.TotalMinutes, 1)
Write-ColorOutput "実行時間: ${durationMinutes}分" "Blue"

# カテゴリ別結果
Write-ColorOutput "`n📋 カテゴリ別結果" "Cyan"
$TestResults | Group-Object Category | ForEach-Object {
    $categoryPassed = ($_.Group | Where-Object { $_.Status -eq "Passed" }).Count
    $categoryTotal = $_.Group.Count
    $categoryRate = [math]::Round(($categoryPassed/$categoryTotal)*100, 1)
    
    $color = if ($categoryRate -eq 100) { "Green" } elseif ($categoryRate -ge 80) { "Yellow" } else { "Red" }
    Write-ColorOutput "$($_.Name): $categoryPassed/$categoryTotal (${categoryRate}%)" $color
}

# HTMLレポート生成
if ($GenerateReport) {
    Write-ColorOutput "`n📄 HTMLレポート生成中..." "Yellow"
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $successRate = if ($totalTests -gt 0) { [math]::Round(($passedTests/$totalTests)*100, 1) } else { 0 }
    
    $htmlContent = "<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>lilToon PCSS Extension - Test Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { text-align: center; margin-bottom: 30px; }
        .summary { background: #f0f0f0; padding: 20px; margin-bottom: 20px; }
        .category { margin-bottom: 20px; }
        .category h3 { background: #e0e0e0; padding: 10px; margin: 0; }
        .test-item { padding: 5px 10px; border-bottom: 1px solid #ccc; }
        .passed { color: green; }
        .failed { color: red; }
    </style>
</head>
<body>
    <div class='header'>
        <h1>lilToon PCSS Extension Test Report</h1>
        <h2>All-in-One Edition v1.2.0</h2>
        <p>Generated: $timestamp</p>
    </div>
    
    <div class='summary'>
        <h3>Summary</h3>
        <p>Total Tests: $totalTests</p>
        <p>Passed: $passedTests</p>
        <p>Failed: $failedTests</p>
        <p>Success Rate: ${successRate}%</p>
    </div>
    
    <div class='results'>"

    # カテゴリ別詳細結果
    $TestResults | Group-Object Category | ForEach-Object {
        $categoryName = $_.Name
        $htmlContent += "<div class='category'><h3>$categoryName</h3>"
        
        $_.Group | ForEach-Object {
            $testName = $_.TestName
            $testStatus = $_.Status
            $statusClass = if ($_.Status -eq "Passed") { "passed" } else { "failed" }
            $htmlContent += "<div class='test-item'><span>$testName</span> - <span class='$statusClass'>$testStatus</span></div>"
        }
        
        $htmlContent += "</div>"
    }

    $htmlContent += "    </div>
</body>
</html>"

    $reportPath = Join-Path $OutputPath "test-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').html"
    $htmlContent | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-ColorOutput "✅ HTMLレポート生成完了: $reportPath" "Green"
}

# 終了ステータス
if ($failedTests -eq 0) {
    Write-ColorOutput "`n🎉 全テスト成功！リリース準備完了" "Green"
    exit 0
} else {
    Write-ColorOutput "`n⚠️ $failedTests 個のテストが失敗しました。修正が必要です。" "Red"
    exit 1
} 