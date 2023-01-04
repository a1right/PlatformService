using Microsoft.AspNetCore;

namespace CommandsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                                 .UseStartup<Startup>();

            var app = builder.Build();
            
            app.Run();
        }
    }
}