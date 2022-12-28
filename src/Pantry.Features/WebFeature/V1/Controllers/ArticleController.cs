using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pantry.Features.WebFeature.Commands;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers.Mappers;
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
    [HttpGet("{articleId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleResponse))]
    public async Task<IActionResult> GetArticleByIdAsync(long articleId)
    {
        ArticleResponse article = (await _queryPublisher.ExecuteAsync(new ArticleByIdQuery(articleId))).ToDtoNotNull();
        return Ok(article);
    }

    /// <summary>
    /// Get articles.
    /// </summary>
    /// <returns>List of articles.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleListResponse))]
    public async Task<IActionResult> GetAllArticlesAsync()
    {
        IEnumerable<ArticleResponse> articles = (await _queryPublisher.ExecuteAsync(new ArticleListQuery())).ToDtos();
        return Ok(new ArticleListResponse { Articles = articles });
    }

    /// <summary>
    /// Creates article.
    /// </summary>
    /// <returns>article.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateArticleAsync([FromBody] ArticleRequest articleRequest)
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
        return Ok(article);
    }

    /// <summary>
    /// Update article.
    /// </summary>
    /// <returns>article.</returns>
    [HttpPut("{articleId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticleResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateArticleAsync([FromBody] ArticleRequest articleRequest, long articleId)
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
        return Ok(article);
    }

    /// <summary>
    ///  Deletes article.
    /// </summary>
    /// <returns>no content.</returns>
    [HttpDelete("{articleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteArticleAsync(long articleId)
    {
        await _commandPublisher.ExecuteAsync(new DeleteArticleCommand(articleId));
        return NoContent();
    }
}
