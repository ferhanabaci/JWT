using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace JWT
{
    public class JwtAuthenticationManager
    {
        private readonly string key;
        private readonly IDictionary<string, string> users = new Dictionary<string, string>()
        { {"test", "password" },{"test1", "pwd"  }};

        public JwtAuthenticationManager(string key)
        {
            this.key = key;
        }

        public string Authenticate(string username, string password)
        {
            if (!users.Any(u => u.Key == username && u.Value == password))
            { return null; }

            //ilk önce  bir jwt güvenlik belirteci işleyicisi olusturuyoruz
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            //Belirtec anahtarı olusturuyoruz
            var tokenKey = Encoding.ASCII.GetBytes(key);

            // Güvenlik belirteci tanımlayıcısı oluşturmamız gerekecek 
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Bir özneye ihtiyacımız var konunun ne oldugunuz burda belirtmeliyiz 

                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, username )
                    }),

                // bir süre ayarı yapmamız gerekiyor, bu süre ayarı yaparken aynı zamanda kimlik bilgileri algoritmayı da tanımlamam gerekiyor.
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
            };

            //Yukardaki tanımlamalar bittikten sonra  bi belirtec değişkeni tanımlamız gerekiyor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
