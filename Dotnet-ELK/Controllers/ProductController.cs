using DotneteLK.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace DotneteLK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IElasticClient elasticClient, ILogger<ProductController> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        [HttpGet(Name = "GetProducts")]
        public async Task<IActionResult> GetProducts(string keyWord)
        {
            var result = await _elasticClient.SearchAsync<Product>(
                s => s.Query(
                    q => q.QueryString(
                        d => d.Query('*' + keyWord + '*')
                        )).Size(1000)
                );

            return Ok(result.Documents.ToList());
        }


        [HttpPost(Name = "AddProduct")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var result = await _elasticClient.IndexDocumentAsync(product);
            return result.IsValid ? Ok() : BadRequest();
        }
    }
}
