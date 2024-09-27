using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;
using System.Collections.Generic;

namespace Balance.BackEnd.v2.Servicios.MovimientosService.Helpers
{
    public static class IOLHelper
    {
        public static TipoMonedaSPB ObtenerTipoMonedaIOL(string tipoCuenta, List<TipoMonedaSPB> tiposMonedaSPB)
        {
            switch (tipoCuenta)
            {
                case "Inversion Argentina Pesos":
                    return tiposMonedaSPB.Single(x => x.Tipo == "PESO_ARG");
                case "Inversion Argentina Dolares":
                    return tiposMonedaSPB.Single(x => x.Tipo == "DOLAR_USD");
                default:
                    throw new ArgumentException($"No se pudo identifcar el tipo de moneda del siguiente campo: {tipoCuenta}");
            }
        }

        public static TipoMovimiento ObtenerTipoMovimientoIOL(string tipoMovimiento, List<TipoMovimientoSPB> tiposMovimiento)
        {
            TipoMovimientoSPB tipoMovimientoSPB;
            string tipoMovLimpio = LimpiarTipoMovimiento(tipoMovimiento);
            switch (tipoMovLimpio)
            {
                case "Compra":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "COMPRA");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Venta":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "VENTA");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Dep&#243;sito de Fondos":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "DEPOSITO");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Cr&#233;dito":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "CREDITO");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Pago de Dividendos":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "PAGO_DIVIDENDOS");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Suscripci&#243;n FCI":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "SUSCRIPCION_FONDO");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Liquidaci&#243;n de Cauci&#243;n":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "CAUCION_LIQUIDACION");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Cauci&#243;n":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "CAUCION");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Extracci&#243;n de Fondos":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "EXTRACCION");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                case "Rescate FCI":
                    tipoMovimientoSPB = tiposMovimiento.Single(x => x.Tipo == "RESCATE_FONDO");
                    return new TipoMovimiento { Id = tipoMovimientoSPB.Id, Tipo = tipoMovimientoSPB.Tipo, Descripcion = tipoMovimientoSPB.Descripcion };
                default:
                    throw new ArgumentException($"No se pudo identifcar el tipo de movimiento: {tipoMovimiento}");
            }
        }

        public static string ProcesarTicketIOL(string dato)
        {
            // Encontrar la posición del paréntesis de apertura '(' y del paréntesis de cierre ')'
            int startIndex = dato.IndexOf('(');
            int endIndex = dato.IndexOf(')');

            // Verificar si ambos paréntesis fueron encontrados y están en el orden correcto
            if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
            {
                // Obtener la subcadena que está dentro de los paréntesis
                string result = dato.Substring(startIndex + 1, endIndex - startIndex - 1);
                return result;
            }
            else
            {
                // Si los paréntesis no se encuentran en el orden correcto o están ausentes
                return string.Empty;
            }
        }

        public static List<Movimiento> ProcesarDolarMEP(List<Movimiento> movimientos, List<TipoMovimientoSPB> tipoMovimientoSPBs)
        {
            List<Movimiento> retorno = movimientos;
            TipoMovimientoSPB? comisionSPB =  tipoMovimientoSPBs
                .Where(t => t.Tipo == "COMISION_MEP")
                .FirstOrDefault();
            TipoMovimientoSPB? dolarMEPSPB = tipoMovimientoSPBs
                .Where(t => t.Tipo == "DOLAR_MEP")
                .FirstOrDefault();

            if (comisionSPB == null || dolarMEPSPB == null)
            {
                throw new Exception("Uno o mas de los tipos de movimientos dolar MEP no estan en DB");
            }

            List<Movimiento> movimientosDeOperacionDolarMEP = movimientos
                .FindAll(m => m.Broker.ResourceKey == "IOL" && m.Ticket.TicketString == "AL30D")
                .GroupBy(m => m.NroMovimiento)
                .Where(g => g.Count() > 1) // Solo grupos con más de un elemento
                .SelectMany(g => g) // Seleccionar todos los movimientos repetidos
                .ToList();

            int cantidadAL30D = movimientosDeOperacionDolarMEP[0].Cantidad;
            DateTime fechaOperacionDolarMEP = movimientosDeOperacionDolarMEP[0].FechaMovimiento;

            if (movimientosDeOperacionDolarMEP != null && movimientosDeOperacionDolarMEP.Count > 0)
            {
                Movimiento? compraAL30 = movimientos
                    .Where(m => m.Broker.ResourceKey == "IOL" &&
                                m.Ticket.TicketString == "AL30" &&
                                m.Cantidad == cantidadAL30D &&
                                m.FechaMovimiento <= fechaOperacionDolarMEP &&
                                m.FechaMovimiento >= fechaOperacionDolarMEP.AddDays(-3))
                    .FirstOrDefault();

                if (compraAL30 != null)
                {
                    Movimiento? ventaDolarAL30D = movimientosDeOperacionDolarMEP
                        .Where(m => m.MontoTotal.Tipo == "DOLAR_USD")
                        .FirstOrDefault();

                    Movimiento? comisionDolarMEP = movimientosDeOperacionDolarMEP
                        .Where(m => m.MontoTotal.Tipo == "PESO_ARG")
                        .FirstOrDefault();

                    movimientosDeOperacionDolarMEP.Add(compraAL30);

                    Movimiento mepVenta = new Movimiento
                    {
                        NroMovimiento = ventaDolarAL30D.NroMovimiento,
                        Broker = ventaDolarAL30D.Broker,
                        Ticket = new Ticket { Id = null }, //TODO. sacer el tipo de ticket dolarmep de db por parametro
                        TipoMovimiento = new TipoMovimiento { Id = dolarMEPSPB.Id, Tipo = dolarMEPSPB.Tipo, Descripcion = dolarMEPSPB.Descripcion},
                        FechaMovimiento = ventaDolarAL30D.FechaMovimiento,
                        Cantidad = ventaDolarAL30D.Cantidad,
                        Precio = ventaDolarAL30D.Precio,
                        MontoTotal = ventaDolarAL30D.MontoTotal,
                        Observaciones = ventaDolarAL30D.Observaciones,
                        EnDb = ventaDolarAL30D.EnDb,
                        PermitirDb = true
                    };

                    Movimiento mepCompra = new Movimiento
                    {
                        NroMovimiento = compraAL30.NroMovimiento,
                        Broker = compraAL30.Broker,
                        Ticket = new Ticket { Id = null }, //TODO. sacer el tipo de ticket dolarmep de db por parametro
                        TipoMovimiento = new TipoMovimiento { Id = dolarMEPSPB.Id, Tipo = dolarMEPSPB.Tipo, Descripcion = dolarMEPSPB.Descripcion },
                        FechaMovimiento = compraAL30.FechaMovimiento,
                        Cantidad = compraAL30.Cantidad,
                        Precio = compraAL30.Precio,
                        MontoTotal = compraAL30.MontoTotal,
                        Observaciones = compraAL30.Observaciones,
                        EnDb = compraAL30.EnDb,
                        PermitirDb = true
                    };

                    Movimiento mepComision = new Movimiento
                    {
                        NroMovimiento = comisionDolarMEP.NroMovimiento,
                        Broker = comisionDolarMEP.Broker,
                        Ticket = new Ticket { Id = null }, //TODO. sacer el tipo de ticket dolarmep de db por parametro
                        TipoMovimiento = new TipoMovimiento { Id = comisionSPB.Id, Tipo = comisionSPB.Tipo, Descripcion = comisionSPB.Descripcion }, //TODO. sacar tipo movimiento de db por parametro
                        FechaMovimiento = comisionDolarMEP.FechaMovimiento,
                        Cantidad = comisionDolarMEP.Cantidad,
                        Precio = comisionDolarMEP.Precio,
                        MontoTotal = comisionDolarMEP.MontoTotal,
                        Observaciones = comisionDolarMEP.Observaciones,
                        EnDb = comisionDolarMEP.EnDb,
                        PermitirDb = true
                    };

                    //SE LIMPIAN LOS MOVIMIENTOS
                    retorno = movimientos
                        .Where(m => !movimientosDeOperacionDolarMEP.Any(o => o.NroMovimiento == m.NroMovimiento))
                        .ToList();

                    //SE AGREGAN LOS NUEVOS MOVIMIENTOS DE OPERACION DOLAR MEP
                    retorno.Add(mepCompra);
                    retorno.Add(mepVenta);
                    retorno.Add(mepComision);
                }
            }
            return retorno;
        }

        private static string LimpiarTipoMovimiento(string tipoMovimiento)
        {
            int indexParentesis = tipoMovimiento.IndexOf('(');
            int indexGuion = tipoMovimiento.IndexOf('-');

            // Verificar si se encontró un paréntesis
            if (indexParentesis != -1)
            {
                return tipoMovimiento.Substring(0, indexParentesis).Trim();
            }
            // Verificar si se encontró un guion
            else if (indexGuion != -1)
            {
                return tipoMovimiento.Substring(0, indexGuion).Trim();
            }
            else
            {
                return tipoMovimiento;
            }
        }
    }
}
