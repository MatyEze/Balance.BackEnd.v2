using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Tipos.Ticket")]
    public class TipoTicketSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("Tipo")]
        public string Tipo { get; set; }
    }
}
