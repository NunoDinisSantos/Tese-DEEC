using SuperUser.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

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
                //Trace.WriteLine(json);
                //var result = JsonSerializer.Deserialize<List<Student>>(json);
                var result = JsonConvert.DeserializeObject<List<Student>>(json);

                return result ?? new List<Student>();
            }

            catch (Exception ex)
            {
                Trace.WriteLine($"GetStudentsAsync failed: {ex.Message}");
                return new List<Student>();
            }

        }

        public async Task<Student> GetStudentAsync(string studentId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/misteriosaquaticos/{studentId}");
                var json = await response.Content.ReadAsStringAsync();
                //var result = JsonSerializer.Deserialize<Student>(json);
                var result = JsonConvert.DeserializeObject<Student>(json);

                return result ?? new Student() { PlayerId = "NOTFOUND" };
            }

            catch (Exception ex)
            {
                return new Student() { PlayerId = "NOTFOUND" };
            }
        }

        public async Task<List<Reward>> GetRewardsAsync()
        {
            try
            {
                var response = await httpClient.GetAsync("api/misteriosaquaticos/rewards");
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Reward>>(json);

                return result ?? new List<Reward>();
            }

            catch (Exception ex)
            {
                Trace.WriteLine($"GetRewardsAsync failed: {ex.Message}");
                return new List<Reward>();
            }

        }

        public async Task<HttpStatusCode> UpdateReward(int id, string description, int price)
        {

            var content = new RewardDTO()
            {
                Name = description,
                Price = price
            };

            var serializedContent = JsonConvert.SerializeObject(content);
            var httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"api/misteriosaquaticos/rewards/{id}", httpContent);

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> UpdateStudentCreditsAsync(string studentId, int credits)
        {
            var response = await httpClient.PutAsJsonAsync($"api/misteriosaquaticos/{studentId}/credits", credits);

            return response.StatusCode;
        }
    }
}