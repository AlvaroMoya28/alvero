# Script de Cobertura Unificada - Frontend y Backend
# Genera reportes de cobertura para ambos proyectos

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     SISTEMA DE GESTIÓN DE EVENTOS - COBERTURA COMPLETA        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

$rootPath = $PSScriptRoot
$frontendPath = Join-Path $rootPath "frontend"
$backendTestPath = Join-Path $rootPath "EventosBackend.Tests"
$backendPath = Join-Path $rootPath "EventosBackend"

# ==============================================================================
# BACKEND - .NET Tests con Coverlet
# ==============================================================================

Write-Host "🔧 BACKEND (.NET Core / C#)" -ForegroundColor Magenta
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor DarkGray

Push-Location $backendTestPath

Write-Host "📦 Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore 2>&1 | Out-Null

Write-Host "🧪 Ejecutando tests del backend con cobertura..." -ForegroundColor Yellow

# Limpiar cobertura anterior
Remove-Item -Path "TestResults" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "coverage_backend" -Recurse -Force -ErrorAction SilentlyContinue

# Ejecutar tests con coverlet
$testOutput = dotnet test `
    /p:CollectCoverage=true `
    /p:CoverletOutput=coverage_backend/ `
    /p:CoverletOutputFormat=cobertura `
    /p:Exclude="[xunit.*]*%2c[*.Tests]*" `
    --verbosity quiet 2>&1 | Out-String

Write-Host $testOutput

# Buscar el archivo de cobertura
$coberturaFile = Get-ChildItem -Path . -Filter "coverage.cobertura.xml" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

if ($coberturaFile) {
    Write-Host "✓ Archivo de cobertura generado: $($coberturaFile.FullName)" -ForegroundColor Green
    
    # Generar reporte HTML con ReportGenerator
    Write-Host "📊 Generando reporte HTML del backend..." -ForegroundColor Yellow
    
    $reportPath = Join-Path $PSScriptRoot "coverage_backend_report"
    
    dotnet tool install --global dotnet-reportgenerator-globaltool --ignore-failed-sources 2>&1 | Out-Null
    
    reportgenerator `
        "-reports:$($coberturaFile.FullName)" `
        "-targetdir:$reportPath" `
        "-reporttypes:Html;TextSummary" 2>&1 | Out-Null
    
    if (Test-Path "$reportPath\index.html") {
        Write-Host "✓ Reporte HTML generado: $reportPath\index.html" -ForegroundColor Green
        
        # Leer resumen
        $summaryFile = Join-Path $reportPath "Summary.txt"
        if (Test-Path $summaryFile) {
            $summary = Get-Content $summaryFile -Raw
            
            # Extraer métricas
            if ($summary -match 'Line coverage:\s*([\d.]+)%') {
                $backendLineCoverage = [decimal]$matches[1]
            } else {
                $backendLineCoverage = 0
            }
            
            if ($summary -match 'Branch coverage:\s*([\d.]+)%') {
                $backendBranchCoverage = [decimal]$matches[1]
            } else {
                $backendBranchCoverage = 0
            }
            
            Write-Host "`n  📊 Line Coverage:   " -NoNewline
            Write-Host "$backendLineCoverage%" -ForegroundColor $(if($backendLineCoverage -ge 85){'Green'}elseif($backendLineCoverage -ge 70){'Yellow'}else{'Red'})
            Write-Host "  🔀 Branch Coverage: " -NoNewline
            Write-Host "$backendBranchCoverage%" -ForegroundColor $(if($backendBranchCoverage -ge 85){'Green'}elseif($backendBranchCoverage -ge 70){'Yellow'}else{'Red'})
        }
    }
} else {
    Write-Host "⚠ No se pudo generar el reporte de cobertura del backend" -ForegroundColor Red
    $backendLineCoverage = 0
    $backendBranchCoverage = 0
}

Pop-Location

# ==============================================================================
# FRONTEND - Jest Tests
# ==============================================================================

Write-Host "`n🎨 FRONTEND (Vue.js / Jest)" -ForegroundColor Magenta
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor DarkGray

Push-Location $frontendPath

Write-Host "🧪 Ejecutando tests del frontend con cobertura..." -ForegroundColor Yellow

# Limpiar cobertura anterior
Remove-Item -Path "coverage" -Recurse -Force -ErrorAction SilentlyContinue

# Ejecutar tests
$npmOutput = npm run test:coverage 2>&1 | Out-String

# Extraer solo el resumen de Jest
$lines = $npmOutput -split "`n"
$summaryStarted = $false
foreach ($line in $lines) {
    if ($line -match "Test Suites:") {
        $summaryStarted = $true
    }
    if ($summaryStarted) {
        Write-Host $line
    }
}

# Leer métricas de cobertura
if (Test-Path "coverage\lcov-report\index.html") {
    Write-Host "`n✓ Reporte de cobertura generado" -ForegroundColor Green
    
    # Buscar en el output del test
    if ($npmOutput -match 'All files\s+\|\s+([\d.]+)\s+\|\s+([\d.]+)\s+\|\s+([\d.]+)\s+\|\s+([\d.]+)') {
        $frontendStmtCoverage = [decimal]$matches[1]
        $frontendBranchCoverage = [decimal]$matches[2]
        $frontendFuncCoverage = [decimal]$matches[3]
        $frontendLineCoverage = [decimal]$matches[4]
        
        Write-Host "`n  📊 Statements:  " -NoNewline
        Write-Host "$frontendStmtCoverage%" -ForegroundColor $(if($frontendStmtCoverage -ge 85){'Green'}elseif($frontendStmtCoverage -ge 70){'Yellow'}else{'Red'})
        Write-Host "  🔀 Branches:    " -NoNewline
        Write-Host "$frontendBranchCoverage%" -ForegroundColor $(if($frontendBranchCoverage -ge 85){'Green'}elseif($frontendBranchCoverage -ge 70){'Yellow'}else{'Red'})
        Write-Host "  ⚡ Functions:   " -NoNewline
        Write-Host "$frontendFuncCoverage%" -ForegroundColor $(if($frontendFuncCoverage -ge 85){'Green'}elseif($frontendFuncCoverage -ge 70){'Yellow'}else{'Red'})
        Write-Host "  📝 Lines:       " -NoNewline
        Write-Host "$frontendLineCoverage%" -ForegroundColor $(if($frontendLineCoverage -ge 85){'Green'}elseif($frontendLineCoverage -ge 70){'Yellow'}else{'Red'})
    } else {
        $frontendStmtCoverage = 0
        $frontendLineCoverage = 0
        Write-Host "⚠ No se pudieron extraer métricas del frontend" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠ No se pudo generar el reporte del frontend" -ForegroundColor Red
    $frontendStmtCoverage = 0
    $frontendLineCoverage = 0
}

Pop-Location

# ==============================================================================
# RESUMEN UNIFICADO
# ==============================================================================

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    RESUMEN GENERAL                             ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

function Show-CoverageBar {
    param($label, $percent, $width = 30)
    $filled = [math]::Floor(($percent / 100) * $width)
    $empty = $width - $filled
    $color = if($percent -ge 85){'Green'}elseif($percent -ge 70){'Yellow'}else{'Red'}
    
    Write-Host "  $label " -NoNewline
    Write-Host ("[" + ("█" * $filled) + ("░" * $empty) + "]") -NoNewline -ForegroundColor $color
    Write-Host (" {0,6:N2}%" -f $percent) -ForegroundColor $color
}

Write-Host "  BACKEND (.NET)" -ForegroundColor Magenta
Show-CoverageBar "    Lines:    " $backendLineCoverage
Show-CoverageBar "    Branches: " $backendBranchCoverage

Write-Host "`n  FRONTEND (Vue.js)" -ForegroundColor Magenta
Show-CoverageBar "    Statements:" $frontendStmtCoverage
Show-CoverageBar "    Lines:    " $frontendLineCoverage

# Calcular cobertura promedio
$avgLineCoverage = ($backendLineCoverage + $frontendLineCoverage) / 2
$avgBranchCoverage = ($backendBranchCoverage + $frontendBranchCoverage) / 2

Write-Host "`n  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray

Write-Host "`n  🎯 COBERTURA PROMEDIO DEL SISTEMA" -ForegroundColor Cyan
Show-CoverageBar "    Lines:    " $avgLineCoverage
Show-CoverageBar "    Branches: " $avgBranchCoverage

$meta = 85.0
$faltante = [math]::Round($meta - $avgLineCoverage, 2)

if ($avgLineCoverage -ge $meta) {
    Write-Host "`n  🎉 " -NoNewline -ForegroundColor Green
    Write-Host "¡META ALCANZADA! Sistema con $avgLineCoverage% de cobertura" -ForegroundColor Green
} else {
    Write-Host "`n  📈 Meta: $meta% | Actual: $avgLineCoverage% | Faltante: $faltante puntos" -ForegroundColor Yellow
}

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                   REPORTES DISPONIBLES                         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "  📂 Backend:  " -NoNewline
Write-Host "coverage_backend_report\index.html" -ForegroundColor White
Write-Host "  📂 Frontend: " -NoNewline
Write-Host "frontend\coverage\lcov-report\index.html" -ForegroundColor White

Write-Host "`n¿Deseas abrir los reportes HTML? (S/N): " -NoNewline -ForegroundColor Yellow
$response = Read-Host

if ($response -match '^[Ss]') {
    Write-Host "`n🌐 Abriendo reportes..." -ForegroundColor Green
    
    $backendReport = Join-Path $rootPath "coverage_backend_report\index.html"
    $frontendReport = Join-Path $frontendPath "coverage\lcov-report\index.html"
    
    if (Test-Path $backendReport) {
        Start-Process $backendReport
    }
    
    if (Test-Path $frontendReport) {
        Start-Sleep -Seconds 1
        Start-Process $frontendReport
    }
}

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    EJECUCIÓN COMPLETADA                        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
