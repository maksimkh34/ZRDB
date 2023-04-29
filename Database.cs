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

    public class InternalSchool
    {
        public int number { get; set; }
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
    public class Database
    {
        const bool isAuthSkipAvalibvale = true;
        SQLiteConnection db;
        string DBpassword;

        public string GetPassword()
        {
            return DBpassword;
        }
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

            User checkingUser = new User();
            checkingUser.Username = login;
            checkingUser.Passhash = password;

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

            User addingUser = new User();
            addingUser.Username = login;
            addingUser.Passhash = password;

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
            List<Userlist> users = new List<Userlist>();
            List<User> users_class = db.Query<User>("SELECT * FROM User");
            for (int i = 0; i < users_class.Count; i++)
            {
                Userlist userlist = new Userlist();
                userlist.number = i + 1;
                userlist.name = users_class[i].Username;
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

        InternalSchool Convert(School s, int num = 1)
        {
            InternalSchool school = new InternalSchool();

            school.number = num;
            school.name = s.name;
            school.subordination = s.subordination;
            school.form = s.form;
            school.supervisor = s.supervisor;
            school.site = s.site;
            school.address = s.address;
            school.management = s.management;
            school.mail = s.mail;
            school.contacts = s.contacts;
            school.PAN = s.PAN;

            return school;
        }

        School Convert(InternalSchool s)
        {
            School school = new School();

            school.name = s.name;
            school.subordination = s.subordination;
            school.form = s.form;
            school.supervisor = s.supervisor;
            school.site = s.site;
            school.address = s.address;
            school.management = s.management;
            school.mail = s.mail;
            school.contacts = s.contacts;
            school.PAN = s.PAN;

            return school;
        }

        Kid Convert(InternalKid k)
        {
            Kid kid = new();

            kid.VoucherID = k.VoucherID;
            kid.VoucherExtraditer = k.VoucherExtraditer;
            kid.FullName = k.FullName;
            kid.DateOfBirth = k.DateOfBirth;
            kid.School = k.School;
            kid.Grade = k.Grade;
            kid.Age = k.Age;
            kid.HomeAddress = k.HomeAddress;
            kid.PhoneNumber = k.PhoneNumber;
            kid.MotherFullName = k.MotherFullName;
            kid.MotherMobPhone = k.MotherFullName;
            kid.MotherJob = k.MotherJob;
            kid.FatherFullName = k.FatherFullName;
            kid.FatherMobPhone = k.FatherMobPhone;
            kid.FatherJob = k.FatherJob;
            kid.Family = k.Family;
            kid.Notes = k.Notes;
            kid.Group = k.Group;

            return kid;
        }

        List<School> Convert(List<InternalSchool> ls)
        {
            List<School> list = new List<School>();
            foreach (InternalSchool s in ls)
            {
                School school = new School();

                school.name = s.name;
                school.subordination = s.subordination;
                school.form = s.form;
                school.supervisor = s.supervisor;
                school.site = s.site;
                school.address = s.address;
                school.management = s.management;
                school.mail = s.mail;
                school.contacts = s.contacts;
                school.PAN = s.PAN;

                list.Add(school);
            }
            return list;
        }

        List<InternalSchool> Convert(List<School> ls, int start = 1)
        {
            List<InternalSchool> list = new List<InternalSchool>();
            foreach (School s in ls)
            {
                InternalSchool school = new InternalSchool();

                school.number = start++;
                school.name = s.name;
                school.subordination = s.subordination;
                school.form = s.form;
                school.supervisor = s.supervisor;
                school.site = s.site;
                school.address = s.address;
                school.management = s.management;
                school.mail = s.mail;
                school.contacts = s.contacts;
                school.PAN = s.PAN;

                list.Add(school);
            }
            return list;
        }

        List<InternalKid> Convert(List<Kid> ls, int start = 1)
        {
            List<InternalKid> list = new List<InternalKid>();
            foreach (Kid k in ls)
            {
                InternalKid kid = new InternalKid();

                kid.Number = start++;

                kid.VoucherID = k.VoucherID;
                kid.VoucherExtraditer = k.VoucherExtraditer;
                kid.FullName = k.FullName;
                kid.DateOfBirth = k.DateOfBirth;
                kid.School = k.School;
                kid.Grade = k.Grade;
                kid.Age = k.Age;
                kid.HomeAddress = k.HomeAddress;
                kid.PhoneNumber = k.PhoneNumber;
                kid.MotherFullName = k.MotherFullName;
                kid.MotherMobPhone = k.MotherFullName;
                kid.MotherJob = k.MotherJob;
                kid.FatherFullName = k.FatherFullName;
                kid.FatherMobPhone = k.FatherMobPhone;
                kid.FatherJob = k.FatherJob;
                kid.Family = k.Family;
                kid.Notes = k.Notes;
                kid.Group = k.Group;

                list.Add(kid);
            }
            return list;
        }

        List<Kid> Convert(List<InternalKid> ls, int start = 1)
        {
            List<Kid> list = new List<Kid>();
            foreach (InternalKid kid in ls)
            {
                Kid k = new Kid();

                kid.VoucherID = k.VoucherID;
                kid.VoucherExtraditer = k.VoucherExtraditer;
                kid.FullName = k.FullName;
                kid.DateOfBirth = k.DateOfBirth;
                kid.School = k.School;
                kid.Grade = k.Grade;
                kid.Age = k.Age;
                kid.HomeAddress = k.HomeAddress;
                kid.PhoneNumber = k.PhoneNumber;
                kid.MotherFullName = k.MotherFullName;
                kid.MotherMobPhone = k.MotherFullName;
                kid.MotherJob = k.MotherJob;
                kid.FatherFullName = k.FatherFullName;
                kid.FatherMobPhone = k.FatherMobPhone;
                kid.FatherJob = k.FatherJob;
                kid.Family = k.Family;
                kid.Notes = k.Notes;
                kid.Group = k.Group;

                list.Add(k);
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
                db.Execute($"DELETE FROM School WHERE name=\"{school.name}\" AND PAN=\"{school.PAN}\";");
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
            try { 
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
    }
}

