namespace Balance.BackEnd.v2.Servicios.MovimientosService.Modelos
{
    public class Ticket
    {
        public int? Id { get; set; }
        public string? TicketString { get; set; }
        public int IdTipo { get; set; }
        public string? Tipo { get; set; }
        public string? Descripcion { get; set; }
    }
}
