using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Tickets")]
    public class TicketSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("Ticket")]
        public string Ticket { get; set; }
        [Column("IdTipo")]
        public int IdTipo { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
        public TipoTicketSPB Tipo { get; set; }
    }
}
