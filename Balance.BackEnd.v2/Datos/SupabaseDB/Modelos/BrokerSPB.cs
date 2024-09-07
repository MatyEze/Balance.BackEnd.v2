using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Balance.BackEnd.v2.Datos.SupabaseDB.Modelos
{
    [Table("Brokers")]
    public class BrokerSPB : BaseModel
    {
        [PrimaryKey("Id")]
        public int Id { get; set; }
        [Column("ResourceKey")]
        public string ResourceKey { get; set; }
        [Column("Descripcion")]
        public string Descripcion { get; set; }
        [Column("Cabecera")]
        public string Cabecera { get; set; }
    }
}
