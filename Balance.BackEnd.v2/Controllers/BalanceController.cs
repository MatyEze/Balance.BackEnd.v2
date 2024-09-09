using Balance.BackEnd.v2.Datos.SupabaseDB;
using Balance.BackEnd.v2.Servicios.DataCompletaService;
using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Balance.BackEnd.v2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BalanceController : ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly ISupabaseDB _supabaseDB;
        private readonly IDataCompletaService _dataCompletaService;

        public BalanceController(ILogger<BalanceController> logger, ISupabaseDB supabaseDB, IDataCompletaService dataCompletaService)
        {
            _logger = logger;
            _supabaseDB = supabaseDB;
            _dataCompletaService = dataCompletaService;
        }

        [HttpPost]
        [Route("{idUsuario}/ResourceKeyBroker/{resourceKeyBroker}")]
        public async Task<IActionResult> UploadRawArrayStringData([FromBody] List<string> datos, [FromRoute] string resourceKeyBroker, [FromRoute] string? idUsuario)
        {
            try
            {
                DataCompleta dataCompleta = await _dataCompletaService.ArrayStringToDataCompleta(datos, resourceKeyBroker, idUsuario ?? "0");
                return Ok(dataCompleta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
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
