using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Core.Persistence;
using Pantry.Service;
using Pantry.Tests.Common;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Environment;

public class IntegrationTestWebApplicationFactory : IntegrationTestWebApplicationFactoryBase<Startup>
{
    private readonly Action<IServiceCollection>? _servicesConfigAction;

    private IntegrationTestWebApplicationFactory(Action<IServiceCollection>? servicesConfigAction = null)
        : base()
    {
        _servicesConfigAction = servicesConfigAction;
    }

    public static async Task<IntegrationTestWebApplicationFactory> CreateAsync(ITestOutputHelper outputHelper, Action<IServiceCollection>? servicesConfigAction = null)
    {
        IntegrationTestWebApplicationFactory testApplication = new(servicesConfigAction);
        await testApplication.SetupDatabaseAsync<AppDbContext>();

        return testApplication;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(
            services =>
            {
                services.OverrideWithSharedInMemorySqliteDatabase<AppDbContext>();
                services.ConfigureSilverback();

                _servicesConfigAction?.Invoke(services);
            });
    }
}
