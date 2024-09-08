using Balance.BackEnd.v2.Datos.SupabaseDB.Modelos;
using Balance.BackEnd.v2.Servicios.MovimientosService.Modelos;

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
