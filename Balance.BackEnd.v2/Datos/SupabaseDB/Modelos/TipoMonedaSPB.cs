using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Tipos.Moneda")]
    public class TipoMonedaSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("Tipo")]
        public string Tipo { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
        [Column("Simbolo")]
        public string Simbolo { get; set; }
    }
}
