using System.Text.Json;
using RecruitmentSystem.ViewModels;

namespace RecruitmentSystem.Services
{
    public class ExamResultService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ExamResultService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<(ExamResultDto? result, string raw)> GetExamResultByEmailAsync(string email)
        {
            string baseUrl = _config["ExamApi:BaseUrl"]!;
            string url = $"{baseUrl}?email={Uri.EscapeDataString(email)}";

            var response = await _httpClient.GetAsync(url);
            var raw = await response.Content.ReadAsStringAsync();

            // ✅ If HTML comes, stop parsing
            if (raw.TrimStart().StartsWith("<"))
                return (null, raw);

            try
            {
                var obj = JsonSerializer.Deserialize<ExamResultDto>(
                    raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return (obj, raw);
            }
            catch
            {
                return (null, raw);
            }
        }
    }
}
