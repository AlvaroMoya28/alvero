# Script de Presentación de Cobertura de Pruebas
# Ejecuta las pruebas con cobertura y muestra resultados profesionales

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   SISTEMA DE GESTIÓN DE EVENTOS - COBERTURA DE PRUEBAS        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "⏳ Limpiando reportes anteriores..." -ForegroundColor Yellow
Remove-Item -Path "coverage" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "🧪 Ejecutando suite completa de pruebas con cobertura...`n" -ForegroundColor Yellow

# Ejecutar pruebas con cobertura
$output = npm run test:coverage 2>&1 | Out-String
$exitCode = $LASTEXITCODE

# Mostrar últimas 40 líneas del output (resumen de Jest)
$lines = $output -split "`n"
$lastLines = $lines | Select-Object -Last 40
$lastLines | ForEach-Object { Write-Host $_ }

Write-Host "`n" -NoNewline

# Verificar si el reporte HTML existe
if (Test-Path "coverage\lcov-report\index.html") {
    Write-Host "✓ Reporte de cobertura generado exitosamente" -ForegroundColor Green
    
    # Leer el archivo HTML y extraer las métricas
    $htmlContent = Get-Content "coverage\lcov-report\index.html" -Raw
    
    # Extraer fracciones de cobertura usando regex mejorado
    $stmtMatch = [regex]::Match($htmlContent, 'statements.*?(\d+)/(\d+)')
    $branchMatch = [regex]::Match($htmlContent, 'branches.*?(\d+)/(\d+)')
    $funcMatch = [regex]::Match($htmlContent, 'functions.*?(\d+)/(\d+)')
    $lineMatch = [regex]::Match($htmlContent, 'lines.*?(\d+)/(\d+)')
    
    if ($stmtMatch.Success) {
        $stmtCovered = [int]$stmtMatch.Groups[1].Value
        $stmtTotal = [int]$stmtMatch.Groups[2].Value
        $stmtPercent = [math]::Round(($stmtCovered / $stmtTotal) * 100, 2)
        
        $branchCovered = [int]$branchMatch.Groups[1].Value
        $branchTotal = [int]$branchMatch.Groups[2].Value
        $branchPercent = [math]::Round(($branchCovered / $branchTotal) * 100, 2)
        
        $funcCovered = [int]$funcMatch.Groups[1].Value
        $funcTotal = [int]$funcMatch.Groups[2].Value
        $funcPercent = [math]::Round(($funcCovered / $funcTotal) * 100, 2)
        
        $lineCovered = [int]$lineMatch.Groups[1].Value
        $lineTotal = [int]$lineMatch.Groups[2].Value
        $linePercent = [math]::Round(($lineCovered / $lineTotal) * 100, 2)
        
        # Calcular progreso hacia meta de 85%
        $meta = 85.0
        $progreso = $stmtPercent
        $faltante = [math]::Round($meta - $progreso, 2)
        
        Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
        Write-Host "║                  RESULTADOS DE COBERTURA                       ║" -ForegroundColor Cyan
        Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan
        
        # Función para crear barra de progreso
        function Show-ProgressBar {
            param($label, $percent, $covered, $total)
            $barLength = 30
            $filled = [math]::Floor(($percent / 100) * $barLength)
            $empty = $barLength - $filled
            
            $color = if ($percent -ge 85) { "Green" } elseif ($percent -ge 70) { "Yellow" } else { "Red" }
            
            Write-Host "  $label " -NoNewline
            Write-Host ("[" + ("█" * $filled) + ("░" * $empty) + "]") -NoNewline -ForegroundColor $color
            Write-Host (" {0,6:N2}% " -f $percent) -NoNewline -ForegroundColor $color
            Write-Host "($covered/$total)" -ForegroundColor Gray
        }
        
        Show-ProgressBar "Statements:  " $stmtPercent $stmtCovered $stmtTotal
        Show-ProgressBar "Branches:    " $branchPercent $branchCovered $branchTotal
        Show-ProgressBar "Functions:   " $funcPercent $funcCovered $funcTotal
        Show-ProgressBar "Lines:       " $linePercent $lineCovered $lineTotal
        
        Write-Host "`n" -NoNewline
        Write-Host "  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
        
        # Mostrar progreso hacia meta
        if ($progreso -ge $meta) {
            Write-Host "`n  🎉 " -NoNewline -ForegroundColor Green
            Write-Host "¡META ALCANZADA! " -NoNewline -ForegroundColor Green
            Write-Host "Cobertura: " -NoNewline
            Write-Host "$progreso%" -ForegroundColor Green -NoNewline
            Write-Host " (Supera el objetivo de $meta%)" -ForegroundColor Green
        } else {
            Write-Host "`n  📊 Progreso hacia meta de $meta%:" -ForegroundColor Cyan
            Write-Host "     • Cobertura actual: " -NoNewline
            Write-Host "$progreso%" -ForegroundColor Yellow
            Write-Host "     • Faltante: " -NoNewline
            
            if ($faltante -le 5) {
                Write-Host "$faltante puntos porcentuales" -ForegroundColor Yellow
                Write-Host "     • Estado: " -NoNewline
                Write-Host "¡Casi en la meta! 🎯" -ForegroundColor Yellow
            } elseif ($faltante -le 10) {
                Write-Host "$faltante puntos porcentuales" -ForegroundColor Yellow
                Write-Host "     • Estado: " -NoNewline
                Write-Host "Buen progreso 📈" -ForegroundColor Yellow
            } else {
                Write-Host "$faltante puntos porcentuales" -ForegroundColor Red
                Write-Host "     • Estado: " -NoNewline
                Write-Host "Necesita más cobertura 🔧" -ForegroundColor Red
            }
        }
        
        Write-Host "`n" -NoNewline
        Write-Host "  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGray
        Write-Host ""
    }
    
    Write-Host "`n📂 Reporte HTML disponible en: " -NoNewline -ForegroundColor Cyan
    Write-Host "coverage\lcov-report\index.html" -ForegroundColor White
    
    Write-Host "`n¿Deseas abrir el reporte HTML en el navegador? (S/N): " -NoNewline -ForegroundColor Yellow
    $response = Read-Host
    
    if ($response -match '^[Ss]') {
        Write-Host "🌐 Abriendo reporte en navegador..." -ForegroundColor Green
        Start-Process "coverage\lcov-report\index.html"
    }
} else {
    Write-Host "⚠ No se pudo generar el reporte de cobertura" -ForegroundColor Red
}

Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                    EJECUCIÓN COMPLETADA                        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

exit $exitCode
