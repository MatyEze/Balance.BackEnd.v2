using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Movimientos")]
    public class MovimientoSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("NrMovimientoBroker")]
        public int NrMovimiento { get; set; }
        [Column("Cantidad")]
        public int Cantidad { get; set; }
        [Column("IdBroker")]
        public int IdBroker { get; set; }
        [Column("IdUsuario")]
        public string IdUsuario { get; set; }
        [Column("IdTipoMovimiento")]
        public int IdTipoMovimiento { get; set; }
        [Column("IdTicket")]
        public int? IdTicket { get; set; }
        [Column("FechaMovimiento")]
        public DateTime FechaMovimiento { get; set; }
        [Column("IdTipoPrecio")]
        public int IdTipoPrecio { get; set; }
        [Column("Precio")]
        public decimal Precio { get; set; }
        [Column("IdTipoMontoTotal")]
        public int IdTipoMontoTotal { get; set; }
        [Column("MontoTotal")]
        public decimal MontoTotal { get; set; }
        [Column("EnDb")]
        public bool EnDb { get; set; }
        [Column("Observaciones")]
        public List<string> Observaciones { get; set; }
        public BrokerSPB BrokerSPB { get; set; }
        public TipoMovimientoSPB TipoMovimientoSPB { get; set; }
        public TicketSPB TicketSPB { get; set; }
        public TipoMonedaSPB TipoPrecio { get; set; }
        public TipoMonedaSPB TipoMontoTotal { get; set; }
    }
}
