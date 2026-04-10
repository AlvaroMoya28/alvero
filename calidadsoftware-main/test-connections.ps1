#!/usr/bin/env pwsh
# Script simple para probar conexiones concurrentes al servidor

param(
    [int]$NumUsuarios = 25,
    [string]$BaseUrl = "https://localhost:5001"
)

Write-Host "======================================"
Write-Host "  PRUEBA DE CONEXIONES CONCURRENTES"
Write-Host "======================================"
Write-Host ""
Write-Host "Configuración:"
Write-Host "  - Usuarios concurrentes: $NumUsuarios"
Write-Host "  - URL: $BaseUrl"
Write-Host ""

$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$jobs = @()

Write-Host "Iniciando $NumUsuarios conexiones concurrentes..." -ForegroundColor Yellow
Write-Host ""

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

# Crear trabajos concurrentes
for ($i = 1; $i -le $NumUsuarios; $i++) {
    $job = Start-Job -ScriptBlock {
        param($BaseUrl, $Index, $Timestamp)
        
        $uniqueId = [guid]::NewGuid().ToString("N").Substring(0, 8)
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        
        try {
            $body = @{
                idUsuario = "LOAD${Timestamp}${Index}${uniqueId}"
                nombre = "Usuario $Index"
                apellido1 = "Test"
                apellido2 = "Concurrency"
                email = "load${Timestamp}${Index}${uniqueId}@test.com"
                telefono = "88888888"
                contrasena = "Password123!"
                confirmarContrasena = "Password123!"
                rol = "Cliente"
                fechaNacimiento = "1990-01-01"
            } | ConvertTo-Json -Compress
            
            $params = @{
                Uri = "$BaseUrl/api/Usuarios/registro"
                Method = 'POST'
                Body = $body
                ContentType = 'application/json'
                TimeoutSec = 30
            }
            
            if ($BaseUrl -like "https://*") {
                $params['SkipCertificateCheck'] = $true
            }
            
            $response = Invoke-RestMethod @params
            $sw.Stop()
            
            return @{
                Index = $Index
                Success = $true
                ElapsedMs = $sw.ElapsedMilliseconds
                UserId = $response.idUsuario
            }
        } catch {
            $sw.Stop()
            return @{
                Index = $Index
                Success = $false
                ElapsedMs = $sw.ElapsedMilliseconds
                Error = $_.Exception.Message
            }
        }
    } -ArgumentList $BaseUrl, $i, $timestamp
    
    $jobs += $job
}

# Esperar resultados con indicador de progreso
$completed = 0
$totalJobs = $jobs.Count

while ($completed -lt $totalJobs) {
    $completed = ($jobs | Where-Object { $_.State -eq 'Completed' -or $_.State -eq 'Failed' }).Count
    $percent = [Math]::Round(($completed / $totalJobs) * 100)
    Write-Progress -Activity "Esperando respuestas" -Status "$completed de $totalJobs completadas" -PercentComplete $percent
    Start-Sleep -Milliseconds 100
}

Write-Progress -Activity "Esperando respuestas" -Completed

$stopwatch.Stop()

# Recopilar resultados
$resultados = @()
foreach ($job in $jobs) {
    $result = Receive-Job -Job $job
    if ($result) {
        $resultados += $result
    }
    Remove-Job -Job $job
}

# Análisis
Write-Host ""
Write-Host "======================================"
Write-Host "  RESULTADOS"
Write-Host "======================================"
Write-Host ""

$exitosos = ($resultados | Where-Object { $_.Success }).Count
$fallidos = $NumUsuarios - $exitosos
$porcentajeExito = [Math]::Round(($exitosos / $NumUsuarios) * 100, 2)

Write-Host "Total de conexiones: $NumUsuarios"
Write-Host "Exitosas: $exitosos ($porcentajeExito%)" -ForegroundColor $(if($exitosos -ge $NumUsuarios * 0.8){"Green"}else{"Yellow"})
Write-Host "Fallidas: $fallidos" -ForegroundColor $(if($fallidos -gt 0){"Red"}else{"Green"})
Write-Host "Tiempo total: $($stopwatch.ElapsedMilliseconds)ms"

if ($exitosos -gt 0) {
    $tiempos = ($resultados | Where-Object { $_.Success } | Select-Object -ExpandProperty ElapsedMs)
    $promedio = [Math]::Round(($tiempos | Measure-Object -Average).Average, 2)
    $minimo = ($tiempos | Measure-Object -Minimum).Minimum
    $maximo = ($tiempos | Measure-Object -Maximum).Maximum
    
    Write-Host ""
    Write-Host "Tiempos de respuesta:"
    Write-Host "  - Promedio: ${promedio}ms"
    Write-Host "  - Mínimo: ${minimo}ms"
    Write-Host "  - Máximo: ${maximo}ms"
    
    $throughput = [Math]::Round(($NumUsuarios * 1000.0 / $stopwatch.ElapsedMilliseconds), 2)
    Write-Host "  - Throughput: $throughput ops/seg"
}

if ($fallidos -gt 0) {
    Write-Host ""
    Write-Host "Errores:" -ForegroundColor Red
    $errores = $resultados | Where-Object { -not $_.Success } | Select-Object -First 5
    foreach ($error in $errores) {
        Write-Host "  Usuario $($error.Index): $($error.Error)"
    }
    if ($fallidos -gt 5) {
        Write-Host "  ... y $($fallidos - 5) errores más"
    }
}

Write-Host ""
Write-Host "======================================"

# Evaluación GQM
if ($porcentajeExito -ge 80 -and $exitosos -ge 100) {
    Write-Host "✅ RESULTADO: El sistema soporta $NumUsuarios usuarios concurrentes" -ForegroundColor Green
    Write-Host "   (Umbral GQM: ≥100 usuarios con ≥80% éxito)" -ForegroundColor Green
} elseif ($porcentajeExito -ge 80) {
    Write-Host "⚠️  RESULTADO: El sistema soporta $NumUsuarios usuarios concurrentes" -ForegroundColor Yellow
    Write-Host "   (Se requieren ≥100 para cumplir umbral GQM completo)" -ForegroundColor Yellow
} else {
    Write-Host "❌ RESULTADO: Tasa de éxito baja ($porcentajeExito%)" -ForegroundColor Red
    Write-Host "   (Se requiere ≥80% para cumplir umbral GQM)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Para probar más usuarios:" -ForegroundColor Cyan
Write-Host "  .\test-connections.ps1 -NumUsuarios 50" -ForegroundColor White
Write-Host "  .\test-connections.ps1 -NumUsuarios 100" -ForegroundColor White
Write-Host "  .\test-connections.ps1 -NumUsuarios 200" -ForegroundColor White
Write-Host ""
