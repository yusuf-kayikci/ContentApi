using System.Threading.Tasks;
using Contents.Api.Model.Request;
using Contents.Business.Abstraction;
using Contents.Business.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Contents.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var result = await _contentService.GetContentsAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPut("{contentId}")]
        public async Task<ActionResult> UpdateContentAsync(string contentId , [FromBody]ContentModel content)
        {
            var updateResult = await _contentService.UpdateContent(contentId, content);
            if (!updateResult.IsSuccess)
            {
                return BadRequest(updateResult.Message);
            }

            return NoContent();
        }


        [HttpGet("ordered/{orderedName}")]
        public ActionResult<ContensByOrderedName> GetByOrderedName(string orderedName)
        {
            var result = _contentService.GetContentsByOrderedName(orderedName);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result?.Data);
        }

        [HttpPost("ordered")]
        public async Task<ActionResult> PostOrderedContentAsync([FromBody] OrderedContentPost model)
        {
            var result = await _contentService.InsertOrderedContentAsync(model.Name, model.Data);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Created($"/{model.Name}", null);
        }

        [HttpPut("ordered/{orderedName}")]
        public async Task<ActionResult> PutOrderedContentAsync(string orderedName, [FromBody] OrderedContentPost model)
        {
            var result = await _contentService.UpdateOrderedContentAsync(orderedName, model.Data);
            if(!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return NoContent();
        }
    }
}
