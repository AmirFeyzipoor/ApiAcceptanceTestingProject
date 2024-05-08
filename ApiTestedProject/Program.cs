using Microsoft.EntityFrameworkCore;
using WebApiTestedProject.Infrastructor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OnlineShopDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//await UpdateDatabase(app);

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("webApi started");
Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

app.Run();

//async Task UpdateDatabase(IApplicationBuilder app)
//{
//    using (var serviceScope = app.ApplicationServices
//        .GetRequiredService<IServiceScopeFactory>().CreateScope())
//    {
//        var dbContext = serviceScope.ServiceProvider
//      .GetService<OnlineShopDbContext>();

//        await dbContext!.Database.MigrateAsync();
//    }
//}
