using AuctionService.Data;
using AuctionService.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IAuctionsRepository, AuctionsRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
//End Delete

app.UseAuthorization();

app.MapControllers();
try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}

app.Run();
