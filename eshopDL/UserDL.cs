using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopUtilities;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace eshopDL
{
    public class UserDL
    {
        public static bool ValidateUser(string username, string password)
        {
            bool exist = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT * FROM [user] WHERE username=@username AND password=@password", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    objComm.Parameters.Add("@password", SqlDbType.NVarChar, 50).Value = password;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            exist = true;
                            new UserDL().UserLoginSave(username);
                        }
                    }
                }
            }
            return exist;
        }

        public static int SaveUser(string firstName, string lastName, string username, string password, string email, string address, string city, string phone, string userType, string salt, string zip)
        {
            int userID = 0;
            try
            {
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    using (SqlCommand objComm = new SqlCommand("INSERT INTO [user] (firstName, lastName, username, email, password, address, city, phone, salt, insertDate, zip) VALUES (@firstName, @lastName, @username, @email, @password, @address, @city, @phone, @salt, @insertDate, @zip); SELECT SCOPE_IDENTITY()", objConn))
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = firstName;
                        objComm.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = lastName;
                        objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                        objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = email;
                        objComm.Parameters.Add("@password", SqlDbType.NVarChar, 50).Value = password;
                        objComm.Parameters.Add("@address", SqlDbType.NVarChar, 100).Value = address;
                        objComm.Parameters.Add("@city", SqlDbType.NVarChar, 50).Value = city;
                        objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = phone;
                        objComm.Parameters.Add("@salt", SqlDbType.NVarChar, 50).Value = salt;
                        objComm.Parameters.Add("@insertDate", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();
                        objComm.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = zip;

                        userID = int.Parse(objComm.ExecuteScalar().ToString());
                        if (userID > 0)
                            AddUserToUserType(username, userType);
                    }
                }
                
            }
            catch (SqlException ex)
            {
                ErrorLog.LogError(ex);
                if (ex.Message.Contains("user"))
                    throw new BLException("Email adresa koju ste uneli je već iskorišćena za kreiranje naloga.<br/>Ukoliko već posedujete nalog prijavite sa svojim korisničkim imenom i šifrom. ", ex);
                
            }
            return userID;
        }

        public static DataTable GetUser(string username, int userID)
        {
            DataTable user = new DataTable();
            if (username == string.Empty && userID <= 0)
                return user;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userID, firstName, lastName, address, city, phone, email, username, insertDate, zip FROM [user]", objConn))
                {
                    if (username != string.Empty)
                    {
                        objComm.CommandText += " WHERE username=@username";
                        objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    }
                    else if (userID > 0)
                    {
                        objComm.CommandText += " WHERE userID=@userID";
                        objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    }

                    objConn.Open();
                    
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            user = new DataTable();
                            user.Load(reader);
                        }
                    }
                }
            }
            return user;
        }

        public static DataTable GetUserByEmail(string email)
        {
            DataTable user = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userID, firstName, lastName, address, city, phone, insertDate, zip FROM [user] WHERE email=@email", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = email;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            user = new DataTable();
                            user.Load(reader);
                        }
                    }
                }
            }
            return user;
        }

        public static int SaveUserType(string name)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO userType (name) VALUES (@name)", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public static bool UserTypeExists(string name)
        {
            bool exists = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT * FROM userType WHERE name=@name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            exists = true;
                    }
                }
            }
            return exists;
        }

        public static int AddUserToUserType(string username, string userTypeName)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO userUserType (userID, userTypeID) VALUES (@userID, @userTypeID)", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = GetUserID(username);
                    objComm.Parameters.Add("@userTypeID", SqlDbType.Int).Value = GetUserTypeID(userTypeName);

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        private static int GetUserID(string username)
        {
            int userID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userID FROM [user] WHERE username=@username", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            userID = reader.GetInt32(0);
                    }
                }
            }
            return userID;
        }

        private static int GetUserTypeID(string name)
        {
            int userTypeID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userTypeID FROM userType WHERE name=@name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            userTypeID = reader.GetInt32(0);
                    }
                }
            }
            return userTypeID;
        }

        public static string[] GetUserTypesForUser(string username)
        {
            string userTypes = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT name FROM userType INNER JOIN userUserType ON userType.userTypeID=userUserType.userTypeID WHERE userUserType.userID=@userID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = GetUserID(username);
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userTypes += reader.GetString(0) + ",";
                        }
                    }
                }
            }
            if (userTypes.Length > 0)
                return userTypes.Substring(0, userTypes.Length - 1).Split(',');
            return new string[0];
        }

        public static bool IsUserInUserType(string username, string userTypeName)
        {
            bool isInUserType = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT * FROM userUserType WHERE userID=@userID AND userTypeID=@userTypeID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = GetUserID(username);
                    objComm.Parameters.Add("@userTypeID", SqlDbType.Int).Value = GetUserTypeID(userTypeName);
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            isInUserType = true;
                        while (reader.Read())
                        {
                            
                        }
                    }

                }
            }
            return isInUserType;
        }

        public static string[] GetUserTypes()
        {
            string userTypes = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT name FROM userType", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            userTypes += reader.GetString(0) + ",";
                    }
                }
            }
            if (userTypes.Length > 0)
                return userTypes.Substring(0, userTypes.Length - 1).Split(',');
            return new string[0];
        }

        public static int RemoveUserFromUserType(string username, string userTypeName)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM userUserType WHERE userID=@userID AND userTypeID=@userTypeID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = GetUserID(username);
                    objComm.Parameters.Add("@userTypeID", SqlDbType.Int).Value = GetUserTypeID(userTypeName);

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public static DataTable GetUsers()
        {
            DataTable users = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userID, firstName, lastName, username, email, address, city, phone FROM [user]", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            users = new DataTable();
                            users.Load(reader);
                        }
                    }
                }
            }
            return users;
        }

        public static int DeleteUser(int userID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM [user] WHERE userID=@userID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public static DataTable GetUserTypesDT()
        {
            DataTable userTypes = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT userTypeID, name FROM userType", objConn))
                {
                    objConn.Open();
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            userTypes = new DataTable();
                            userTypes.Load(reader);
                        }
                    }
                }
            }
            return userTypes;
        }

        public static User GetUser(int userID, string username)
        {
            User user = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT [user].userID, firstName, lastName, username, [password], email, [address], city, phone, userType.userTypeID, userType.name, insertDate, zip FROM [user] INNER JOIN userUserType ON [user].userID=userUserType.userID INNER JOIN userType ON userUserType.userTypeID=userType.userTypeID", objConn))
                {
                    objConn.Open();
                    if (userID > 0)
                    {
                        objComm.CommandText += " WHERE userID=@userID";
                        objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;    
                    }
                    else if (username != string.Empty)
                    {
                        objComm.CommandText += " WHERE username=@username";
                        objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    }

                    if (userID > 0 || username != string.Empty)
                    {
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user = new User();
                                user.UserID = reader.GetInt32(0);
                                user.FirstName = reader.GetString(1);
                                user.LastName = reader.GetString(2);
                                user.Username = reader.GetString(3);
                                user.Password = reader.GetString(4);
                                user.Email = reader.GetString(5);
                                user.Address = reader.GetString(6);
                                user.City = reader.GetString(7);
                                user.Phone = reader.GetString(8);
                                user.UserType = new UserType(reader.GetInt32(9), reader.GetString(10));
                                user.InsertDate = !Convert.IsDBNull(reader[11]) ? Common.ConvertToLocalTime(reader.GetDateTime(11)) : DateTime.MinValue;
                                user.Zip = !Convert.IsDBNull(reader[12]) ? reader.GetString(12) : string.Empty;
                            }
                        }
                    }
                }
            }
            return user;
        }

        public static string GetUserSalt(string username)
        {
            string salt = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT salt FROM [user] WHERE username=@username", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            salt = reader.GetString(0);
                    }
                }
            }
            return salt;
        }

        public DataTable GetLast10()
        {
            DataTable users = new DataTable();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_get_last_10", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        users.Load(reader);
                    }
                }
            }
            return Common.ConvertToLocalTime(ref users);
        }

        public int UserLoginSave(string username)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("userLogin_save", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 100).Value = username;
                    objComm.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now.ToUniversalTime();

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public bool UserExists(string username)
        {
            bool exists = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_exists", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        exists = reader.HasRows;
                    }
                }
            }
            return exists;
        }

        public bool SaveUserToken(string username, string token)
        {
            bool status = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_saveToken", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                    objComm.Parameters.Add("@token", SqlDbType.NVarChar, 500).Value = token;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        status = reader.HasRows;
                    }
                }
            }
            return status;
        }

        public User GetUserFromToken(string token)
        {
            User user = null;
            string username = string.Empty;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_get_token", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@token", SqlDbType.NVarChar, 500).Value = token;

                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            username = reader.GetString(0);
                    }
                }
            }

            user = GetUser(-1, username);
            return user;
        }

        public bool ChangePassword(int userID, string password, string salt)
        {
            bool status = false;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_changePassword", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    objComm.Parameters.Add("@password", SqlDbType.NVarChar, 50).Value = password;
                    objComm.Parameters.Add("@salt", SqlDbType.NVarChar, 50).Value = salt;

                    status = objComm.ExecuteNonQuery() == -1;
                }
            }
            return status;
        }

        public int UpdateUser(User user)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("user_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = user.LastName;
                    objComm.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = user.FirstName;
                    objComm.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = user.Email;
                    objComm.Parameters.Add("@phone", SqlDbType.NVarChar, 50).Value = user.Phone;
                    objComm.Parameters.Add("@address", SqlDbType.NVarChar, 100).Value = user.Address;
                    objComm.Parameters.Add("@zip", SqlDbType.NVarChar, 5).Value = user.Zip;
                    objComm.Parameters.Add("@city", SqlDbType.NVarChar, 50).Value = user.City;
                    objComm.Parameters.Add("@userID", SqlDbType.Int).Value=user.UserID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        //public int ChangePassword(int userID, string password, string salt)
        //{
            //int status = 0;
            //using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            //{
                //using(SqlCommand objComm = new SqlCommand("user_changePassword", objConn))
                //{
                    //objConn.Open();
                    //objComm.CommandType = CommandType.StoredProcedure;
                    //objComm.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                    //objComm.Parameters.Add("@password", SqlDbType.NVarChar, 50).Value = password;
                    //objComm.Parameters.Add("@salt", SqlDbType.NVarChar, 50).Value = salt;

                    //status = objComm.ExecuteNonQuery();
                //}
            //}
            //return status;
        //}
    }
}
