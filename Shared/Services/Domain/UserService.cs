using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared.Services
{
    public interface IUserService
    {
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
        
        public async Task<ICollection<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync(_usersEndpoint);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ICollection<User>>(json);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _httpClient.PutAsync(user, _usersEndpoint)
        }
    }
}