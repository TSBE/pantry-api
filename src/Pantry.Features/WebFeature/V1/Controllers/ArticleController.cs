using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pantry.Features.WebFeature.Commands;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers.Extensions;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Silverback.Messaging.Publishing;

namespace Pantry.Features.WebFeature.V1.Controllers;

[Route("api/v{version:apiVersion}/articles")]
[ApiController]
public class ArticleController : ControllerBase
{
    private readonly ICommandPublisher _commandPublisher;

    private readonly IQueryPublisher _queryPublisher;

    public ArticleController(IQueryPublisher queryPublisher, ICommandPublisher commandPublisher)
    {
        _queryPublisher = queryPublisher;
        _commandPublisher = commandPublisher;
    }

    /// <summary>
    /// Get article.
    /// </summary>
    /// <returns>returns logged in users article.</returns>
    [HttpGet("{articleId:long}")]
    public async Task<Results<Ok<ArticleResponse>, BadRequest>> GetArticleByIdAsync(long articleId)
    {
        ArticleResponse article = (await _queryPublisher.ExecuteAsync(new ArticleByIdQuery(articleId))).ToDtoNotNull();
        return TypedResults.Ok(article);
    }

    /// <summary>
    /// Get articles.
    /// </summary>
    /// <returns>List of articles.</returns>
    [HttpGet]
    public async Task<Results<Ok<ArticleListResponse>, BadRequest>> GetAllArticlesAsync()
    {
        IEnumerable<ArticleResponse> articles = (await _queryPublisher.ExecuteAsync(new ArticleListQuery())).ToDtos();
        return TypedResults.Ok(new ArticleListResponse { Articles = articles });
    }

    /// <summary>
    /// Creates article.
    /// </summary>
    /// <returns>article.</returns>
    [HttpPost]
    public async Task<Results<Ok<ArticleResponse>, BadRequest>> CreateArticleAsync([FromBody] ArticleRequest articleRequest)
    {
        ArticleResponse article = (await _commandPublisher.ExecuteAsync(
            new CreateArticleCommand(
                articleRequest.StorageLocationId,
                articleRequest.GlobalTradeItemNumber,
                articleRequest.Name,
                articleRequest.BestBeforeDate,
                articleRequest.Quantity,
                articleRequest.Content,
                articleRequest.ContentType.ToModelNotNull()))).ToDtoNotNull();
        return TypedResults.Ok(article);
    }

    /// <summary>
    /// Update article.
    /// </summary>
    /// <returns>article.</returns>
    [HttpPut("{articleId:long}")]
    public async Task<Results<Ok<ArticleResponse>, BadRequest>> UpdateArticleAsync([FromBody] ArticleRequest articleRequest, long articleId)
    {
        ArticleResponse article = (await _commandPublisher.ExecuteAsync(
            new UpdateArticleCommand(
                articleId,
                articleRequest.StorageLocationId,
                articleRequest.GlobalTradeItemNumber,
                articleRequest.Name,
                articleRequest.BestBeforeDate,
                articleRequest.Quantity,
                articleRequest.Content,
                articleRequest.ContentType.ToModelNotNull()))).ToDtoNotNull();
        return TypedResults.Ok(article);
    }

    /// <summary>
    ///  Deletes article.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{articleId:long}")]
    public async Task<Results<NoContent, BadRequest>> DeleteArticleAsync(long articleId)
    {
        await _commandPublisher.ExecuteAsync(new DeleteArticleCommand(articleId));
        return TypedResults.NoContent();
    }
}
