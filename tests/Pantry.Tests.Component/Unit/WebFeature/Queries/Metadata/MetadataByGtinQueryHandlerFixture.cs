using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Pantry.Core.Models.EanSearchOrg;
using Pantry.Core.Models.OpenFoodFacts;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.EanSearchOrg;
using Pantry.Features.EanSearchOrg.Configuration;
using Pantry.Features.OpenFoodFacts;
using Pantry.Features.OpenFoodFacts.Responses;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class MetadataByGtinQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var metadata = new Metadata { GlobalTradeItemNumber = "GTIN", FoodFacts = new Core.Models.OpenFoodFacts.Product { Brands = "Unittest" } };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Metadatas.Add(metadata);
        });
        var openFoodFactsApiService = Substitute.For<IOpenFoodFactsApiService>();
        var eanSearchOrgApiService = Substitute.For<IEanSearchOrgApiService>();
        var options = Substitute.For<IOptions<EanSearchOrgConfiguration>>();
        var queryHandler = new MetadataByGtinQueryHandler(Substitute.For<ILogger<MetadataByGtinQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold, openFoodFactsApiService, eanSearchOrgApiService, options);

        // Act
        Metadata actual = await queryHandler.ExecuteAsync(new MetadataByGtinQuery(metadata.GlobalTradeItemNumber));

        // Assert
        actual.FoodFacts?.Brands.Should().Be(metadata.FoodFacts.Brands);
        await openFoodFactsApiService.DidNotReceive().GetProduct(Arg.Any<string>());
        await eanSearchOrgApiService.DidNotReceive().Lookup(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallOpenFoodFacts()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var openFoodFactsApiService = Substitute.For<IOpenFoodFactsApiService>();

        var product = new ProductResponse
        {
            Code = "GTIN",
            Status = 1,
            Product = new Product
            {
                Brands = "Unittest Brand"
            }
        };

        openFoodFactsApiService.GetProduct(Arg.Any<string>()).Returns(product);

        var eanSearchOrgApiService = Substitute.For<IEanSearchOrgApiService>();
        var options = Substitute.For<IOptions<EanSearchOrgConfiguration>>();
        var queryHandler = new MetadataByGtinQueryHandler(Substitute.For<ILogger<MetadataByGtinQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold, openFoodFactsApiService, eanSearchOrgApiService, options);

        // Act
        Metadata actual = await queryHandler.ExecuteAsync(new MetadataByGtinQuery("GTIN"));

        // Assert
        actual.FoodFacts?.Brands.Should().Be(product.Product.Brands);
        await openFoodFactsApiService.Received(1).GetProduct(Arg.Any<string>());
        await eanSearchOrgApiService.DidNotReceive().Lookup(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallEanSearchOrg()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var openFoodFactsApiService = Substitute.For<IOpenFoodFactsApiService>();
        var eanSearchOrgApiService = Substitute.For<IEanSearchOrgApiService>();
        var products = new List<NonFoodProduct>
        {
            new NonFoodProduct
            {
                Ean = "GTIN",
                Name = "UnitTest Product"
            }
        };
        eanSearchOrgApiService.Lookup(Arg.Any<string>(), Arg.Any<string>()).Returns(products);
        var options = Substitute.For<IOptions<EanSearchOrgConfiguration>>();
        options.Value.Returns(new EanSearchOrgConfiguration { Token = "Fake" });
        var queryHandler = new MetadataByGtinQueryHandler(Substitute.For<ILogger<MetadataByGtinQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold, openFoodFactsApiService, eanSearchOrgApiService, options);

        // Act
        Metadata actual = await queryHandler.ExecuteAsync(new MetadataByGtinQuery("GTIN"));

        // Assert
        actual.ProductFacts?.Name.Should().Be("UnitTest Product");
        await openFoodFactsApiService.Received(1).GetProduct(Arg.Any<string>());
        await eanSearchOrgApiService.Received(1).Lookup(Arg.Any<string>(), Arg.Any<string>());
    }
}
