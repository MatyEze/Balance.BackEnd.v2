using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.ActivosService;
using Balance.BackEnd.v2.Servicios.ActivosService.Modelos;
using Balance.BackEnd.v2.Servicios.DataCompletaService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

namespace Balance.BackEnd.v2.Servicios.DataCompletaService
{
    public class DataCompletaService : IDataCompletaService
    {
        private readonly IMovimientosService _movimientosService;
        private readonly IActivosService _activosService;
        private readonly ILogger<DataCompletaService> _logger;
        private readonly IMapper _mapper;
        private readonly ISupabaseDB _supabaseDB;

        public DataCompletaService(IMovimientosService movimientosService, IActivosService activosService, ILogger<DataCompletaService> logger, IMapper mapper, ISupabaseDB supabaseDB)
        {
            _movimientosService = movimientosService;
            _activosService = activosService;
            _logger = logger;
            _mapper = mapper;
            _supabaseDB = supabaseDB;
        }

        public async Task<DataCompleta> ArrayStringToDataCompleta(List<string> data, string brokerResourceKey, string idUsuario)
        {
            try
            {
                DataCompleta dataCompleta = new DataCompleta();
                List<Movimiento> movimientos;
                Activos activos;

                if (data.Count > 0)
                {
                    movimientos = await _movimientosService.ProcesarMovimientos(data, brokerResourceKey);
                    activos = await _activosService.GenerarActivos(movimientos);

                    dataCompleta.Activos = activos;
                    await _movimientosService.UploadMovimientos(movimientos, idUsuario);

                    dataCompleta.Movimientos = movimientos;
                }

                return dataCompleta;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar data completa: {ex}");
                throw new Exception($"Error al generar data completa: {ex}");
            }
        }

        public async Task<DataCompleta> GetDataCompletaDB(List<Movimiento> movimientosEnDbFalse, string idUsuario)
        {
            try
            {
                List<Movimiento> movimientos = await _movimientosService.GetMovimientosFromDB(movimientosEnDbFalse, idUsuario);
                Activos activos = await _activosService.GenerarActivos(movimientos);

                return new DataCompleta { Activos = activos, Movimientos = movimientos };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener dataCompleta desde DB: {ex}");
                throw new Exception($"Error al obtener dataCompleta desde DB: {ex}");
            }
        }

        
    }
}
