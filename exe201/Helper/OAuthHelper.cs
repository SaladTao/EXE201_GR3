using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace exe201.Helper
{
    public class OAuthHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string[] Scopes = { "https://www.googleapis.com/auth/cloud-platform" }; // Scopes cho Gemini API
        private static string ApplicationName = "Gemini API Client";

        public static async Task<string> GetAccessToken()
        {
            // Lấy thông tin xác thực từ tệp client_secrets.json mà bạn tải từ Google Cloud Console
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromFile("client_secrets.json").Secrets,
                Scopes,
                "user", // Đây là ID người dùng
                CancellationToken.None,
                new FileDataStore("Gemini.Api.Auth.Store")
            );

            // Trả về OAuth 2.0 access token
            return credential.Token.AccessToken;
        }
        public static async Task<string> GetAccessToken(string clientId, string clientSecret, string redirectUri)
        {
            try
            {
                // Bước 1: Tạo URL để lấy mã code
                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                    $"?client_id={Uri.EscapeDataString(clientId)}" +
                    $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                    $"&response_type=code" +
                    $"&scope=https://www.googleapis.com/auth/generative-language" +
                    $"&access_type=offline&prompt=consent";

                // In URL để kiểm tra (hoặc mở trong trình duyệt)
                Console.WriteLine($"Please visit this URL to authorize: {authUrl}");
                Console.Write("Enter the authorization code: ");
                var code = Console.ReadLine(); // Thay bằng logic tự động lấy code từ callback

                // Bước 2: Trao đổi code để lấy access token
                var tokenRequestBody = new
                {
                    code,
                    client_id = clientId,
                    client_secret = clientSecret,
                    redirect_uri = redirectUri,
                    grant_type = "authorization_code"
                };

                var jsonContent = JsonConvert.SerializeObject(tokenRequestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
                if (tokenResponse.IsSuccessStatusCode)
                {
                    var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                    var tokenData = JObject.Parse(tokenJson);
                    return tokenData["access_token"]?.ToString();
                }
                else
                {
                    var errorResponse = await tokenResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to obtain access token. Status: {tokenResponse.StatusCode}, Details: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"OAuth error: {ex.Message}", ex);
            }
        }
    }
}