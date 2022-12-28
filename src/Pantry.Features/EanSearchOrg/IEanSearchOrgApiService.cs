using System.Collections.Generic;
using System.Threading.Tasks;
using Pantry.Core.Models.EanSearchOrg;
using Refit;

namespace Pantry.Features.EanSearchOrg;

[Headers(
    "Accept: application/json; charset=utf-8",
    "Accept-Encoding: gzip, deflate",
    "cache-control: no-cache")]
public interface IEanSearchOrgApiService
{
    [Get("/api?op=barcode-lookup&format=json&language=3")]
    public Task<IEnumerable<NonFoodProduct>> Lookup([Query] string token, [Query] string ean);

    [Get("/api?op=barcode-lookup&format=json&language=3")]
    public Task<string> LookupAsString([Query] string token, [Query] string ean);
}
