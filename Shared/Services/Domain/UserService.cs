using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared.Services
{
    public interface IUserService
    {
        Task<int> GetCountAsync();
        Task<ICollection<User>> GetUsersAsync();
        Task UpdateUserAsync(User user);
    }
    
    public class UserService : IUserService
    {
        private readonly string _usersEndpoint;
        private readonly string _address;
        
        private HttpClient _httpClient;
        
        public UserService()
        {
            _address = "https://jsonplaceholder.typicode.com/{0}";
            _usersEndpoint = string.Format(_address, "users");
            
            _httpClient = new HttpClient();
        }

        public async Task<int> GetCountAsync()
            => (await GetUsersAsync()).Count;

        public async Task<ICollection<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync(_usersEndpoint);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ICollection<User>>(json);
        }

        public async Task UpdateUserAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync(_usersEndpoint, payload);
        }
    }
}