using Newtonsoft.Json;


namespace Models
{
    public class DTOCredentials
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
