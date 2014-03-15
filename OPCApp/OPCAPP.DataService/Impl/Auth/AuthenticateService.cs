using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using Intime.OPC.ApiClient;

using OPCApp.DataService.Interface;
using OPCApp.Domain;


namespace OPCApp.DataService.Impl.Auth
{
    [Export(typeof(IAuthenticateService))]
    public class AuthenticateService : IAuthenticateService
    {
        public string Login(string userName, string password)
        {
            return "OK";
        }


        public List<User> GetUserList(string fieldName, string value)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");

            using (var client1 = factory.Create())
            {
                var response = client1.GetAsync("/api/account/selectUser").Result;
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    var users = response.Content.ReadAsAsync<List<User>>().Result;
                    return users;
                }
            }
            //Uri ur
           
        }
        public bool AddUser(User user)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");

            using (var client = factory.Create())
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

            using (var client = factory.Create())
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
            using (var client = factory.Create())
            {
                var response = client.PutAsJsonAsync("/api/account/deleteuser", user).Result;
                return response.IsSuccessStatusCode;
            }
        }
        public bool SetIsStop(bool isStop)
        {
            var factory = new DefaultApiHttpClientFactory(new Uri("http://localhost:1403"), "100000001", "test001");
            using (var client = factory.Create())
            {
                var url = isStop ? "/api/account/stop" : "/api/account/enable";
                var response = client.PutAsJsonAsync(url, isStop).Result;
                return response.IsSuccessStatusCode;
            }
        }



        public bool SetIsStop(int userId, bool isStop)
        {
            throw new NotImplementedException();
        }

        public Infrastructure.DataService.ResultMsg Add(Domain.Models.OPC_AuthUser model)
        {
            throw new NotImplementedException();
        }

        public Infrastructure.DataService.ResultMsg Edit(Domain.Models.OPC_AuthUser model)
        {
            throw new NotImplementedException();
        }

        public Infrastructure.DataService.ResultMsg Delete(Domain.Models.OPC_AuthUser model)
        {
            throw new NotImplementedException();
        }

        public Infrastructure.PageResult<Domain.Models.OPC_AuthUser> Search(Infrastructure.DataService.IFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
