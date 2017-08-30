using Newtonsoft.Json;

namespace WhereAreMyBusDriver.Models
{

    public class Driver
    {
        [JsonProperty(PropertyName = "placa")]
        public string Placa { get; set; }
        [JsonProperty(PropertyName = "vehiculo")]
        public string Vehiculo { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "nombre")]
        public string Nombre { get; set; }
        public string Token { get; set; }
    }

}
