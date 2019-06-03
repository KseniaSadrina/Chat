using Newtonsoft.Json;

namespace Chat.Configuration
{
  public class TokenConfiguration
  {
    [JsonProperty("issuer")]
    public string Issuer { get; set; }

    [JsonProperty("access_expiration")]
    public int AccessExpiration { get; set; }

    [JsonProperty("audience")]
    public string Audience { get; set; }

    [JsonProperty("secret")]
    public string Secret { get; set; }
  }
}
