using Bookenstein.Application.Interfaces;
using Bookenstein.Infrastructure.Persistence;
using Bookenstein.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (PostgreSQL) – usa a connection string do appsettings.json
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI dos repositórios/adapters
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Swagger apenas em DEV (ajuste se quiser em Prod)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseAuthentication(); // habilite quando tiver auth
// app.UseAuthorization();

app.MapControllers();

// (Opcional) aplicar migrations no start – útil em DEV
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
