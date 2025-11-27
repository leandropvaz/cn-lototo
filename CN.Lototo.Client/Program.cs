using Blazored.LocalStorage;
using CN.Lototo.Client;
using CN.Lototo.Client.Services;
using CN.Lototo.Domain.Interfaces;
using CN.Lototo.Infrastructure.Data;
using CN.Lototo.Infrastructure.Data.Repositories;
using CN.Lototo.Infrastructure.Data.UnitOfWork;
using CN.Lototo.Infrastructure.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddBlazoredLocalStorage();

// Auth Service
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthHeaderHandler>();

// Auth Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());

// DbContext
builder.Services.AddDbContext<LototoContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("BancoLototo");
    options.UseSqlServer(cs);
});

// DI repositórios e UoW
builder.Services.AddScoped<IPlantaRepository, PlantaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<StorageService>();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var chave = Encoding.UTF8.GetBytes(jwt["Chave"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwt["Emissor"],
        ValidAudience = jwt["Publico"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chave),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PerfilAdministrador", policy =>
        policy.RequireRole("Administrador", "SuperGestor"));

    options.AddPolicy("PerfilSuperGestor", policy =>
        policy.RequireRole("SuperGestor"));
});


// Authorization
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
