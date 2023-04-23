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

    public enum DatabaseResult
    {
        Success,
        ConnectionError
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
    }
}
