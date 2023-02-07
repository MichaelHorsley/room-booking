using host_api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace host_api_tests;

internal class TestingApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;

    private readonly Dictionary<Type, object> _serviceDictionary = new();

    public TestingApplication(string environment = "Development")
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        builder.ConfigureServices(services =>
        {
            foreach (var (key, value) in _serviceDictionary)
            {
                services.AddSingleton(key, value);
            }
        });

        return base.CreateHost(builder);
    }

    public void AddServiceToDependencyInjection<T>(T commandHandler)
    {
        _serviceDictionary.Add(typeof(T), commandHandler);
    }
}