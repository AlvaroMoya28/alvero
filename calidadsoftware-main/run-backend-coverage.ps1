# Script Simple de Cobertura Backend
# Genera reporte incluso con tests fallidos

Write-Host "`n🔧 BACKEND - Cobertura de Pruebas" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor DarkGray

$testProjectPath = "C:\Users\Álvaro\Desktop\UCR\SemesterVIII\calidadsoftware\EventosBackend.Tests"

Push-Location $testProjectPath

# Limpiar resultados anteriores
Remove-Item -Path "TestResults" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "🧪 Ejecutando tests con cobertura..." -ForegroundColor Yellow

# Ejecutar con coverlet
$result = dotnet test `
    --collect:"XPlat Code Coverage" `
    --results-directory:"./TestResults" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Include="[EventosBackend]*" `
    2>&1

Write-Host $result

# Buscar archivo de cobertura
$coverageFile = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

if (!$coverageFile) {
    Write-Host "⚠ No se encontró coverage.cobertura.xml, buscando alternativas..." -ForegroundColor Yellow
    $coverageFile = Get-ChildItem -Path "TestResults" -Filter "*.cobertura.xml" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
}

if ($coverageFile) {
    Write-Host "✓ Archivo de cobertura encontrado: $($coverageFile.FullName)" -ForegroundColor Green
    
    # Instalar ReportGenerator si no está instalado
    dotnet tool install --global dotnet-reportgenerator-globaltool --ignore-failed-sources 2>&1 | Out-Null
    
    $reportPath = "C:\Users\Álvaro\Desktop\UCR\SemesterVIII\calidadsoftware\coverage_backend_report"
    
    Write-Host "📊 Generando reporte HTML..." -ForegroundColor Yellow
    
    reportgenerator `
        "-reports:$($coverageFile.FullName)" `
        "-targetdir:$reportPath" `
        "-reporttypes:Html;TextSummary" 2>&1 | Out-Null
    
    if (Test-Path "$reportPath\index.html") {
        Write-Host "✓ Reporte generado: $reportPath\index.html" -ForegroundColor Green
        
        # Leer resumen
        $summaryFile = Join-Path $reportPath "Summary.txt"
        if (Test-Path $summaryFile) {
            Write-Host "`n📈 RESUMEN DE COBERTURA:" -ForegroundColor Cyan
            Get-Content $summaryFile
        }
        
        Write-Host "`n¿Abrir reporte en navegador? (S/N): " -NoNewline -ForegroundColor Yellow
        $response = Read-Host
        
        if ($response -match '^[Ss]') {
            Start-Process "$reportPath\index.html"
        }
    } else {
        Write-Host "⚠ No se pudo generar el reporte HTML" -ForegroundColor Red
    }
} else {
    Write-Host "❌ No se encontró ningún archivo de cobertura en TestResults" -ForegroundColor Red
    Write-Host "Contenido de TestResults:" -ForegroundColor Gray
    Get-ChildItem -Path "TestResults" -Recurse | ForEach-Object { Write-Host "  $_" }
}

Pop-Location

Write-Host "`n✅ Proceso completado`n" -ForegroundColor Green
