using Balance.BackEnd.v2.Datos.SupabaseDB;
using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Helpers;
using System.Globalization;
using AutoMapper;
using Balance.BackEnd.v2.Servicios.AnalisisTecnicoService;
using Balance.BackEnd.v2.Servicios.AnalisisTecnicoService.Modelos;

namespace Balance.BackEnd.v2.Servicios.MovimientosService
{
    public class MovimientosService : IMovimientosService
    {
        private readonly ILogger<MovimientosService> _logger;
        private readonly ISupabaseDB _supabaseDB;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IAnalisisTecnicoService _analisisTecnicoService;

        public MovimientosService(ILogger<MovimientosService> logger, ISupabaseDB supabaseDB, IConfiguration configuration, IMapper mapper, IAnalisisTecnicoService analisisTecnicoService)
        {
            _logger = logger;
            _supabaseDB = supabaseDB;
            _configuration = configuration;
            _mapper = mapper;
            _analisisTecnicoService = analisisTecnicoService;
        }

        public async Task<List<Movimiento>> ProcesarMovimientos(List<string> data, string brokerResourceKey)
        {
            BrokerSPB? brokerSPB = await _supabaseDB.GetBrokerSPBByResourceKey(brokerResourceKey);

            if (brokerSPB == null)
            {
                _logger.LogError($"El broker: {brokerResourceKey} es invalido.");
                throw new Exception($"El broker: {brokerResourceKey} es invalido.");
            }

            switch (brokerSPB.ResourceKey)
            {
                case "IOL":
                    return await ProcesarMovimientosIOL(data, brokerSPB);
                default:
                    _logger.LogError($"El procesamiento de datos para el broker {brokerResourceKey} no esta implementado.");
                    throw new Exception($"El procesamiento de datos para el broker {brokerResourceKey} no esta implementado.");
            }
        }

        private async Task<List<Movimiento>> ProcesarMovimientosIOL(List<string> data, BrokerSPB brokerSPB)
        {
            List<Movimiento> movimientos = new List<Movimiento>();
            List<MovimientoSPB> movimientosToUpload = new List<MovimientoSPB>();
            CultureInfo culture = new CultureInfo("es-ES");
            List<TipoMonedaSPB> tiposMonedaSPB = await _supabaseDB.GetTiposMoneda();
            List<TipoMovimientoSPB> tiposMovimientoSPB = await _supabaseDB.GetTiposMovimiento();

            data.RemoveAt(0); // elimino la cabecera

            foreach (var item in data)
            {
                List<string> linea = new List<string>(item.Split(new[] { " | " }, StringSplitOptions.None));
                Movimiento movimiento = new Movimiento() { Observaciones = new List<string>() };

                //Se obtiene la moneda utilizada en el movimiento
                TipoMonedaSPB tipoMoneda = IOLHelper.ObtenerTipoMonedaIOL(linea[13], tiposMonedaSPB);

                //Armado de movimiento
                movimiento.NroMovimiento = int.Parse(linea[0]);
                movimiento.TipoMovimiento = IOLHelper.ObtenerTipoMovimientoIOL(linea[2], tiposMovimientoSPB);
                await ProcesarTicket(IOLHelper.ProcesarTicketIOL(linea[2]), movimiento);


                movimiento.Broker = new Broker { Id = brokerSPB.Id, Descripcion = brokerSPB.Descripcion, ResourceKey = brokerSPB.ResourceKey };

                //movimiento.Cantidad = int.Parse(linea[6]);
                //Fecha es requerido para poder calcular bien la cantidad, por los splits
                ProcesarFecha(movimiento, linea[3], culture);

                ProcesarCantidad(movimiento, linea[6]);
                ProcesarPrecio(movimiento, linea[7], culture, tipoMoneda);
                ProcesarMontoTotal(movimiento, linea[11], culture, tipoMoneda);

                //PostProcesamiento(movimiento);

                //idUsuario = "0" es que no se habilito desde el front para subir a db
                //Tambien se verifica que el movimiento pueda ser subido a db
                //if (idUsuario != "0" && movimiento.PermitirDb)
                //{
                //    //movimientosToUpload.Add(movimiento.ToMovimientoSPB(idUsuario, true));
                //    MovimientoSPB movimientoSPB = _mapper.Map<MovimientoSPB>(movimiento);

                //    movimientoSPB.EnDb = true;
                //    movimiento.EnDb = true;
                //}

                movimientos.Add(movimiento);
            }

            //if (movimientosToUpload.Count > 0)
            //{
            //    await _supabaseDB.InsertMovimentosSPB(movimientosToUpload);
            //}
            movimientos = IOLHelper.ProcesarDolarMEP(movimientos, tiposMovimientoSPB);
            return movimientos;
        }

        private void ProcesarCantidad(Movimiento movimiento, string cantidad)
        {
            try
            {
                if (int.TryParse(cantidad, out int result))
                {
                    movimiento.Cantidad = result;
                }
                else
                {
                    movimiento.Cantidad = 0;
                    movimiento.Observaciones.Add($"No se pudo procesar la cantidad: {cantidad}");
                }

                if (movimiento.Cantidad > 0 && movimiento.Ticket.TicketString != string.Empty)
                {
                    //TODO. Agregar un servicio usando alphavantage.co para obtener los splits de los tickets y remplazar esta parte del codigo
                    if (movimiento.Ticket.TicketString == "NVDA")
                    {
                        if (movimiento.FechaMovimiento <= new DateTime(2024, 6, 7))
                        {
                            movimiento.Observaciones.Add($"SPLIT NVDA del 07/06/2024 - Se multiplica la cantidad por 10 cantidad original: [{movimiento.Cantidad}]");
                            movimiento.Cantidad *= 10; //el split de esta fecha fue de x10
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al Procesar la cantidad del movimiento cantidadString: {cantidad} | {ex}");
                throw new Exception("Error al Procesar la cantidad del movimiento");
            }
        }

        private void ProcesarMontoTotal(Movimiento movimiento, string monto, CultureInfo culture, TipoMonedaSPB tipoMoneda)
        {
            if (decimal.TryParse(monto, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, culture, out decimal result))
            {
                movimiento.MontoTotal = new Moneda
                {
                    IdTipo = tipoMoneda.Id,
                    Tipo = tipoMoneda.Tipo,
                    Simbolo = tipoMoneda.Simbolo,
                    Cantidad = result,
                    Descripcion = tipoMoneda.Descripcion
                };
            }
            else
            {
                movimiento.MontoTotal = new Moneda
                {
                    IdTipo = tipoMoneda.Id,
                    Tipo = tipoMoneda.Tipo,
                    Simbolo = tipoMoneda.Simbolo,
                    Cantidad = 0,
                    Descripcion = tipoMoneda.Descripcion
                };
                movimiento.Observaciones.Add($"Error al leer el monto total: valor leido [{monto}]");
            }
        }

        private void ProcesarPrecio(Movimiento movimiento, string precio, CultureInfo culture, TipoMonedaSPB tipoMoneda)
        {
            if (decimal.TryParse(precio, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign, culture, out decimal result))
            {
                movimiento.Precio = new Moneda
                {
                    IdTipo = tipoMoneda.Id,
                    Tipo = tipoMoneda.Tipo,
                    Simbolo = tipoMoneda.Simbolo,
                    Cantidad = result,
                    Descripcion = tipoMoneda.Descripcion
                };
            }
            else
            {
                movimiento.Precio = new Moneda
                {
                    IdTipo = tipoMoneda.Id,
                    Tipo = tipoMoneda.Tipo,
                    Simbolo = tipoMoneda.Simbolo,
                    Cantidad = 0,
                    Descripcion = tipoMoneda.Descripcion
                };
                movimiento.Observaciones.Add($"Error al leer el precio: valor leido [{precio}]");
            }
        }

        private void ProcesarFecha(Movimiento movimiento, string fecha, CultureInfo culture)
        {
            DateTime fechaMovimiento;
            // Intentar convertir la fecha
            if (!DateTime.TryParse(fecha, culture, DateTimeStyles.None, out fechaMovimiento))
            {
                // Si el parsing falla, intenta tratarlo como número de serie de Excel
                if (int.TryParse(fecha, out int excelSerialDate))
                {
                    // Convertir el número de serie a fecha
                    DateTime baseDate = new DateTime(1900, 1, 1);
                    fechaMovimiento = baseDate.AddDays(excelSerialDate - 2); // Ajustar por error bisiesto de Excel
                    movimiento.Observaciones.Add("Fecha procesada formato excel");
                }
                else
                {
                    // Manejar el error o establecer un valor predeterminado
                    fechaMovimiento = DateTime.MinValue; // Valor predeterminado o manejar error
                    movimiento.Observaciones.Add("Error al procesar fecha");
                }
            }
            movimiento.FechaMovimiento = fechaMovimiento;
        }

        private async Task ProcesarTicket(string ticketString, Movimiento movimiento)
        {
            //Se trata de obtener desde DB  
            Ticket? ticket = await GetTicketFromDB(ticketString, movimiento);

            if (ticket != null)
            {
                movimiento.Ticket = ticket;
                return;
            }

            //TODO. Migrar analisisTecnicoService para poder descomentar esto
            if (_configuration.GetValue<bool>("InsertarTicketsNoIdentificados"))
            {
                //Si no esta en DB consultamos a AnalisisTecnico

                List<TipoTicketSPB> tiposTicketSPB = await _supabaseDB.GetTiposTicket();

                //Primero checkeamos si es un CEDEAR - se asume que no son Acciones no Argentinas
                //TODO. Mas adelante ver como manejar acciones no Argentinas
                TicketInfoo? ticketInfooCDEAR = await _analisisTecnicoService.TicketInfoCdear(ticketString);

                if (ticketInfooCDEAR != null)
                {
                    TicketSPB respuesta = await _supabaseDB.InsertTicket(ticketString, tiposTicketSPB.Where(x => x.Tipo == "CEDEAR").First().Id, ticketInfooCDEAR.Descripcion);
                    movimiento.Ticket = new Ticket { Id = respuesta.Id, IdTipo = respuesta.IdTipo, TicketString = respuesta.Ticket, Tipo = "CEDEAR" };
                }

                //Si no es CEDEAR se asume que es una Accion Argentina
                TicketInfoo? ticketInfooAccionArg = await _analisisTecnicoService.TicketInfoAccion(ticketString);

                if (ticketInfooAccionArg != null && ticketInfooAccionArg.Mercado.Equals("BCBA"))
                {
                    TicketSPB respuesta = await _supabaseDB.InsertTicket(ticketString, tiposTicketSPB.Where(x => x.Tipo == "ACCION_ARG").First().Id, ticketInfooAccionArg.Descripcion);
                    movimiento.Ticket = new Ticket { Id = respuesta.Id, IdTipo = respuesta.IdTipo, TicketString = respuesta.Ticket, Tipo = "ACCION_ARG" };
                }
            }

            //TODO. Identificar fondo en https://fondosonline.com
            //usando https://fondosonline.com/Operations/Funds/GetFundsProducts?sortColumn=MonthPercent&isAscending=false&PageSize=1000&searchFundName=&searchCurrency=-1&searchFocus=-1&searchStrategy=&searchHorizon=-1&searchProfile=-1&isActive=false&searchMinInvestment=
            //talvez que esto se corra 1 vez al dia y actualize la db para no pegarle cada rato a esta url ?

            if (movimiento.Ticket == null)
            {
                //No se identifico el ticket
                movimiento.Ticket = new Ticket { Id = 0, IdTipo = 0, TicketString = ticketString, Tipo = "Undefined" };
                movimiento.Observaciones.Add($"Este movimiento no puede ser subido a la nube porque no se pudo identificar el ticket: {ticketString}");
                movimiento.PermitirDb = false;
            }
        }

        public async Task UploadMovimientos(List<Movimiento> movimientos, string idUsuario)
        {
            if (idUsuario != "0" && movimientos.Count > 0)
            {
                List<MovimientoSPB> movimientosUpload = new List<MovimientoSPB>();
                foreach (Movimiento movimiento in movimientos)
                {
                    if (movimiento.PermitirDb)
                    {
                        MovimientoSPB movimientoSPB = _mapper.Map<MovimientoSPB>(movimiento);

                        movimientoSPB.EnDb = true;
                        movimientoSPB.IdUsuario = idUsuario;
                        movimiento.EnDb = true;

                        movimientosUpload.Add(movimientoSPB);
                    }
                }

                if (movimientosUpload.Count > 0)
                {
                    await _supabaseDB.InsertMovimentosSPB(movimientosUpload);
                }
            }
        }

        public List<Movimiento> ConcatenarMovimientos(List<Movimiento> movimientosOld, List<Movimiento> movimientosNew)
        {
            foreach (Movimiento newMov in movimientosNew)
            {
                if (movimientosOld.Any(x => x.NroMovimiento.Equals(newMov.NroMovimiento) 
                && x.Broker.ResourceKey.Equals(newMov.Broker.ResourceKey) 
                && x.TipoMovimiento.Tipo.Equals(newMov.TipoMovimiento.Tipo)))
                {
                    continue;
                }
                movimientosOld.Add(newMov);
            }
            return movimientosOld;
        }

        public async Task<List<Movimiento>> GetMovimientosFromDB(List<Movimiento> movimientosEnDbFalse, string idUsuario)
        {
            List<MovimientoSPB> movimientosSPB = await _supabaseDB.GetMovimientosSPB(idUsuario);

            if (movimientosSPB.Count > 0)
            {
                foreach (MovimientoSPB movSPB in movimientosSPB)
                {
                    if (movimientosEnDbFalse.Any(x => x.NroMovimiento.Equals(movSPB.NrMovimiento)
                    && x.Broker.ResourceKey.Equals(movSPB.BrokerSPB.ResourceKey)
                    && x.TipoMovimiento.Tipo.Equals(movSPB.TipoMovimientoSPB.Tipo)))
                    {
                        continue;
                    }

                    movimientosEnDbFalse.Add(new Movimiento
                    {
                        NroMovimiento = movSPB.NrMovimiento,
                        Broker = new Broker
                        {
                            Id = movSPB.BrokerSPB.Id,
                            Descripcion = movSPB.BrokerSPB.Descripcion,
                            ResourceKey = movSPB.BrokerSPB.ResourceKey
                        },
                        Ticket = new Ticket
                        {
                            Id = movSPB.TicketSPB?.Id,
                            TicketString = movSPB.TicketSPB?.Ticket,
                            IdTipo = movSPB.TicketSPB?.IdTipo,
                            Tipo = movSPB.TicketSPB?.Tipo.Tipo,
                            Descripcion = movSPB.TicketSPB?.Descripcion
                        },
                        TipoMovimiento = new TipoMovimiento
                        {
                            Id = movSPB.TipoMovimientoSPB.Id,
                            Tipo = movSPB.TipoMovimientoSPB.Tipo,
                            Descripcion = movSPB.TipoMovimientoSPB.Descripcion
                        },
                        FechaMovimiento = movSPB.FechaMovimiento,
                        Cantidad = movSPB.Cantidad,
                        Precio = new Moneda
                        {
                            IdTipo = movSPB.TipoPrecio.Id,
                            Tipo = movSPB.TipoPrecio.Tipo,
                            Simbolo = movSPB.TipoPrecio.Simbolo,
                            Descripcion = movSPB.TipoPrecio.Descripcion,
                            Cantidad = movSPB.Precio
                        },
                        MontoTotal = new Moneda
                        {
                            IdTipo = movSPB.TipoMontoTotal.Id,
                            Tipo = movSPB.TipoMontoTotal.Tipo,
                            Simbolo = movSPB.TipoMontoTotal.Simbolo,
                            Descripcion = movSPB.TipoMontoTotal.Descripcion,
                            Cantidad = movSPB.MontoTotal
                        },
                        Observaciones = movSPB.Observaciones,
                        EnDb = movSPB.EnDb,
                        PermitirDb = true

                    });
                }
            }

            //Marcar enDb true a los que estan en DB
            foreach (Movimiento movimiento in movimientosEnDbFalse)
            {
                if (movimientosSPB.Any(x => x.NrMovimiento == movimiento.NroMovimiento && x.BrokerSPB.ResourceKey == movimiento.Broker.ResourceKey))
                {
                    movimiento.EnDb = true;
                }
            }

            return movimientosEnDbFalse;
        }

        private async Task<Ticket?> GetTicketFromDB(string ticketString, Movimiento movimiento)
        {
            try
            {
                List<TicketSPB> ticketSPB = await _supabaseDB.GetTicketByString(ticketString);

                if (ticketSPB == null || ticketSPB.Count == 0)
                {
                    return null;
                }

                if (ticketSPB.Count > 1)
                {
                    //TODO. Ver como resolver esto talvez se daja undefined y se agerga una accion al movimiento ? para que desde el front se elija el tipo
                    movimiento.Observaciones.Add($"El ticket: {ticketString} tiene varios tipos en DB y no ha logrado identificar a cual se esta referenciando, esto no permite subir el movimiento a la nube");
                    movimiento.PermitirDb = false;
                    return new Ticket { Id = 0, IdTipo = 0, TicketString = ticketString, Tipo = "Undefined" };
                }
                else
                {
                    return new Ticket
                    {
                        Id = ticketSPB[0].Id,
                        IdTipo = ticketSPB[0].IdTipo,
                        TicketString = ticketSPB[0].Ticket,
                        Tipo = ticketSPB[0].Tipo.Tipo,
                        Descripcion = ticketSPB[0].Descripcion,
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar ticket desde DB: {ex}");
            }
        }
    }
}
