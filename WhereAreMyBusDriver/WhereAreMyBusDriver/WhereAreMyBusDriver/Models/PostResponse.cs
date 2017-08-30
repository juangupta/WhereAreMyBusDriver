namespace WhereAreMyBusDriver.Models
{
    using Newtonsoft.Json;
    public class PostResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

}
