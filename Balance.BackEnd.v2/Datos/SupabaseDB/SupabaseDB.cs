using AutoMapper;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace Balance.BackEnd.v2.Datos.SupabaseDB
{
    public class SupabaseDB : ISupabaseDB
    {
        private readonly Supabase.Client _client;
        private readonly ILogger<SupabaseDB> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public SupabaseDB(IConfiguration configuration, ILogger<SupabaseDB> logger, IMemoryCache memoryCache, IMapper mapper)
        {
            Supabase.SupabaseOptions options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            string key = configuration["Supabase:Key"]!;
            string url = configuration["Supabase:Url"]!;
            _client = new Supabase.Client(url, key, options);
            
            _logger = logger;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }

        private void GuardarEnCache(string key, object valor, int minutosEnCache)
        {
            MemoryCacheEntryOptions entryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(minutosEnCache));

            _memoryCache.Set(key, valor, entryOptions);
        }


        public async Task<List<TipoTicketSPB>> GetTiposTicket()
        {
            var result = await _client.From<TipoTicketSPB>().Get();
            return result.Models;
        }

        public async Task<List<TipoMonedaSPB>> GetTiposMoneda()
        {
            var result = await _client.From<TipoMonedaSPB>().Get();
            return result.Models;
        }

        public async Task<List<TipoMovimientoSPB>> GetTiposMovimiento()
        {
            var result = await _client.From<TipoMovimientoSPB>().Get();
            return result.Models;
        }

        public async Task<BrokerSPB?> GetBrokerSPBByCabecera(string cabecera)
        {
            var result = await _client.From<BrokerSPB>()
                .Where(x => x.Cabecera == cabecera)
                .Get();

            return result.Model;
        }

        public async Task<BrokerSPB?> GetBrokerSPBByResourceKey(string resourceKey)
        {
            var result = await _client.From<BrokerSPB>()
                .Where(x => x.ResourceKey == resourceKey)
                .Get();

            return result.Model;
        }

        public async Task<List<TicketSPB>?> GetTicketByString(string ticketString)
        {
            try
            {
                if (ticketString.Equals(string.Empty))
                {
                    return null;
                }

                string cacheKey = $"key_GetTicketByString_{ticketString}";

                if (!_memoryCache.TryGetValue(cacheKey, out List<TicketSPB>? tickets))
                {
                    var result = await _client.From<TicketSPB>()
                        .Select("Id, Ticket, IdTipo, Descripcion, Tipo:IdTipo(Id, Tipo)")
                        .Where(x => x.Ticket == ticketString).Get();

                    tickets = result.Models;
                    if (tickets != null && tickets.Count > 0)
                    {
                        GuardarEnCache(cacheKey, tickets, 5);
                    }
                }

                return tickets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Ticket por String en DB");
                throw new Exception($"Error al obtener Ticket por String en DB: {ex}");
            }
        }

        public async Task<TicketSPB> InsertTicket(string ticketString, int idTipo, string descripcion)
        {
            try
            {
                var result = await _client.From<Insert_TicketSPB>().Insert(new Insert_TicketSPB { Ticket = ticketString, IdTipo = idTipo, Descripcion = descripcion });
                return _mapper.Map<TicketSPB>(result.Models[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al Insertar Ticket en DB");
                throw new Exception($"Error al Insertar Ticket en DB: {ex}");
            }
        }

        public async Task<string> InsertMovimentosSPB(List<MovimientoSPB> movimientosSPB)
        {
            try
            {
                var result = await _client.Rpc("BULK_INSERT_MOVIMIENTOS", new { movimientos_data = movimientosSPB });
                return result.Content ?? "0";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al Insertar Movimientos en DB");
                throw new Exception($"Error al Insertar Movimientos en DB: {ex}");
            }
        }

        public async Task<List<MovimientoSPB>> GetMovimientosSPB(string idUsuario)
        {
            try
            {
                var result = await _client.From<MovimientoSPB>()
                .Select("Id, NrMovimientoBroker, Cantidad, IdBroker, IdUsuario, IdTipoMovimiento, " +
                "IdTicket, FechaMovimiento, IdTipoPrecio, " +
                "Precio, IdTipoMontoTotal, " +
                "MontoTotal, EnDb, Observaciones, " +
                "TicketSPB:IdTicket(Id, Ticket, IdTipo, Descripcion, Tipo:IdTipo(Id, Tipo)), " +
                "BrokerSPB:IdBroker(Id, ResourceKey, Descripcion), " +
                "TipoMovimientoSPB:IdTipoMovimiento(Id, Tipo, Descripcion), " +
                "TipoPrecio:IdTipoPrecio(Id, Tipo, Descripcion, Simbolo), " +
                "TipoMontoTotal:IdTipoMontoTotal(Id, Tipo, Descripcion, Simbolo)")
                .Where(x => x.IdUsuario == idUsuario).Get();

                return result.Models;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Movimientos de DB");
                throw new Exception($"Error al obtener Movimientos de DB: {ex}");
            }
        }

        public async Task<int> InsertMovimientos(List<Movimiento> movimientos, string idUsuario)
        {
            try
            {
                List<MovimientoSPB> movimientosSPB = new List<MovimientoSPB>();
                foreach (Movimiento movimiento in movimientos)
                {
                    movimientosSPB.Add(new MovimientoSPB
                    {
                        NrMovimiento = movimiento.NroMovimiento,
                        IdBroker = movimiento.Broker.Id,
                        Cantidad = movimiento.Cantidad,
                        IdUsuario = idUsuario,
                        IdTipoMovimiento = movimiento.TipoMovimiento.Id,
                        IdTicket = movimiento.Ticket.Id,
                        IdTipoPrecio = movimiento.Precio.IdTipo,
                        Precio = movimiento.Precio.Cantidad,
                        IdTipoMontoTotal = movimiento.MontoTotal.IdTipo,
                        MontoTotal = movimiento.MontoTotal.Cantidad,
                        EnDb = true,
                        Observaciones = movimiento.Observaciones
                    });
                }

                var result = await _client.Rpc("BULK_INSERT_MOVIMIENTOS", new { movimientos_data = movimientosSPB });

                int.TryParse(result.Content, out int cantidadInsertados);

                return cantidadInsertados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al Insertar Movimientos en DB: {ex}");
            }
        }
    }
}
