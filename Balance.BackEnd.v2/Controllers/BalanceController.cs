using Balance.BackEnd.v2.Datos.SupabaseDB;
using Microsoft.AspNetCore.Mvc;

namespace Balance.BackEnd.v2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BalanceController : ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly ISupabaseDB _supabaseDB;

        public BalanceController(ISupabaseDB supabaseDB, ILogger<BalanceController> logger)
        {
            _supabaseDB = supabaseDB;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> test(string idUsuario)
        {
            try
            {
                var result = await _supabaseDB.GetMovimientosSPB(idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
