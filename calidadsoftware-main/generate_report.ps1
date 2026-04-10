# Script para generar reportes detallados
cd C:\Users\Álvaro\Desktop\UCR\SemesterVIII\calidadsoftware
npm run --prefix ./frontend test:coverage 2>&1 | Out-File frontend_coverage_detail.txt
Get-Content frontend_coverage_detail.txt | Select-String -Pattern "^\s+\S.*\.vue\s+\|" | Where-Object { $_ -match '\|\s+[0-5]\d?\s+\|' } | Select-Object -First 30
