using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddDbContext<PlatformDbContext>(options =>
                options.UseInMemoryDatabase("InMemory"));
            builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
            builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandServiceUrl"]}");

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            PrepDb.PrepPopulation(app);

            app.Run();
        }
    }
}