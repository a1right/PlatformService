using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this._environment = environment;
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }
            
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGrpcService<GrpcPlatformService>();

                    endpoints.MapGet("/protos/platforms.proto", async context =>
                    {
                        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
                    });
                }
            );
            PrepDb.PrepPopulation(app, env.IsProduction());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_environment.IsProduction())
            {
                Console.WriteLine("--> Using SQLServer Database");
                services.AddDbContext<PlatformDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("PlaftormsConn")));
            }
            else
            {
                Console.WriteLine("--> Using InMemory Database");
                services.AddDbContext<PlatformDbContext>(options =>
                    options.UseInMemoryDatabase("InMemory"));
            }

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IPlatformRepository, PlatformRepository>();
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddGrpc();

            Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandServiceUrl"]}");

            
        }
    }
}
