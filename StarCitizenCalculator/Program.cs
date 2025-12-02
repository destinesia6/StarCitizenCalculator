namespace StarCitizenCalculator;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; // Needed for AddSingleton
using System.Threading.Tasks;
using RazorConsole.Core;

internal static class Program
{
	private static async Task Main(string[] args)
	{
		Console.Title = "Star Citizen Delivery Calculator";

		try
		{
			IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);
                
			// 1. Configure Services: This is where we MUST register the AppState.
			hostBuilder.ConfigureServices((hostContext, services) =>
			{
				// Register the AppState model as a Singleton. 
				// This ensures the component receives the same instance across its lifecycle.
				services.AddSingleton(new AppState());
			});

			// 2. Configure RazorConsole: Use the confirmed extension method.
			// This registers all rendering services and identifies the root component.
			hostBuilder.UseRazorConsole<CalculatorApp>();

			// 3. Build and run the host
			IHost host = hostBuilder.Build();
			await host.RunAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"\n[ERROR] The application failed to start: {ex.Message}");
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}