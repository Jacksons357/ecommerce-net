# Script para resetar completamente o banco de dados e aplicar seeders
Write-Host "🔄 Resetando banco de dados..." -ForegroundColor Yellow

# Parar aplicação se estiver rodando
Write-Host "📦 Parando aplicações..." -ForegroundColor Blue

# Remover database
Write-Host "🗑️ Removendo banco de dados..." -ForegroundColor Red
dotnet ef database drop --force --project src\Ecommerce.Infrastructure --startup-project src\Ecommerce.Api

# Aplicar todas as migrations novamente
Write-Host "⚙️ Aplicando migrations..." -ForegroundColor Green
dotnet ef database update --project src\Ecommerce.Infrastructure --startup-project src\Ecommerce.Api

Write-Host "✅ Banco de dados resetado com sucesso!" -ForegroundColor Green
Write-Host "🚀 Agora você pode executar: dotnet run --project src\Ecommerce.Api --urls http://localhost:5000" -ForegroundColor Cyan
