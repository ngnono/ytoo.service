using OPCApp.DataService.Interface;
using OPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace OPCApp.DataService.Impl
{
    [Export(typeof(IAuthenticateService))]
    public class AuthenticateService : IAuthenticateService
    {
        public string Login(string userName, string password)
        {
            return "OK";
            throw new NotImplementedException();
        }


        public List<User> GetUserList(string fieldName, string value)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage hrm = new HttpRequestMessage();
            hrm.Method = HttpMethod.Post; ;
            //hrm.Properties = new Dictionary<string, object>() { };
            HttpResponseMessage response = client.GetAsync("http://localhost:1401/Api/Order/Index").Result;
            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
                return users.ToList();
            }
            else
            {
                return null;
            }

        }
        public bool AddUser(User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsJsonAsync("http://localhost:1401/Api/Order/AddUser", user).Result;
            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UpdateUser(User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsJsonAsync("http://localhost:1401/Api/Order/UpdateUser", user).Result;
            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsAsync<object>().Result;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DelUser(User user)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsJsonAsync("http://localhost:1401/Api/Order/DelUser", user).Result;
            if (response.IsSuccessStatusCode)
            {
                var users = response.Content.ReadAsAsync<object>().Result;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetIsStop(bool IsStop)
        {
            throw new System.NotImplementedException();
        }

    }
}
