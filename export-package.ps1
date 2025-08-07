# lilToon PCSS Extension パッケージエクスポートスクリプト
# なんｊ民の俺が作成したぜ！🔥

param(
    [string]$Version = "2.4.0",
    [string]$PackageName = "com.liltoon.pcss-extension",
    [string]$ExportType = "standard" # standard, clean, full, vpm
)

Write-Host "🎯 lilToon PCSS Extension パッケージエクスポート開始..." -ForegroundColor Green
Write-Host "📦 バージョン: $Version" -ForegroundColor Yellow
Write-Host "🏷️ パッケージ名: $PackageName" -ForegroundColor Yellow
Write-Host "📋 エクスポートタイプ: $ExportType" -ForegroundColor Yellow

# Unity Editorのパスを設定
$UnityPath = "C:\Program Files\Unity\Hub\Editor\2022.3.22f1\Editor\Unity.exe"

# プロジェクトパスを設定
$ProjectPath = "C:\Users\downl\Desktop\LilToon-PCSS-extension\liltoon-pcss-extension"

# エクスポートディレクトリを作成
$ExportDir = "..\ExportedPackages"
if (!(Test-Path $ExportDir)) {
    New-Item -ItemType Directory -Path $ExportDir -Force
    Write-Host "📁 エクスポートディレクトリを作成: $ExportDir" -ForegroundColor Cyan
}

# エクスポートタイプに応じてファイル名を設定
switch ($ExportType) {
    "standard" {
        $OutputFile = "$ExportDir\$PackageName-$Version.unitypackage"
        $MethodName = "lilToonPCSS.Editor.PackageExporter.ExportPackage"
    }
    "clean" {
        $OutputFile = "$ExportDir\$PackageName-$Version-clean.unitypackage"
        $MethodName = "lilToonPCSS.Editor.PackageExporter.ExportPackageClean"
    }
    "full" {
        $OutputFile = "$ExportDir\$PackageName-$Version-full.unitypackage"
        $MethodName = "lilToonPCSS.Editor.PackageExporter.ExportPackageFull"
    }
    "vpm" {
        $OutputFile = "$ExportDir\$PackageName-$Version-vpm.zip"
        $MethodName = "lilToonPCSS.Editor.PackageExporter.ExportPackageVPM"
    }
    default {
        Write-Host "❌ 無効なエクスポートタイプ: $ExportType" -ForegroundColor Red
        Write-Host "有効なタイプ: standard, clean, full, vpm" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "🚀 Unity Editorでパッケージエクスポートを実行中..." -ForegroundColor Green

# Unity Editorを起動してパッケージをエクスポート
$Arguments = @(
    "-projectPath", $ProjectPath,
    "-quit",
    "-batchmode",
    "-executeMethod", $MethodName,
    "-logFile", "$ExportDir\export-log.txt"
)

try {
    $Process = Start-Process -FilePath $UnityPath -ArgumentList $Arguments -Wait -PassThru -NoNewWindow
    
    if ($Process.ExitCode -eq 0) {
        Write-Host "✅ パッケージエクスポート完了！" -ForegroundColor Green
        Write-Host "📁 出力ファイル: $OutputFile" -ForegroundColor Cyan
        
        # ファイルサイズを表示
        if (Test-Path $OutputFile) {
            $FileSize = (Get-Item $OutputFile).Length
            $FileSizeMB = [math]::Round($FileSize / 1MB, 2)
            Write-Host "📊 ファイルサイズ: $FileSizeMB MB" -ForegroundColor Yellow
        }
        
        # エクスポートされたファイルを開く
        if (Test-Path $OutputFile) {
            Write-Host "📂 エクスポートされたファイルを開いています..." -ForegroundColor Cyan
            Start-Process "explorer.exe" -ArgumentList "/select,$OutputFile"
        }
    } else {
        Write-Host "❌ パッケージエクスポートに失敗しました" -ForegroundColor Red
        Write-Host "終了コード: $($Process.ExitCode)" -ForegroundColor Red
        
        # ログファイルを確認
        $LogFile = "$ExportDir\export-log.txt"
        if (Test-Path $LogFile) {
            Write-Host "📋 ログファイルの内容:" -ForegroundColor Yellow
            Get-Content $LogFile -Tail 20
        }
    }
} catch {
    Write-Host "❌ Unity Editorの起動に失敗しました" -ForegroundColor Red
    Write-Host "エラー: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Unity Editorのパスを確認してください: $UnityPath" -ForegroundColor Yellow
}

Write-Host "🎉 パッケージエクスポート処理完了！" -ForegroundColor Green 