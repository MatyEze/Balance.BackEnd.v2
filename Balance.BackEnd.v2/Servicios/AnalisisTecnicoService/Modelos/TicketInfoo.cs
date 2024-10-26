using System.Text.Json.Serialization;

namespace Balance.BackEnd.v2.Servicios.AnalisisTecnicoService.Modelos
{
    public class TicketInfoo
    {
        [JsonPropertyName("name")]
        public string Ticket { get; set; }
        [JsonPropertyName("type")]
        public string Tipo { get; set; }
        [JsonPropertyName("description")]
        public string Descripcion { get; set; }
        [JsonPropertyName("exchange-traded")]
        public string Mercado { get; set; }
    }
}
