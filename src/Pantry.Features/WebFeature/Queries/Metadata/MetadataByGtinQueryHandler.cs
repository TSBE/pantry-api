using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.EanSearchOrg;
using Pantry.Features.EanSearchOrg.Configuration;
using Pantry.Features.OpenFoodFacts;
using Pantry.Features.WebFeature.Diagnostics;
using Refit;

namespace Pantry.Features.WebFeature.Queries;

public class MetadataByGtinQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<MetadataByGtinQueryHandler> _logger;

    private readonly IPrincipal _principal;

    private readonly IOpenFoodFactsApiService _openFoodFactsApiService;

    private readonly IEanSearchOrgApiService _eanSearchOrgApiService;

    private readonly EanSearchOrgConfiguration _eanSearchOrgConfiguration;

    public MetadataByGtinQueryHandler(
        ILogger<MetadataByGtinQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal,
        IOpenFoodFactsApiService openFoodFactsApiService,
        IEanSearchOrgApiService eanSearchOrgApiService,
        IOptions<EanSearchOrgConfiguration> eanSearchOrgConfiguration)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
        _openFoodFactsApiService = openFoodFactsApiService;
        _eanSearchOrgApiService = eanSearchOrgApiService;
        _eanSearchOrgConfiguration = eanSearchOrgConfiguration.Value;
    }

    public async Task<Metadata> ExecuteAsync(MetadataByGtinQuery query)
    {
        _logger.ExecutingQuery(nameof(MetadataByGtinQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var metadata = await appDbContext.Metadatas.FirstOrDefaultAsync(x => x.GlobalTradeItemNumber == query.GlobalTradeItemNumber);

        if (metadata is null)
        {
            metadata = new Metadata { GlobalTradeItemNumber = query.GlobalTradeItemNumber };
            try
            {
                _logger.ExecutingOpenFoodFacts(query.GlobalTradeItemNumber);
                var productResponse = await _openFoodFactsApiService.GetProduct(query.GlobalTradeItemNumber);
                if (productResponse is not null && productResponse.Status > 0)
                {
                    metadata.FoodFacts = productResponse.Product;
                }
            }
            catch (ApiException ex)
            {
                _logger.ExecutedOpenFoodFacts(ex.Message);
            }

            if (metadata.FoodFacts is null)
            {
                try
                {
                    _logger.ExecutingEanSearchOrg(query.GlobalTradeItemNumber);
                    var nonFoodResponse = await _eanSearchOrgApiService.Lookup(_eanSearchOrgConfiguration.Token, query.GlobalTradeItemNumber);
                    metadata.ProductFacts = nonFoodResponse.FirstOrDefault();
                }
                catch (ApiException ex)
                {
                    _logger.ExecutedEanSearchOrg(ex.Message);
                }
            }

            if (metadata.FoodFacts is not null || metadata.ProductFacts is not null)
            {
                appDbContext.Metadatas.Add(metadata);
                await appDbContext.SaveChangesAsync();
            }
        }

        return metadata;
    }
}
