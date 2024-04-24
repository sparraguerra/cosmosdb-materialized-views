var appName = Assembly.GetExecutingAssembly().GetName().Name!;
Log.Logger = new LoggerConfiguration()
               .CreateBootstrapLogger();

Log.Information($"Starting up {appName}");

try
{

    var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var environment = context.HostingEnvironment;
        configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        configuration.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        var environment = context.HostingEnvironment;
        services.AddHttpContextAccessor();
        services.AddSingleton(context.Configuration);
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.AllowTrailingCommas = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TimespanJsonConverter());
        });

        services.AddSingleton(s => 
        {
            var connectionString = context.Configuration["CosmosDBConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
            }

            var serializerOptions = new CosmosSerializationOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            };

            return new CosmosClientBuilder(connectionString)
                    .WithSerializerOptions(serializerOptions)
                    .Build();
        });
    })
    .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
if (type.Equals("StopTheHostException", StringComparison.Ordinal))
{
    throw;
}
Log.Fatal(ex, $"{appName} Unhandled exception");
}
finally
{
    Log.Information($"{appName} Shut down complete");
    Log.CloseAndFlush();
}