#!/usr/bin/env pwsh
# Script de Prueba de Concurrencia Real
# Ejecuta contra el servidor en ejecución para medir usuarios concurrentes

param(
    [int]$NumUsuarios = 25,
    [string]$BaseUrl = "https://localhost:5001",
    [int]$TiempoEsperaMax = 30,
    [switch]$SkipServerCheck
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  PRUEBA DE CONCURRENCIA REAL" -ForegroundColor Cyan
Write-Host "  Sistema de Gestión de Citas" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que el servidor esté en ejecución
Write-Host "🔍 Verificando servidor en $BaseUrl..." -ForegroundColor Yellow
try {
    # Intentar con /swagger primero, luego con endpoint de API
    $requestParams = @{
        Method = 'GET'
        TimeoutSec = 5
        ErrorAction = 'Stop'
    }
    
    # Agregar SkipCertificateCheck para HTTPS
    if ($BaseUrl -like "https://*") {
        $requestParams['SkipCertificateCheck'] = $true
    }
    
    try {
        $requestParams['Uri'] = "$BaseUrl/swagger/index.html"
        $response = Invoke-WebRequest @requestParams
    } catch {
        # Intentar endpoint de API directo
        $requestParams['Uri'] = "$BaseUrl/api/Usuario"
        $response = Invoke-WebRequest @requestParams
    }
    Write-Host "✓ Servidor disponible" -ForegroundColor Green
} catch {
    Write-Host "✗ Error: El servidor no está en ejecución" -ForegroundColor Red
    Write-Host "  Por favor, ejecuta: ./start-dev.ps1" -ForegroundColor Yellow
    Write-Host "  O verifica que el puerto sea correcto" -ForegroundColor Yellow
    Write-Host "  Puertos comunes: http://localhost:5000 o https://localhost:5001" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "📊 Configuración de prueba:" -ForegroundColor Cyan
Write-Host "  - Usuarios concurrentes: $NumUsuarios"
Write-Host "  - URL Base: $BaseUrl"
Write-Host "  - Endpoint: POST /api/Usuario/registrar"
Write-Host ""

# Generar timestamp único para esta ejecución
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$resultados = @()
$jobs = @()

Write-Host "🚀 Iniciando prueba de concurrencia..." -ForegroundColor Green
Write-Host ""

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

# Crear trabajos en paralelo
for ($i = 1; $i -le $NumUsuarios; $i++) {
    $job = Start-Job -ScriptBlock {
        param($BaseUrl, $Index, $Timestamp)
        
        $uniqueId = [guid]::NewGuid().ToString("N").Substring(0, 8)
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        
        try {
            $body = @{
                idUsuario = "LOAD${Timestamp}${Index}${uniqueId}"
                nombre = "Usuario Load Test $Index"
                apellido1 = "Test"
                apellido2 = "Concurrency"
                email = "loadtest${Timestamp}${Index}${uniqueId}@example.com"
                telefono = "88888888"
                contrasena = "Password123!"
                confirmarContrasena = "Password123!"
                rol = "Cliente"
                fechaNacimiento = "1990-01-01"
            } | ConvertTo-Json
            
            $headers = @{
                "Content-Type" = "application/json"
                "Accept" = "application/json"
            }
            
            $requestParams = @{
                Uri = "$BaseUrl/api/Usuario/registrar"
                Method = 'POST'
                Body = $body
                Headers = $headers
                TimeoutSec = 30
                ErrorAction = 'Stop'
            }
            
            # Agregar SkipCertificateCheck para HTTPS
            if ($BaseUrl -like "https://*") {
                $requestParams['SkipCertificateCheck'] = $true
            }
            
            $response = Invoke-RestMethod @requestParams
            
            $sw.Stop()
            
            return @{
                Index = $Index
                Success = $true
                StatusCode = 200
                ElapsedMs = $sw.ElapsedMilliseconds
                Message = "OK"
                UserId = $response.idUsuario
            }
        } catch {
            $sw.Stop()
            
            $statusCode = 0
            $message = $_.Exception.Message
            
            if ($_.Exception.Response) {
                $statusCode = [int]$_.Exception.Response.StatusCode
            }
            
            return @{
                Index = $Index
                Success = $false
                StatusCode = $statusCode
                ElapsedMs = $sw.ElapsedMilliseconds
                Message = $message
                UserId = $null
            }
        }
    } -ArgumentList $BaseUrl, $i, $timestamp
    
    $jobs += $job
}

Write-Host "⏳ Esperando respuestas de $NumUsuarios solicitudes concurrentes..." -ForegroundColor Yellow

# Esperar a que todos los trabajos terminen
$allJobs = Wait-Job -Job $jobs -Timeout $TiempoEsperaMax

$stopwatch.Stop()

# Recopilar resultados
foreach ($job in $jobs) {
    $result = Receive-Job -Job $job
    if ($result) {
        $resultados += $result
    }
    Remove-Job -Job $job
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  RESULTADOS DE CONCURRENCIA" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Análisis de resultados
$exitosos = ($resultados | Where-Object { $_.Success -eq $true }).Count
$fallidos = ($resultados | Where-Object { $_.Success -eq $false }).Count
$tiempoTotal = $stopwatch.ElapsedMilliseconds

if ($exitosos -gt 0) {
    $tiemposExitosos = ($resultados | Where-Object { $_.Success -eq $true } | Select-Object -ExpandProperty ElapsedMs)
    $tiempoPromedio = ($tiemposExitosos | Measure-Object -Average).Average
    $tiempoMin = ($tiemposExitosos | Measure-Object -Minimum).Minimum
    $tiempoMax = ($tiemposExitosos | Measure-Object -Maximum).Maximum
    
    # Calcular percentil 95
    $sorted = $tiemposExitosos | Sort-Object
    $p95Index = [Math]::Floor($sorted.Count * 0.95)
    $p95 = $sorted[$p95Index]
} else {
    $tiempoPromedio = 0
    $tiempoMin = 0
    $tiempoMax = 0
    $p95 = 0
}

$porcentajeExito = [Math]::Round(($exitosos / $NumUsuarios) * 100, 2)
$throughput = [Math]::Round(($NumUsuarios * 1000.0 / $tiempoTotal), 2)

# Mostrar resultados
Write-Host "📈 Métricas de Rendimiento:" -ForegroundColor Green
Write-Host "  ├─ Total de solicitudes: $NumUsuarios"
Write-Host "  ├─ Solicitudes exitosas: $exitosos ($porcentajeExito%)"
Write-Host "  ├─ Solicitudes fallidas: $fallidos"
Write-Host "  ├─ Tiempo total: ${tiempoTotal}ms"
Write-Host "  └─ Throughput: $throughput ops/segundo"
Write-Host ""

if ($exitosos -gt 0) {
    Write-Host "⏱️  Estadísticas de Tiempo (solicitudes exitosas):" -ForegroundColor Green
    Write-Host "  ├─ Tiempo promedio: ${tiempoPromedio}ms"
    Write-Host "  ├─ Tiempo mínimo: ${tiempoMin}ms"
    Write-Host "  ├─ Tiempo máximo: ${tiempoMax}ms"
    Write-Host "  └─ Percentil 95 (P95): ${p95}ms"
    Write-Host ""
}

# Evaluar resultado según GQM
Write-Host "🎯 Evaluación GQM:" -ForegroundColor Cyan

# Objetivo 4.1: Usuarios Concurrentes (≥100 usuarios)
if ($NumUsuarios -ge 100 -and $porcentajeExito -ge 80) {
    Write-Host "  ✓ Usuarios Concurrentes: ACEPTABLE ($NumUsuarios usuarios, ${porcentajeExito}% éxito)" -ForegroundColor Green
} elseif ($NumUsuarios -ge 50 -and $porcentajeExito -ge 80) {
    Write-Host "  ⚠ Usuarios Concurrentes: PARCIAL ($NumUsuarios usuarios, ${porcentajeExito}% éxito)" -ForegroundColor Yellow
    Write-Host "    Recomendación: Probar con ≥100 usuarios" -ForegroundColor Yellow
} else {
    Write-Host "  ✗ Usuarios Concurrentes: REQUIERE MEJORA ($NumUsuarios usuarios, ${porcentajeExito}% éxito)" -ForegroundColor Red
}

# Objetivo 4.2: Porcentaje de transacciones <2000ms (≥95%)
if ($exitosos -gt 0) {
    $transaccionesBajo2s = ($tiemposExitosos | Where-Object { $_ -lt 2000 }).Count
    $porcentajeSLA = [Math]::Round(($transaccionesBajo2s / $exitosos) * 100, 2)
    
    if ($porcentajeSLA -ge 95) {
        Write-Host "  ✓ SLA de Tiempo: ACEPTABLE (${porcentajeSLA}% < 2000ms)" -ForegroundColor Green
    } elseif ($porcentajeSLA -ge 80) {
        Write-Host "  ⚠ SLA de Tiempo: PARCIAL (${porcentajeSLA}% < 2000ms)" -ForegroundColor Yellow
    } else {
        Write-Host "  ✗ SLA de Tiempo: REQUIERE MEJORA (${porcentajeSLA}% < 2000ms)" -ForegroundColor Red
    }
}

Write-Host ""

# Mostrar errores si los hay
if ($fallidos -gt 0) {
    Write-Host "⚠️  Detalles de Errores:" -ForegroundColor Yellow
    $errores = $resultados | Where-Object { $_.Success -eq $false } | Select-Object Index, StatusCode, Message
    foreach ($error in $errores | Select-Object -First 10) {
        Write-Host "  Usuario $($error.Index): [HTTP $($error.StatusCode)] $($error.Message)"
    }
    if ($fallidos -gt 10) {
        Write-Host "  ... y $($fallidos - 10) errores más"
    }
    Write-Host ""
}

# Guardar resultados en JSON
$reportPath = "TestResults/ConcurrencyTest_${timestamp}.json"
New-Item -ItemType Directory -Force -Path (Split-Path $reportPath) | Out-Null

$report = @{
    timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    configuracion = @{
        numUsuarios = $NumUsuarios
        baseUrl = $BaseUrl
        endpoint = "/api/Usuario/registrar"
    }
    resultados = @{
        totalSolicitudes = $NumUsuarios
        exitosas = $exitosos
        fallidas = $fallidos
        porcentajeExito = $porcentajeExito
        tiempoTotalMs = $tiempoTotal
        throughput = $throughput
    }
    estadisticas = @{
        tiempoPromedioMs = [Math]::Round($tiempoPromedio, 2)
        tiempoMinimoMs = $tiempoMin
        tiempoMaximoMs = $tiempoMax
        percentil95Ms = $p95
        porcentajeSLA = if ($exitosos -gt 0) { $porcentajeSLA } else { 0 }
    }
    evaluacionGQM = @{
        usuariosConcurrentes = if ($NumUsuarios -ge 100 -and $porcentajeExito -ge 80) { "ACEPTABLE" } 
                               elseif ($NumUsuarios -ge 50 -and $porcentajeExito -ge 80) { "PARCIAL" } 
                               else { "REQUIERE MEJORA" }
        slaTiempo = if ($exitosos -gt 0 -and $porcentajeSLA -ge 95) { "ACEPTABLE" }
                    elseif ($exitosos -gt 0 -and $porcentajeSLA -ge 80) { "PARCIAL" }
                    else { "REQUIERE MEJORA" }
    }
    detalles = $resultados
}

$report | ConvertTo-Json -Depth 10 | Out-File -FilePath $reportPath -Encoding UTF8

Write-Host "💾 Reporte guardado en: $reportPath" -ForegroundColor Cyan
Write-Host ""

# Conclusión
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "🏁 Prueba completada" -ForegroundColor Green
Write-Host ""

if ($porcentajeExito -ge 80 -and ($exitosos -eq 0 -or $porcentajeSLA -ge 95)) {
    Write-Host "✅ El sistema maneja correctamente $NumUsuarios usuarios concurrentes" -ForegroundColor Green
    Write-Host "   Métricas GQM: ACEPTABLES" -ForegroundColor Green
} elseif ($porcentajeExito -ge 50) {
    Write-Host "⚠️  El sistema maneja parcialmente $NumUsuarios usuarios concurrentes" -ForegroundColor Yellow
    Write-Host "   Métricas GQM: PARCIALES" -ForegroundColor Yellow
} else {
    Write-Host "❌ El sistema requiere optimización para $NumUsuarios usuarios concurrentes" -ForegroundColor Red
    Write-Host "   Métricas GQM: REQUIEREN MEJORA" -ForegroundColor Red
}

Write-Host ""
Write-Host "💡 Para probar diferentes niveles de concurrencia:" -ForegroundColor Cyan
Write-Host "   .\test-concurrency.ps1 -NumUsuarios 50" -ForegroundColor White
Write-Host "   .\test-concurrency.ps1 -NumUsuarios 100" -ForegroundColor White
Write-Host "   .\test-concurrency.ps1 -NumUsuarios 200" -ForegroundColor White
Write-Host ""
