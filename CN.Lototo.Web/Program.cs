using Azure.Storage.Blobs;
using CN.Lototo.Application.Interfaces;
using CN.Lototo.Application.Services;
using CN.Lototo.Domain.Interfaces;
using CN.Lototo.Infrastructure.Data;
using CN.Lototo.Infrastructure.Data.Repositories;
using CN.Lototo.Infrastructure.Data.UnitOfWork;
using CN.Lototo.Infrastructure.Storage;
using CN.Lototo.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
//  RAZOR COMPONENTS (.NET 8 / Blazor Server)
// ===========================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

// ===========================================
//  AUTH (APENAS DENTRO DO BLAZOR)
// ===========================================
// Provider customizado que guarda o utilizador em memória
builder.Services.AddScoped<LototoAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<LototoAuthenticationStateProvider>());

// Autorização para usar [Authorize] nos componentes
builder.Services.AddAuthorizationCore();

// Serviço de autenticação de domínio (usa o provider acima)
builder.Services.AddScoped<AuthService>();

// ===========================================
//  BANCO DE DADOS
// ===========================================
builder.Services.AddDbContext<LototoContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Lototo");
    options.UseSqlServer(cs);
});

// ===========================================
//  REPOSITÓRIOS + UOW
// ===========================================

builder.Services.AddSingleton<BlobServiceClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connString = config.GetConnectionString("BlobStorage");
    return new BlobServiceClient(connString);
});

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IPlantaRepository, PlantaRepository>();
builder.Services.AddScoped<PlantasService>();
builder.Services.AddScoped<UsuariosService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IEquipamentoExcelParser, EquipmentExcelParser>();
builder.Services.AddScoped<IImageOcrService, AzureImageOcrService>();

builder.Services.AddScoped<ProtectedSessionStorage>();

// Application
builder.Services.AddScoped<IEquipamentoAppService, EquipamentoAppService>();

builder.Services.AddHttpClient();

var app = builder.Build();

// ===========================================
//  PIPELINE HTTP
// ===========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// se tiver página de not-found
app.UseStatusCodePagesWithReExecute("/not-found");

// Antiforgery + assets estáticos (padrão template .NET 8)
app.UseAntiforgery();
app.MapStaticAssets();

// Mapeia os Razor Components (Blazor Server)
app.MapRazorComponents<CN.Lototo.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
