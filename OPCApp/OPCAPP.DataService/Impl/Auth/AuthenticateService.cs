using OPCApp.DataService.Interface;
using OPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Net.Http;
using Intime.OPC.ApiClient;
using OPCAPP.Domain;
using System.Threading.Tasks;
using OPCApp.Domain.Models;

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
            //HttpClient client = new HttpClient();
            //HttpRequestMessage hrm = new HttpRequestMessage();
            //hrm.Method = HttpMethod.Post; ;
            ////hrm.Properties = new Dictionary<string, object>() { };
            //HttpResponseMessage response = client.GetAsync("http://localhost:1401/Api/Order/Index").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
            //    return users.ToList();
            //}
            //else
            //{
            //    return null;
            //}
            
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            ApiHttpClient client = factory.Create();

            using (var client1 = factory.Create()) {
                var response = client.GetAsync("/api/account/selectUser").Result;
                if (response.IsSuccessStatusCode)
                {
                    var users = response.Content.ReadAsAsync<List<User>>().Result;
                    return users;
                }
                else
                {
                    return null;
                }
            }
            //Uri ur
           
        }
        public bool AddUser(User user)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            ApiHttpClient client = factory.Create();

            using (var client1 = factory.Create())
            {
                user.IsValid = true;
                user.CreateDate = DateTime.Now;
                user.CreateUserId = 1;
                user.UpdateDate = DateTime.Now;
                user.UpdateUserId = 1;
                var response = client.PutAsJsonAsync("/api/account/adduser",user).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool UpdateUser(User user)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            ApiHttpClient client = factory.Create();

            using (var client1 = factory.Create())
            {
                var response = client.PutAsJsonAsync("/api/account/updateuser", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool DelUser(User user)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            ApiHttpClient client = factory.Create();

            using (var client1 = factory.Create())
            {
                var response = client.PutAsJsonAsync("/api/account/deleteuser", user).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool SetIsStop(bool IsStop)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            ApiHttpClient client = factory.Create();

            using (var client1 = factory.Create())
            {
                var url = IsStop ? "/api/account/stop" : "/api/account/enable";
                var response = client.PutAsJsonAsync(url, IsStop).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
