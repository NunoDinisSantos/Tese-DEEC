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

        public async Task<List<Student>> GetStudentsAsync()
        {
            var response = await _httpClient.GetAsync("api/misteriosaquaticos");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Student>>(json); // Está mal -> result é 0 porque não está a ser mapeado

            if(result == null)
            {
                result = new List<Student>();
            }

            return result;
        }

        public async Task<Student> GetStudentAsync(string studentId)
        {
            var response = await _httpClient.GetAsync($"api/misteriosaquaticos/{studentId}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Student>(json);

            if (result == null) { return new Student() { PlayerId = "NOTFOUND" }; } 

            return result;
        }

        public async Task<HttpStatusCode> UpdateStudentCreditsAsync(string studentId, int credits)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/misteriosaquaticos/{studentId}", credits);

            return response.StatusCode;
        }
    }
}