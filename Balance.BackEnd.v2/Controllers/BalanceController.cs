using AutoMapper;
using Balance.BackEnd.v2.Controllers.RequestModels;
using Balance.BackEnd.v2.Datos.SupabaseDB;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.ActivosService;
using Balance.BackEnd.v2.Servicios.ActivosService.Modelos;
using Balance.BackEnd.v2.Servicios.DataCompletaService;
using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Balance.BackEnd.v2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BalanceController : ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly ISupabaseDB _supabaseDB;
        private readonly IDataCompletaService _dataCompletaService;
        private readonly IMapper _mapper;
        private readonly IActivosService _activivosService;

        public BalanceController(ILogger<BalanceController> logger, ISupabaseDB supabaseDB, IDataCompletaService dataCompletaService, IMapper mapper, IActivosService activivosService)
        {
            _logger = logger;
            _supabaseDB = supabaseDB;
            _dataCompletaService = dataCompletaService;
            _mapper = mapper;
            _activivosService = activivosService;
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

        [HttpPost]
        public async Task<IActionResult> AgregarMovimientosADataCompleta(AgregarDataCompletaRequest request)
        {
            try
            {
                request.DataCompletaCargada.Movimientos.Concat(request.DataCompletaNueva.Movimientos);
                Activos activosNuevos = await _activivosService.GenerarActivos(request.DataCompletaCargada.Movimientos);
                request.DataCompletaCargada.Activos = activosNuevos;

                return Ok(request.DataCompletaCargada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBrokerFromArrayString(List<string> datos)
        {
            try
            {
                BrokerSPB? brokerSPB = await _supabaseDB.GetBrokerSPBByCabecera(datos[0]);
                Broker broker = _mapper.Map<Broker>(brokerSPB);
                return Ok(broker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Route("{idUsuario}")]
        public async Task<IActionResult> InsertMovimientos([FromBody] List<Movimiento> movimientos, [FromRoute] string idUsuario)
        {
            try
            {
                var result = await _supabaseDB.InsertMovimientos(movimientos, idUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Route("{idUsuario}")]
        public async Task<IActionResult> GetDataCompletaDB([FromBody] List<Movimiento> movimientosEnDbFalse, [FromRoute] string idUsuario)
        {
            try
            {
                DataCompleta data = await _dataCompletaService.GetDataCompletaDB(movimientosEnDbFalse, idUsuario);
                return Ok(data);
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
