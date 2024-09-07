using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Tipos.Movimiento")]
    public class TipoMovimientoSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("Tipo")]
        public string Tipo { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
    }
}
