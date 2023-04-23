using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZRDB;

namespace Database
{ 
    enum LoginResult
    {
        Success,
        InvalidPassword,
        InvalidLogin,
        ConnectionError,
        InvalidMethod
    }

    enum DatabaseResult
    {
        Success,
        ConnectionError
    }

    class User
    {
        public string Passhash;
        public string Username;
    }

    class Database
    {
        const bool isAuthSkipAvalibvale = false;
        SQLiteConnection db;
        string DBpassword;

        Database()
        {
            string[] EncryptedPassword = DBEncryption.getPasswords();
            DBpassword = DBEncryption.compileKeyPass(EncryptedPassword, DBEncryption.getKey(EncryptedPassword));
        }

        public DatabaseResult Connect()
        {
            try
            {
                var options = new SQLiteConnectionString("data.zb", true,
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
            if (isAuthSkipAvalibvale && login == "" && password == "")
            {
                return LoginResult.Success;
            }
            else if (!isAuthSkipAvalibvale && login == "" && password == "") 
            { 
                return LoginResult.InvalidMethod;
            }

            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(password.ToCharArray()));

                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            password = sb.ToString();

            User checkingUser = new User();
            checkingUser.Username = login;
            checkingUser.Passhash = password;

            List<User> Users = db.Query<User>("SELECT * FROM Users");

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
    }
}
