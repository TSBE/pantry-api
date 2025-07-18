﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Core.Persistence;
using Pantry.Service;

namespace Pantry.Tests.Component.Integration.Environment;

internal sealed class IntegrationTestWebApplicationFactory : IntegrationTestWebApplicationFactoryBase<Startup>
{
    private readonly Action<IServiceCollection>? _servicesConfigAction;

    private IntegrationTestWebApplicationFactory(Action<IServiceCollection>? servicesConfigAction = null)
        : base()
    {
        _servicesConfigAction = servicesConfigAction;
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task<IntegrationTestWebApplicationFactory> CreateAsync(ITestOutputHelper outputHelper, Action<IServiceCollection>? servicesConfigAction = null)
#pragma warning restore IDE0060 // Remove unused parameter
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
