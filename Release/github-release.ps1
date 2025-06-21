# GitHub Release Creation Script
# Run this after uploading the ZIP file to GitHub Releases

$Version = "1.2.0"
$ZipFile = "com.liltoon.pcss-extension-$Version.zip"
$ReleaseTitle = "lilToon PCSS Extension v$Version"

Write-Host "噫 GitHub Release Information" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Green
Write-Host "Title: $ReleaseTitle" -ForegroundColor Green
Write-Host "ZIP File: $ZipFile" -ForegroundColor Green
Write-Host "SHA256: 5c29502d019ad50c63beb9a9f83f955824db7173903f53da207fa0df5c3e0cfa" -ForegroundColor Green

Write-Host ""
Write-Host "搭 Release Creation Steps:" -ForegroundColor Yellow
Write-Host "1. Go to: https://github.com/zapabob/liltoon-pcss-extension/releases/new"
Write-Host "2. Tag: v$Version"
Write-Host "3. Title: $ReleaseTitle"
Write-Host "4. Upload: $ZipFile"
Write-Host "5. Copy release notes from RELEASE_NOTES.md"
Write-Host "6. Mark as latest release"
Write-Host "7. Publish release"

Write-Host ""
Write-Host "迫 VPM Repository URL:"
Write-Host "https://github.com/zapabob/liltoon-pcss-extension/releases/download/v$Version/$ZipFile"
