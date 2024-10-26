using Balance.BackEnd.v2.Servicios.AnalisisTecnicoService.Modelos;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Balance.BackEnd.v2.Servicios.AnalisisTecnicoService
{
    public class AnalisisTecnicoService : IAnalisisTecnicoService
    {
        private readonly IMemoryCache _cache;

        public AnalisisTecnicoService(IMemoryCache cache)
        {
            _cache = cache;
        }

        private void GuardarCache(string key, object? value, int minutosEnCache)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutosEnCache)
            });
        }

        public async Task<TicketInfoo?> TicketInfoCdear(string ticket)
        {
            string key = $"TicketInfoCdear_{ticket}";
            int cacheMinutes = 15;
            if (_cache.TryGetValue(key, out TicketInfoo? ticketInfoCache))
            {
                return ticketInfoCache;
            }

            string url = $"https://analisistecnico.com.ar/services/datafeed/symbols?symbol={ticket}:CEDEAR";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                TicketInfoo? ticketInfoo = JsonSerializer.Deserialize<TicketInfoo>(responseString);

                if (ticketInfoo != null && ticketInfoo.Ticket != null)
                {
                    //limpiear el :CEDEAR
                    ticketInfoo.Ticket = ticketInfoo.Ticket.Substring(0, ticketInfoo.Ticket.IndexOf(":"));
                }
                else
                {
                    ticketInfoo = null;
                }

                GuardarCache(key, ticketInfoo, cacheMinutes);
                return ticketInfoo;
            }
        }

        public async Task<TicketInfoo?> TicketInfoAccion(string ticket)
        {
            string key = $"TicketInfoAccion_{ticket}";
            int cacheMinutes = 15;
            if (_cache.TryGetValue(key, out TicketInfoo? ticketInfoCache))
            {
                return ticketInfoCache;
            }

            string url = $"https://analisistecnico.com.ar/services/datafeed/symbols?symbol={ticket}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                TicketInfoo? ticketInfoo = JsonSerializer.Deserialize<TicketInfoo>(responseString);

                if (ticketInfoo != null && ticketInfoo.Ticket != null)
                {
                    GuardarCache(key, ticketInfoo, cacheMinutes);
                    return ticketInfoo;
                }
                else
                {
                    GuardarCache(key, null, cacheMinutes);
                    return null;
                }
            }
        }
    }
}
