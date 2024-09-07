using Balance.BackEnd.v2.Datos.SupabaseDB;
using Microsoft.AspNetCore.Mvc;

namespace Balance.BackEnd.v2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BalanceController : ControllerBase
    {
        private readonly ISupabaseDB _supabaseDB;

        public BalanceController(ISupabaseDB supabaseDB)
        {
            _supabaseDB = supabaseDB;
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
