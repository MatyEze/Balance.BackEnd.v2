namespace Balance.BackEnd.v2.Servicios.MovimientosService.Modelos
{
    public class Movimiento
    {
        public int NroMovimiento { get; set; }
        public Broker Broker { get; set; }
        public Ticket Ticket { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public int Cantidad { get; set; }
        public Moneda Precio { get; set; }
        public Moneda MontoTotal { get; set; }
        public List<string> Observaciones { get; set; }
        public bool EnDb { get; set; } = false;
        public bool PermitirDb { get; set; } = true;
    }
}
