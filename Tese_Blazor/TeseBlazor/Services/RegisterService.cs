namespace TeseBlazor.RegisterService
{
    public class RegisterService
    {
        private readonly HttpClient _httpClient;

        public RegisterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterStudentAsync(string numeroEstudante)
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:44335/api/misteriosaquaticos/2013168188", numeroEstudante);
            return response.IsSuccessStatusCode;
        }
    }
}