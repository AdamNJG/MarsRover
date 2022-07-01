using MarsRover.Forms;
using MarsRover.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{

    public static void Main(String[] args)
    {
        IHost host = CreateHostBuilder().Build();
        
        Application.Run(host.Services.GetRequiredService<MainForm>());
    }

    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<MainForm>();
                services.AddSingleton<IRoverService, RoverService>();
            });
            
    }
}

