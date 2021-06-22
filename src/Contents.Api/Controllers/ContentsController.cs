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

        // GET: api/<ContentsController>
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

        // GET: api/<ContentsController>/<name>
        [HttpGet("{name}")]
        public ActionResult<ContensByOrderedName> GetByOrderedName(string name)
        {
            var result = _contentService.GetContentsByOrderedName(name);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result?.Data);
        }

        // POST api/<ContentsController>/<orderedName>
        [HttpPost("{orderedName}")]
        public async Task<ActionResult> PostAsync(string orderedName, [FromBody] OrderedContentPost model)
        {
            var result = await _contentService.InsertOrderedContentAsync(orderedName, model.Data);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Created($"/{orderedName}", null);
        }

        // PUT api/<ContentsController>/<orderedName>
        [HttpPut("{orderedName}")]
        public async Task<ActionResult> PutAsync(string orderedName, [FromBody] OrderedContentPost model)
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
