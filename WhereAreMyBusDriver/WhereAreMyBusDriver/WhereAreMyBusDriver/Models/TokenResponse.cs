namespace WhereAreMyBusDriver.Models
{
    using Newtonsoft.Json;
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }
        [JsonProperty(PropertyName = "localId")]
        public string LocalId { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "idToken")]
        public string IdToken { get; set; }
        [JsonProperty(PropertyName = "registered")]
        public bool Registered { get; set; }
        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty(PropertyName = "expiresIn")]
        public string ExpiresIn { get; set; }
    }

}
