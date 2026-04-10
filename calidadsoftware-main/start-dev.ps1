# DoctorPC - Script de inicio en PowerShell con colores
param()

function Show-Color {
    param(
        [string]$Text,
        [string]$Color = 'White'
    )
    Write-Host $Text -ForegroundColor $Color
}

Clear-Host
Show-Color '=================================================================' Blue
Show-Color ' Iniciando Entorno de Desarrollo DoctorPC' Blue
Show-Color '=================================================================' Blue
Write-Host

# --- PASO 1: VALIDACION DE DEPENDENCIAS ---
Show-Color '[*] Verificando dependencias necesarias...' Cyan
$validationFailed = $false
$dotnetVersionRequired = 8

# Validar Ngrok
if (-not (Get-Command ngrok -ErrorAction SilentlyContinue)) {
    Show-Color "[X] ERROR: 'ngrok' no se encuentra. Puedes instalarlo ejecutando:" Red
    Show-Color      'winget install --id Ngrok.Ngrok && ngrok update' Yellow
    Write-Host
    $validationFailed = $true
} else {
    Show-Color '[✓] ngrok encontrado.' Green
}

# Validar NPM
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Show-Color "[X] ERROR: 'npm' no se encuentra. Puedes instalarlo ejecutando:" Red
    Show-Color '    winget install --id OpenJS.NodeJS.LTS -e' Yellow
    Write-Host
    $validationFailed = $true
} else {
    Show-Color '[✓] npm encontrado.' Green
}

# Validar .NET SDK
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Show-Color "[X] ERROR: 'dotnet' no se encuentra. Por favor, instala el SDK de .NET." Red
    $validationFailed = $true
} else {
    $sdks = & dotnet --list-sdks
    if (-not $sdks) {
        Show-Color "[X] ERROR: No se encontró ningún SDK de .NET instalado. Solo está presente el runtime o la instalación está dañada." Red
        Show-Color '[i] Para este proyecto se requiere .NET SDK versión 8. Puedes instalarlo ejecutando:' Cyan
        Show-Color '    winget install Microsoft.DotNet.SDK.8' Yellow
        Write-Host
        $validationFailed = $true
    } elseif (-not ($sdks -match "^$dotnetVersionRequired\.")) {
        Show-Color "[X] ERROR: No se encontró el SDK de .NET versión $dotnetVersionRequired." Red
        Show-Color '[i] SDKs de .NET encontrados en tu sistema:' Cyan
        $sdks | ForEach-Object { Write-Host $_ }
        Show-Color '[i] Para este proyecto se requiere .NET SDK versión 8. Puedes instalarlo ejecutando:' Cyan
        Show-Color '    winget install Microsoft.DotNet.SDK.8' Yellow
        Write-Host
        $validationFailed = $true
    } else {
        Show-Color "[✓] .NET SDK versión $dotnetVersionRequired encontrado." Green
    }
}

if ($validationFailed) {
    Write-Host
    Show-Color '[!] Faltan una o más dependencias. El script no puede continuar.' Red
    Show-Color '[!] Revisa los mensajes de error, instala lo necesario y vuelve a ejecutar el script.' Yellow
    Show-Color '[i] Si después de instalar alguna dependencia todavía hay error, reinicia la computadora.' Cyan
    Write-Host
    Show-Color 'Presiona cualquier tecla para salir...' Cyan
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
    exit 1
}

Write-Host
Show-Color '[OK] Todas las dependencias están en su lugar.' Green
Write-Host
Show-Color 'Presiona cualquier tecla para continuar...' Cyan
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
Clear-Host

# --- CERRAR PROCESOS PREVIOS DE NGROK ---
Show-Color '[*] Cerrando procesos previos de ngrok si existen...' Cyan
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# --- CONFIGURAR AUTHTOKEN DE NGROK SI FALTA ---
$ngrokConfig = "$env:LOCALAPPDATA\ngrok\ngrok.yml"
$authtoken = '2yw3X2qbnhryUc7P4Hra5e9Ozvv_5sp17JghXk3ZMzpNUca4T'
$needToken = $false
if (-not (Test-Path $ngrokConfig)) {
    $needToken = $true
} else {
    $content = Get-Content $ngrokConfig -Raw
    if ($content -notmatch $authtoken) { $needToken = $true }
}
if ($needToken) {
    Show-Color '[*] Configurando authtoken de Ngrok automáticamente...' Cyan
    ngrok config add-authtoken $authtoken
    Show-Color '[OK] Ngrok authtoken configurado.' Green
}

# --- PASO 2: INSTALAR DEPENDENCIAS DEL FRONTEND SI FALTAN ---
$frontendPath = Join-Path $PSScriptRoot 'frontend'
$vueCli = Join-Path $frontendPath 'node_modules\.bin\vue-cli-service.cmd'
if (-not (Test-Path $vueCli)) {
    Show-Color "[!] Dependencias de Vue no encontradas o incompletas. Ejecutando 'npm install' en la carpeta frontend..." Yellow
    Push-Location $frontendPath
    npm install
    Pop-Location
    Show-Color '[OK] Dependencias de Vue instaladas.' Green
}

# --- PREGUNTAR SI SE DESEA PROBAR FUNCIONALIDADES DE PAGO (NGROK) ---
$startNgrok = $false
Write-Host
$resp = Read-Host '¿Deseas probar funcionalidades de pago (requiere exponer backend con ngrok)? (y/n)'
if ($resp -eq 'y' -or $resp -eq 'Y') { $startNgrok = $true }

# --- PASO 3: INICIO DEL ENTORNO ---
Show-Color '[*] Abriendo los servicios en pestañas de Windows Terminal...' Cyan
Write-Host

$backendPath = Join-Path $PSScriptRoot 'EventosBackend'

# --- INICIO DE SERVICIOS EN WINDOWS TERMINAL (FIX ARGUMENTOS) ---
$tabs = @(
    "new-tab --title `"Frontend (Vue)`" -d `"$frontendPath`" cmd /k `"title Frontend (Vue) && npm run serve`"",
    "new-tab --title `"Backend (.NET)`" -d `"$backendPath`" cmd /k `"title Backend (.NET) && dotnet restore && dotnet clean && dotnet run --property WarningLevel=0`""
)
if ($startNgrok) {
    $tabs += "new-tab --title `"Ngrok Tunnel`" cmd /k `"title Ngrok Tunnel && ngrok http https://localhost:5001 --domain eminent-pure-stud.ngrok-free.app`""
}
$cmd = $tabs -join ' ; '
Start-Process wt.exe -ArgumentList $cmd

Write-Host
Show-Color '[✓] Entorno iniciado. Revisa la ventana de Windows Terminal que se ha abierto.' Green
