using Microsoft.AspNetCore.Mvc;
using Quiniela.Data;
using Quiniela.Models;

namespace Quiniela.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;

        public RankingController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking()
        {
            try
            {
                var ranking = await _databaseService.ExecuteStoredProcedure<Ranking>("quiniela.SP_ObtenerRanking");
                return Ok(ranking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex });
            }
        }
    }
}