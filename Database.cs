using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZRDB;

namespace Database_nsp
{
    public enum LoginResult
    {
        Success,
        InvalidPassword,
        InvalidLogin,
        ConnectionError,
        InvalidMethod
    }

    public enum DefaultResult
    {
        Success,
        DatabaseError,
        VariableError
    }

    public enum DatabaseResult
    {
        Success,
        ConnectionError
    }

    public enum AddUserResult
    {
        Success,
        DatabaseError,
        AlreadyExists
    }

    public enum RemoveUserResult
    {
        Success,
        DatabaseError,
        UserNotFound,
        removingLastUser
    }

    public class Userlist
    {
        public int number { get; set; }
        public string name { get; set; }
    }

    class User
    {
        public string Passhash { get; set; }
        public string Username { get; set; }
    }

    class School
    {
        public string name { get; set; }
        public string subordination { get; set; }
        public string management { get; set; }
        public string form { get; set; }
        public string supervisor { get; set; }
        public string mail { get; set; }
        public string site { get; set; }
        public string PAN { get; set; }
        public string contacts { get; set; }
        public string address { get; set; }
    }

    public class Database
    {
        const bool isAuthSkipAvalibvale = true;
        SQLiteConnection db;
        string DBpassword;

        private string GetHash(string text)
        {
            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(text.ToCharArray()));

                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            return sb.ToString();
        }

        string getDefaultDBPath()
        {
            return "D:\\ZRDB\\";
        }

        public Database()
        {
            string[] EncryptedPassword = DBEncryption.getPasswords();
            DBpassword = DBEncryption.compileKeyPass(EncryptedPassword, DBEncryption.getKey(EncryptedPassword));
        }

        public DatabaseResult Connect()
        {
            try
            {
                var options = new SQLiteConnectionString(getDefaultDBPath() + "data.zb", true,
                    key: DBpassword);
                db = new SQLiteConnection(options);
                return DatabaseResult.Success;
            }
            catch (Exception ex)
            {
                Application.Current.Properties.Add("ConnectionErrorText", ex.ToString());
                return DatabaseResult.ConnectionError;
            }
        }

        public LoginResult TryLogin(string login, string password)
        {
            if (login == "" && password == ".Gy^9@#-+DnnNk7df-48_l")
            {
                Application.Current.Properties.Add("CurrentUserName", "$$SUPERUSER$$");
                MessageBoxInterface.ShowWarn("В систему был произведен вход от имени Суперпользователя. Этот метод аутентификации создан ТОЛЬКО для экстренных случаев. Пожалуйста, зарегистрируйте собственную учетную запись. ");
                return LoginResult.Success; 
            }
            if (isAuthSkipAvalibvale && login == "" && password == "")
            {
                Application.Current.Properties.Add("CurrentUserName", "$$SKIPPEDLOGIN$$");
                return LoginResult.Success;
            }
            else if (!isAuthSkipAvalibvale && login == "" && password == "") 
            { 
                return LoginResult.InvalidMethod;
            }

            password = GetHash(password);

            User checkingUser = new User();
            checkingUser.Username = login;
            checkingUser.Passhash = password;

            List<User> Users = db.Query<User>("SELECT * FROM User");

            foreach(User user_f in Users)
            {
                if(user_f.Username == checkingUser.Username)
                {
                    if(user_f.Passhash == checkingUser.Passhash)
                    {
                        Application.Current.Properties.Add("CurrentUserName", user_f.Username);
                        return LoginResult.Success;
                    }
                    else
                    {
                        return LoginResult.InvalidPassword;
                    }
                }
            }
            return LoginResult.InvalidLogin;
        }

        public AddUserResult AddUser(string login, string password)
        {
            List<User> users = db.Query<User>("SELECT * FROM User WHERE Username=\"" + login + "\"");
            if(users.Count != 0) 
            {
                return AddUserResult.AlreadyExists;
            }
            password = GetHash(password);

            User addingUser = new User();
            addingUser.Username = login;
            addingUser.Passhash = password;

            try 
            {
                db.Insert(addingUser);
            } catch
            {
                return AddUserResult.DatabaseError;
            }
            return AddUserResult.Success;
        }

        public RemoveUserResult RemoveUser(bool forceRemoving = false)
        {
            List<User> allUsers = db.Query<User>("SELECT * FROM User;");
            if (allUsers.Count == 1 && !forceRemoving)
            {
                return RemoveUserResult.removingLastUser;
            }

            List<User> list = db.Query<User>("SELECT * FROM User WHERE Username=\"" + Application.Current.Properties["CurrentUserName"] +"\";");
            if(list.Count != 0)
            {
                try
                {
                    db.Execute("DELETE FROM User WHERE Username=\"" + Application.Current.Properties["CurrentUserName"] + "\";");
                    return RemoveUserResult.Success;
                }
                catch
                {
                    return RemoveUserResult.DatabaseError;
                }
            }
            else
            {
                return RemoveUserResult.UserNotFound;
            }

        }

        public DefaultResult ChangePassword(string password)
        {
            password = GetHash(password);
            try
            {
                db.Execute("UPDATE User SET Passhash=\"" + password + "\" WHERE Username=\"" + Application.Current.Properties["CurrentUserName"] + "\";");
                return DefaultResult.Success;
            } catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        public List<Userlist> GetUsers()
        {
            List<Userlist> users = new List<Userlist>();
            List<User> users_class = db.Query<User>("SELECT * FROM User");
            for(int i = 0; i < users_class.Count; i++) 
            {
                Userlist userlist = new Userlist();
                userlist.number = i + 1;
                userlist.name = users_class[i].Username;
                users.Add(userlist);
            }
            return users;
        }
    }
}
