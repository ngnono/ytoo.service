using OPCApp.Domain;
using System.Collections.Generic;
namespace OPCApp.DataService.Interface
{
    public interface IAuthenticateService
    {
       string Login(string userName,string password);



       List<User> GetUserList(string fieldName, string value);
       bool AddUser(User user);
       bool UpdateUser(User user);
       bool DelUser(User user);
       bool SetIsStop(bool isStop);
    }
}
