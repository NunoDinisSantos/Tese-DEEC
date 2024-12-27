using SuperUser.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SuperUser.Service
{
    public class StudentService(HttpClient httpClient)
    {
        public async Task<List<Student>> GetStudentsAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("api/misteriosaquaticos");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<Student>>(json);
                return result ?? new List<Student>();
            }

            catch (Exception ex)
            {
                return new List<Student>();
            }

        }

        public async Task<Student> GetStudentAsync(string studentId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/misteriosaquaticos/{studentId}");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Student>(json);

                return result ?? new Student() { PlayerId = "NOTFOUND" };
            }

            catch (Exception ex)
            {
                return new Student() { PlayerId = "NOTFOUND" };
            }
        }

        public async Task<HttpStatusCode> UpdateStudentCreditsAsync(string studentId, int credits)
        { 
            var response = await httpClient.PutAsJsonAsync($"api/misteriosaquaticos/{studentId}/credits", credits);

            return response.StatusCode;
        }
    }
}