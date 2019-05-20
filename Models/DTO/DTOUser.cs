using Models.Enums;
using Newtonsoft.Json;

namespace Models
{
    public class RegistrationUser
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("type")]
        public UserType UserType { get; set; }

    }

    public class DTOUser
    {
        public DTOUser()
        {

        }

        public DTOUser(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserType = user.UserType;
            UserName = user.UserName;
        }

        public DTOUser(User user, string accessToken, string role): this(user)
        {
            AccessToken = accessToken;
            Role = role;
        }

        public DTOUser(User user, string accessToken, string refreshToken, string role) : this(user, accessToken, role)
        {
            RefreshToken = refreshToken;
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fullName")]
        public string FullName => string.Join(" ", FirstName, LastName);

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("type")]
        public UserType UserType { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

    }
}
