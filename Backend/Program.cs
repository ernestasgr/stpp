using Microsoft.EntityFrameworkCore;
using backend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddEntityFrameworkNpgsql()
    .AddDbContext<ApiDbContext>(opt => opt.UseNpgsql(
        builder.Configuration.GetConnectionString("Database")
    ));

builder.Services.AddTransient<IRepository<Movie>, MovieRepository>();
builder.Services.AddTransient<IRepository<Showing>, ShowingRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
