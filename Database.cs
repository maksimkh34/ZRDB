using SQLite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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
        public int Number { get; set; }
        public string Name { get; set; }
    }

    class User
    {
        public string Passhash { get; set; }
        public string Username { get; set; }
    }

    class School
    {
        public string Name { get; set; }
        public string Subordination { get; set; }
        public string Management { get; set; }
        public string Form { get; set; }
        public string Supervisor { get; set; }
        public string Mail { get; set; }
        public string Site { get; set; }
        public string PAN { get; set; }
        public string Contacts { get; set; }
        public string Address { get; set; }
    }

    public class InternalSchool
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Subordination { get; set; }
        public string Management { get; set; }
        public string Form { get; set; }
        public string Supervisor { get; set; }
        public string Mail { get; set; }
        public string Site { get; set; }
        public string PAN { get; set; }
        public string Contacts { get; set; }
        public string Address { get; set; }
    }

    class Kid
    {
        public int VoucherID { get; set; }
        public string VoucherExtraditer { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Age { get; set; }
        public string HomeAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string MotherFullName { get; set; }
        public string MotherMobPhone { get; set; }
        public string MotherJob { get; set; }
        public string FatherFullName { get; set; }
        public string FatherMobPhone { get; set; }
        public string FatherJob { get; set; }
        public string Family { get; set; }
        public string Notes { get; set; }
        public string Group { get; set; }
    }

    public class InternalKid
    {
        public int Number { get; set; }
        public int VoucherID { get; set; }
        public string VoucherExtraditer { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Age { get; set; }
        public string HomeAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string MotherFullName { get; set; }
        public string MotherMobPhone { get; set; }
        public string MotherJob { get; set; }
        public string FatherFullName { get; set; }
        public string FatherMobPhone { get; set; }
        public string FatherJob { get; set; }
        public string Family { get; set; }
        public string Notes { get; set; }
        public string Group { get; set; }
    }

    class Group
    {
        public string Name { get; set; }
    }

    public class Database
    {
        const bool isAuthSkipAvalibvale = true;
        SQLiteConnection db;
        readonly string DBpassword;

        public string GetPassword()
        {
            return DBpassword;
        }
        private static string GetHash(string text)
        {
            StringBuilder sb = new();
            byte[] hashValue = MD5.HashData(Encoding.UTF8.GetBytes(text.ToCharArray()));

            foreach (byte b in hashValue)
            {
                sb.Append($"{b:X2}");
            }
            return sb.ToString();
        }

        static string GetDefaultDBPath()
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
                var options = new SQLiteConnectionString(GetDefaultDBPath() + "data.zb", true,
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
            if (login == "" && password == DBpassword)
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

            User checkingUser = new()
            {
                Username = login,
                Passhash = password
            };

            List<User> Users = db.Query<User>("SELECT * FROM User");

            foreach (User user_f in Users)
            {
                if (user_f.Username == checkingUser.Username)
                {
                    if (user_f.Passhash == checkingUser.Passhash)
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
            if (users.Count != 0)
            {
                return AddUserResult.AlreadyExists;
            }
            password = GetHash(password);

            User addingUser = new()
            {
                Username = login,
                Passhash = password
            };

            try
            {
                db.Insert(addingUser);
            }
            catch
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

            List<User> list = db.Query<User>("SELECT * FROM User WHERE Username=\"" + Application.Current.Properties["CurrentUserName"] + "\";");
            if (list.Count != 0)
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
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        public List<Userlist> GetUsers()
        {
            List<Userlist> users = new();
            List<User> users_class = db.Query<User>("SELECT * FROM User");
            for (int i = 0; i < users_class.Count; i++)
            {
                Userlist userlist = new()
                {
                    Number = i + 1,
                    Name = users_class[i].Username
                };
                users.Add(userlist);
            }
            return users;
        }

        public List<InternalSchool> GetSchools()
        {
            return Convert(db.Query<School>("SELECT * FROM School; "));
        }

        public DefaultResult InsertSchool(InternalSchool s)
        {
            try
            {
                db.Insert(Convert(s));
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        static School Convert(InternalSchool s)
        {
            School school = new()
            {
                Name = s.Name,
                Subordination = s.Subordination,
                Form = s.Form,
                Supervisor = s.Supervisor,
                Site = s.Site,
                Address = s.Address,
                Management = s.Management,
                Mail = s.Mail,
                Contacts = s.Contacts,
                PAN = s.PAN
            };

            return school;
        }

        static Kid Convert(InternalKid k)
        {
            Kid kid = new()
            {
                VoucherID = k.VoucherID,
                VoucherExtraditer = k.VoucherExtraditer,
                FullName = k.FullName,
                DateOfBirth = k.DateOfBirth,
                School = k.School,
                Grade = k.Grade,
                Age = k.Age,
                HomeAddress = k.HomeAddress,
                PhoneNumber = k.PhoneNumber,
                MotherFullName = k.MotherFullName,
                MotherMobPhone = k.MotherFullName,
                MotherJob = k.MotherJob,
                FatherFullName = k.FatherFullName,
                FatherMobPhone = k.FatherMobPhone,
                FatherJob = k.FatherJob,
                Family = k.Family,
                Notes = k.Notes,
                Group = k.Group
            };

            return kid;
        }

        static List<InternalSchool> Convert(List<School> ls, int start = 1)
        {
            List<InternalSchool> list = new();
            foreach (School s in ls)
            {
                InternalSchool school = new()
                {
                    Number = start++,
                    Name = s.Name,
                    Subordination = s.Subordination,
                    Form = s.Form,
                    Supervisor = s.Supervisor,
                    Site = s.Site,
                    Address = s.Address,
                    Management = s.Management,
                    Mail = s.Mail,
                    Contacts = s.Contacts,
                    PAN = s.PAN
                };

                list.Add(school);
            }
            return list;
        }

        static List<InternalKid> Convert(List<Kid> ls, int start = 1)
        {
            List<InternalKid> list = new ();
            foreach (Kid k in ls)
            {
                InternalKid kid = new()
                {
                    Number = start++,

                    VoucherID = k.VoucherID,
                    VoucherExtraditer = k.VoucherExtraditer,
                    FullName = k.FullName,
                    DateOfBirth = k.DateOfBirth,
                    School = k.School,
                    Grade = k.Grade,
                    Age = k.Age,
                    HomeAddress = k.HomeAddress,
                    PhoneNumber = k.PhoneNumber,
                    MotherFullName = k.MotherFullName,
                    MotherMobPhone = k.MotherFullName,
                    MotherJob = k.MotherJob,
                    FatherFullName = k.FatherFullName,
                    FatherMobPhone = k.FatherMobPhone,
                    FatherJob = k.FatherJob,
                    Family = k.Family,
                    Notes = k.Notes,
                    Group = k.Group
                };

                list.Add(kid);
            }
            return list;
        }

        public DefaultResult ClearSchools()
        {
            try
            {
                db.Execute("DELETE FROM School;");
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }
        public DefaultResult ClearKids()
        {
            try
            {
                db.Execute("DELETE FROM Kid;");
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        public DefaultResult RemoveSchool(InternalSchool s)
        {
            try
            {
                School school = Convert(s);
                db.Execute($"DELETE FROM School WHERE name=\"{school.Name}\" AND PAN=\"{school.PAN}\";");
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        public List<InternalKid> GetKids()
        {
            return Convert(db.Query<Kid>("SELECT * FROM Kid"));
        }

        public DefaultResult InsertKid(InternalKid kid)
        {
            try
            {
            db.Insert(Convert(kid));
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }

        }

        public DefaultResult RemoveKid(InternalKid k)
        {
            try
            {
                Kid kid = Convert(k);
                db.Execute($"DELETE FROM Kid WHERE VoucherID=\"{kid.VoucherID}\" AND FullName=\"{kid.FullName}\";");
                return DefaultResult.Success;
            }
            catch
            {
                return DefaultResult.DatabaseError;
            }
        }

        public Dictionary<string, int> GetGroups()
        {
            var list = GetKids();
            Dictionary<string, int> groups = new();
            foreach (var kid in list)
            {
                if (!groups.ContainsKey(kid.Group))
                {
                    groups.Add(kid.Group, 1);
                }
                else
                {
                    groups[kid.Group]++;
                }
            }
            return groups;
        }

    }
}

