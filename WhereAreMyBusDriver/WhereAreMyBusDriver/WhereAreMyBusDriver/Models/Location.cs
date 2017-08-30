namespace WhereAreMyBusDriver.Models
{
    using Newtonsoft.Json;
    public class Location
    {
        [JsonProperty(PropertyName = "placa")]
        public string Placa { get; set; }
        [JsonProperty(PropertyName = "latitud")]
        public float Latitud { get; set; }
        [JsonProperty(PropertyName = "longitud")]
        public float Longitud { get; set; }
        [JsonProperty(PropertyName = "vehiculo")]
        public string Vehiculo { get; set; }
        [JsonProperty(PropertyName = "ruta")]
        public string Ruta { get; set; }

    }

}
