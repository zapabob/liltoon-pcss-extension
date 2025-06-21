# lilToon PCSS Extension - シンプルテストスイート
# All-in-One Edition v1.2.0

Write-Host "🚀 lilToon PCSS Extension - テストスイート開始" -ForegroundColor Cyan
Write-Host "All-in-One Edition v1.2.0" -ForegroundColor Blue
Write-Host "=" * 60 -ForegroundColor Blue

$startTime = Get-Date
$passedTests = 0
$failedTests = 0

# 1. ファイル構造チェック
Write-Host "`n📁 1. ファイル構造チェック" -ForegroundColor Cyan

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

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
        $passedTests++
    } else {
        Write-Host "❌ $file (Missing)" -ForegroundColor Red
        $failedTests++
    }
}

# 2. package.json 検証
Write-Host "`n📦 2. package.json 検証" -ForegroundColor Cyan

if (Test-Path "package.json") {
    try {
        $packageJson = Get-Content "package.json" | ConvertFrom-Json
        
        if ($packageJson.version -eq "1.2.0") {
            Write-Host "✅ バージョン 1.2.0 確認" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "❌ バージョン不正: $($packageJson.version)" -ForegroundColor Red
            $failedTests++
        }
        
        if ($packageJson.displayName -like "*All-in-One Edition*") {
            Write-Host "✅ All-in-One Edition 表記確認" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "❌ All-in-One Edition 表記なし" -ForegroundColor Red
            $failedTests++
        }
    } catch {
        Write-Host "❌ package.json 読み込みエラー" -ForegroundColor Red
        $failedTests++
    }
}

# 3. シェーダーファイルチェック
Write-Host "`n🎨 3. シェーダーファイルチェック" -ForegroundColor Cyan

$shaderFiles = @(
    "Shaders\lilToon_PCSS_Extension.shader",
    "Shaders\Poiyomi_PCSS_Extension.shader"
)

foreach ($shader in $shaderFiles) {
    if (Test-Path $shader) {
        $content = Get-Content $shader -Raw
        if ($content -like "*_PCSSEnabled*" -and $content -like "*_PCSSLightSize*") {
            Write-Host "✅ $shader PCSS プロパティ確認" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "❌ $shader PCSS プロパティ不足" -ForegroundColor Red
            $failedTests++
        }
    }
}

# 4. VCC統合チェック
Write-Host "`n🔗 4. VCC統合チェック" -ForegroundColor Cyan

if (Test-Path "Editor\VCCSetupWizard.cs") {
    $vccContent = Get-Content "Editor\VCCSetupWizard.cs" -Raw
    if ($vccContent -like "*All-in-One Edition*") {
        Write-Host "✅ VCC All-in-One Edition 対応" -ForegroundColor Green
        $passedTests++
    } else {
        Write-Host "❌ VCC All-in-One Edition 未対応" -ForegroundColor Red
        $failedTests++
    }
}

# 5. lilToon互換性チェック
Write-Host "`n🔧 5. lilToon互換性チェック" -ForegroundColor Cyan

if (Test-Path "Runtime\LilToonCompatibilityManager.cs") {
    $compatContent = Get-Content "Runtime\LilToonCompatibilityManager.cs" -Raw
    if ($compatContent -like "*DetectLilToonVersion*") {
        Write-Host "✅ lilToon互換性機能確認" -ForegroundColor Green
        $passedTests++
    } else {
        Write-Host "❌ lilToon互換性機能不足" -ForegroundColor Red
        $failedTests++
    }
}

# 6. ドキュメントチェック
Write-Host "`n📚 6. ドキュメントチェック" -ForegroundColor Cyan

$docFiles = @("README.md", "CHANGELOG.md")
foreach ($doc in $docFiles) {
    if (Test-Path $doc) {
        $content = Get-Content $doc -Raw
        if ($content.Length -gt 100) {
            Write-Host "✅ $doc 内容充実" -ForegroundColor Green
            $passedTests++
        } else {
            Write-Host "❌ $doc 内容不足" -ForegroundColor Red
            $failedTests++
        }
    }
}

# テスト完了
$endTime = Get-Date
$totalDuration = $endTime - $startTime
$totalTests = $passedTests + $failedTests

Write-Host "`n🏁 テスト完了" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Blue

Write-Host "`n📊 テスト結果サマリー" -ForegroundColor Cyan
Write-Host "総テスト数: $totalTests" -ForegroundColor Blue
Write-Host "成功: $passedTests" -ForegroundColor Green
Write-Host "失敗: $failedTests" -ForegroundColor Red

if ($totalTests -gt 0) {
    $successRate = [math]::Round(($passedTests/$totalTests)*100, 1)
    Write-Host "成功率: $successRate%" -ForegroundColor Blue
}

$durationSeconds = [math]::Round($totalDuration.TotalSeconds, 1)
Write-Host "実行時間: $durationSeconds 秒" -ForegroundColor Blue

# 結果判定
if ($failedTests -eq 0) {
    Write-Host "`n🎉 全テスト成功！リリース準備完了" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n⚠️ $failedTests 個のテストが失敗しました。修正が必要です。" -ForegroundColor Red
    exit 1
} 