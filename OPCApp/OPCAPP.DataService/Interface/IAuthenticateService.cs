using OPCAPP.Domain;
using System.Collections.Generic;
namespace OPCAPP.DataService.Interface
{
    public interface IAuthenticateService
    {
       string Login(string userName,string password);



       List<User> GetUserList(string fieldName, string value);
       bool AddUser(User user);
       bool UpdateUser(User user);
       bool DelUser(User user);
       bool SetIsStop(bool IsStop);
    }
}
