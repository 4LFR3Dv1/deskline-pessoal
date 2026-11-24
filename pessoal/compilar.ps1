$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot
$motor = Join-Path $PSScriptRoot 'windows\deskline.exe'
$hash = 'c0cb517ace3f66943b252c085e815cfc64e5f4c8d5478e4b67247a1305467376'
if (-not (Test-Path -LiteralPath $motor)) {
    New-Item -ItemType Directory -Force (Split-Path $motor) | Out-Null
    Invoke-WebRequest -UseBasicParsing 'https://github.com/luannzin/deskline/releases/download/v0.9.0/deskline.exe' -OutFile $motor
}
if ((Get-FileHash -LiteralPath $motor).Hash -ne $hash) {
    throw 'O SHA-256 do motor difere do publicado para a versao 0.9.0.'
}
$dist = Join-Path $raiz 'dist'
New-Item -ItemType Directory -Force $dist | Out-Null
$saida = Join-Path $dist 'DesklinePessoal.exe'
$icone = Join-Path $raiz 'assets\icon.ico'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $csc /nologo /target:winexe /platform:x64 /optimize+ /codepage:65001 "/out:$saida" "/win32icon:$icone" "/resource:$motor,deskline.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll (Join-Path $PSScriptRoot 'Pessoal.cs')
if ($LASTEXITCODE -ne 0) { throw 'O compilador falhou.' }
Get-Item -LiteralPath $saida
