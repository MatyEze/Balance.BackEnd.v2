using Balance.BackEnd.v2.Datos.SupabaseDB;
using Balance.BackEnd.v2.Datos.SupabaseDB.Mapeos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Mapeos;
using Balance.BackEnd.v2.StartupExtension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CACHE
builder.Services.AddMemoryCache();

//SupabaseDB
builder.Services.AddScoped<ISupabaseDB, SupabaseDB>();
builder.Services.AddAutoMapper(typeof(SupabaseDBProfile));

//Se agregan servicios por inyeccion, estan incluidos los profiles de automaper que usa cada servicio
ServiceExtension.AddScopesServices(builder.Services);

//AllowedOriginsCors
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.WithOrigins(allowedOrigins!) // Reemplaza con la URL de tu aplicación Angular
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
