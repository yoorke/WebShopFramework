using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using eshopDL;

/// <summary>
/// Summary description for CustomRoleProvider
/// </summary>
public class CustomRoleProvider:RoleProvider
{

    /*#region Properties

        private string connStr;

    #endregion*/

    /*public int GetUserID(string username)
    {
        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT ID_KORISNIK FROM KORISNIK WHERE KORIME = @KORIME", objConn);

        objComm.Parameters.Add("@KORIME", SqlDbType.NVarChar, 256).Value = username;

        int id=0;

        try
        {
            objConn.Open();
            System.Data.SqlClient.SqlDataReader reader = objComm.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                id = (int)reader["ID_KORISNIK"];
            }
            reader.Close();
        }
        finally
        {
            objConn.Close();
        }

        return id;
    }*/

    /*private string GetUserName(int userid)
    {
        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT KORIME FROM KORISNIK WHERE ID_KORISNIK = @ID_KORISNIK", objConn);

        objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int).Value = userid;

        string name = "";

        try
        {
            objConn.Open();
            System.Data.SqlClient.SqlDataReader reader = objComm.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                name = (string)reader["KORIME"];
            }
            reader.Close();
        }
        finally
        {
            objConn.Close();
        }

        return name;
    }*/

    /*private int GetRoleID(string rolename)
    {
        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT ID_ULOGA FROM ULOGA WHERE NAZIV = @NAZIV", objConn);

        objComm.Parameters.Add("@NAZIV", SqlDbType.NVarChar, 256).Value = rolename;

        int id = 0;

        try
        {
            objConn.Open();
            System.Data.SqlClient.SqlDataReader reader = objComm.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                id = (int)reader["ID_ULOGA"];
            }
            reader.Close();
        }
        finally
        {
            objConn.Close();
        }

        return id;
    }*/

    /*private string GetRoleName(int roleid)
    {
        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT NAZIV FROM ULOGA WHERE ID_ULOGA = @ID_ULOGA", objConn);

        objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int).Value = roleid;

        string name = "";

        try
        {
            objConn.Open();
            System.Data.SqlClient.SqlDataReader reader = objComm.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                name = (string)reader["NAZIV"];
            }
            reader.Close();
        }
        finally
        {
            objConn.Close();
        }

        return name;
    }*/

    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    {
        //connStr = ConfigurationManager.ConnectionStrings[config["connectionStringName"]].ConnectionString;
        base.Initialize(name, config);
    }

    public override void CreateRole(string roleName)
    {
        if (roleName.IndexOf(',') > 0)
        {
            throw new ArgumentException("Naziv uloge ne moze da sadrzi povlake.");
        }

        if (RoleExists(roleName))
        {
            throw new System.Configuration.Provider.ProviderException("Naziv uloge vec postoji.");
        }

        UserDL.SaveUserType(roleName);
        /*System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("INSERT INTO ULOGA (NAZIV) VALUES(@NAZIV)", objConn);

        objComm.Parameters.Add("@NAZIV", SqlDbType.NVarChar, 256).Value = roleName;

        objConn.Open();
        objComm.ExecuteNonQuery();
        objConn.Close();*/
    }

    public override bool RoleExists(string roleName)
    {       
        /*bool exists = false;

        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM ULOGA WHERE NAZIV = @NAZIV", objConn);

        objComm.Parameters.Add("@NAZIV", SqlDbType.NVarChar, 256).Value = roleName;

        objConn.Open();

        int numRecs = (int)objComm.ExecuteScalar();

        if (numRecs > 0)
        {
            exists = true;
        }

        objConn.Close();

        return exists;*/
        return UserDL.UserTypeExists(roleName);
    }

    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
        foreach (string rolename in roleNames)
        {
            if (!RoleExists(rolename))
            {
                throw new  System.Configuration.Provider.ProviderException("Naziv uloge nije pronadjen.");
            }
        }

        foreach (string username in usernames)
        {
            if (username.IndexOf(',') > 0)
            {
                throw new ArgumentException("Korisnicko ime ne moze da sadrzi povlake.");
            }

            foreach (string rolename in roleNames)
            {
                if (IsUserInRole(username, rolename))
                {
                    throw new System.Configuration.Provider.ProviderException("Korisnik je vec u ulozi.");
                }
            }
        }

        UserDL.AddUserToUserType(usernames[0], roleNames[0]);
        /*System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("INSERT INTO KORISNIKULOGE (ID_KORISNIK,ID_ULOGA) VALUES(@ID_KORISNIK,@ID_ULOGA)", objConn);

        System.Data.SqlClient.SqlParameter userParam = objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int);
        System.Data.SqlClient.SqlParameter roleParam = objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int);

        System.Data.SqlClient.SqlTransaction tran = null;

        try
        {
            objConn.Open();
            tran = objConn.BeginTransaction();
            objComm.Transaction = tran;

            foreach (string username in usernames)
            {
                foreach (string rolename in roleNames)
                {
                    userParam.Value = GetUserID(username);
                    roleParam.Value = GetRoleID(rolename);
                    objComm.ExecuteNonQuery();
                }
            }
            tran.Commit();
        }
        catch (System.Data.SqlClient.SqlException e)
        {            
            tran.Rollback();
        }
        finally
        {
            objConn.Close();
        }*/
    }

    public override string[] GetRolesForUser(string username)
    {
        return UserDL.GetUserTypesForUser(username);
        /*string tmpRoleNames = "";

        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT ID_ULOGA FROM KORISNIKULOGE WHERE ID_KORISNIK = @ID_KORISNIK", objConn);

        int id=GetUserID(username);
        objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int).Value = id;

        
        System.Data.SqlClient.SqlDataReader reader = null;

        try
        {
            objConn.Open();

            reader = objComm.ExecuteReader();

            while (reader.Read())
            {
                tmpRoleNames += GetRoleName(reader.GetInt32(0)) + ",";
            }
        }
        finally
        {
            if (reader != null) 
                reader.Close();
            objConn.Close();
        }

        if (tmpRoleNames.Length > 0)
        {
            tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
            return tmpRoleNames.Split(',');
        }

        return new string[0];*/
    }

    public override bool IsUserInRole(string username, string roleName)
    {
        return UserDL.IsUserInUserType(username, roleName);
        /*bool userIsInRole = false;

        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM KORISNIKULOGE WHERE ID_KORISNIK = @ID_KORISNIK AND ID_ULOGA = @ID_ULOGA", objConn);

        objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int).Value = GetRoleID(roleName);
        objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int).Value = GetUserID(username);

        objConn.Open();

        int numRecs = (int)objComm.ExecuteScalar();

        if (numRecs > 0)
        {
            userIsInRole = true;
        }

        objConn.Close();

        return userIsInRole;*/

    }

    public override string[] GetAllRoles()
    {
        return UserDL.GetUserTypes();
        /*string tmpRoleNames = "";

        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT NAZIV FROM ULOGA", objConn);

        System.Data.SqlClient.SqlDataReader reader = null;

        try
        {
            objConn.Open();

            reader = objComm.ExecuteReader();

            while (reader.Read())
            {
                tmpRoleNames += reader.GetString(0) + ",";
            }
        }
        finally
        {
            if (reader != null) 
                reader.Close();
            objConn.Close();
        }

        if (tmpRoleNames.Length > 0)
        {
            tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
            return tmpRoleNames.Split(',');
        }

        return new string[0];*/
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
        foreach (string rolename in roleNames)
        {
            if (!RoleExists(rolename))
            {
                throw new System.Configuration.Provider.ProviderException("Naziv uloge nije pronadjen.");
            }
        }

        foreach (string username in usernames)
        {
            foreach (string rolename in roleNames)
            {
                if (!IsUserInRole(username, rolename))
                {
                    throw new System.Configuration.Provider.ProviderException("Korisnik ne pripada ulozi.");
                }
            }
        }

        UserDL.RemoveUserFromUserType(usernames[0], roleNames[0]);
        /*System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("DELETE FROM KORISNIKULOGE WHERE ID_KORISNIK = @ID_KORISNIK AND ID_ULOGA = @ID_ULOGA", objConn);


        System.Data.SqlClient.SqlParameter userParam = objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int);
        System.Data.SqlClient.SqlParameter roleParam = objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int);

        System.Data.SqlClient.SqlTransaction tran = null;

        try
        {
            objConn.Open();
            tran = objConn.BeginTransaction();
            objComm.Transaction = tran;

            foreach (string username in usernames)
            {
                foreach (string rolename in roleNames)
                {
                    userParam.Value = GetUserID(username);
                    roleParam.Value = GetRoleID(rolename);
                    objComm.ExecuteNonQuery();
                }
            }

            tran.Commit();
        }
        catch (System.Data.SqlClient.SqlException)
        {
            tran.Rollback();
        }
        finally
        {
            objConn.Close();
        }*/
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
        throw new NotImplementedException();
        /*System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT ID_KORISNIK FROM KORISNIKULOGE WHERE ID_KORISNIK = @ID_KORISNIK AND ID_ULOGA = @ID_ULOGA", objConn);

        System.Data.SqlClient.SqlParameter userParam = objComm.Parameters.Add("@ID_KORISNIK", SqlDbType.Int);
        System.Data.SqlClient.SqlParameter roleParam = objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int);
        userParam.Value = GetUserID(usernameToMatch);
        roleParam.Value = GetRoleID(roleName);

        string tmpUserNames = "";
        System.Data.SqlClient.SqlDataReader reader = null;

        try
        {
            objConn.Open();

            reader = objComm.ExecuteReader();

            while (reader.Read())
            {
                tmpUserNames += GetUserName(reader.GetInt32(0)) + ",";
            }
        }
        finally
        {
            if (reader != null) { reader.Close(); }

            objConn.Close();
        }

        if (tmpUserNames.Length > 0)
        {
            tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
            return tmpUserNames.Split(',');
        }

        return new string[0];*/
    }

    public override string[] GetUsersInRole(string roleName)
    {
        throw new NotImplementedException();
        /*string tmpUserNames = "";

        System.Data.SqlClient.SqlConnection objConn = new System.Data.SqlClient.SqlConnection(connStr);
        System.Data.SqlClient.SqlCommand objComm = new System.Data.SqlClient.SqlCommand("SELECT ID_KORISNIK FROM KORISNIKULOGE WHERE ID_ULOGA = @ID_ULOGA", objConn);

        System.Data.SqlClient.SqlParameter roleParam = objComm.Parameters.Add("@ID_ULOGA", SqlDbType.Int);
        roleParam.Value = GetRoleID(roleName);

        System.Data.SqlClient.SqlDataReader reader = null;

        try
        {
            objConn.Open();

            reader = objComm.ExecuteReader();

            while (reader.Read())
            {
                tmpUserNames += GetUserName(reader.GetInt32(0)) + ",";
            }
        }
        finally
        {
            if (reader != null) { reader.Close(); }
            objConn.Close();
        }

        if (tmpUserNames.Length > 0)
        {
            tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
            return tmpUserNames.Split(',');
        }

        return new string[0];*/
    }

    #region Neimplementirane metode

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }        

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    #endregion
}