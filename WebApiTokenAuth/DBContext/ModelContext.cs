using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApiTokenAuth.Model;

namespace WebApiTokenAuth.DBContext
{
    public class ModelContext
    {
      public  static List<UserModel> users = new List<UserModel>();

      public int CreateUsers(UserModel model)
      {
          users.Add(model);
          return 1;
      }
      public UserModel Find(string userName, string password)
      {
         return   users.Single(a => a.MobileNo == userName && a.Password == password);
      }
    }
}