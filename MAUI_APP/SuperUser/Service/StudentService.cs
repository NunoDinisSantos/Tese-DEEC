using SuperUser.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SuperUser.Service
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;

        public StudentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Estudante>> GetStudentsAsync()
        {
            var response = await _httpClient.GetAsync("api/misteriosaquaticos");
            response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not successful

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseWrapper<Estudante>>(json);
            return result?.Items ?? new List<Estudante>();
        }

        public async Task<Estudante> GetStudentAsync(string studentId)
        {
            var response = await _httpClient.GetAsync($"api/misteriosaquaticos/{studentId}");
            response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not successful
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Estudante>(json);

            if (result == null) { return new Estudante() { PlayerId = "NOTFOUND" }; } 

            return result;
        }

        public async Task<HttpStatusCode> UpdateStudentCreditsAsync(string studentId, int creditos)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/misteriosaquaticos/{studentId}", creditos);
            response.EnsureSuccessStatusCode();

            return response.StatusCode;
        }
    }
}