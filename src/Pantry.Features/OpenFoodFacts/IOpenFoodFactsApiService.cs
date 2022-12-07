using System.Threading.Tasks;
using Pantry.Features.OpenFoodFacts.Responses;
using Refit;

namespace Pantry.Features.OpenFoodFacts;

[Headers(
    "Accept: application/json; charset=utf-8",
    "Accept-Encoding: gzip, deflate",
    "cache-control: no-cache",
    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0")]
public interface IOpenFoodFactsApiService
{
    [Get("/api/v2/product/{barcode}")]
    public Task<ProductResponse> GetProduct(string barcode);

    [Get("/api/v2/product/{barcode}")]
    public Task<string> GetProductAsString(string barcode);
}
