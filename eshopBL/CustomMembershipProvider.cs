using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using eshopDL;

namespace eshopBL
{
    public class CustomMembershipProvider:MembershipProvider
    {
        private string _firstName;
        private string _lastName;
        private string _address;
        private string _city;
        private string _phone;
        private string _userType;
        private string _zip;
        

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string UserType
        {
            get { return _userType; }
            set { _userType = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public CustomMembershipProvider()
        {
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override bool ValidateUser(string username, string password)
        {
            string salt = UserBL.GetSalt(username);
            password = UserBL.hashPassword(password, salt);
            return UserDL.ValidateUser(username, password);
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return NewUser(username, password, email, FirstName, LastName, Address, City, Phone, UserType, isApproved, Zip, out status);
        }

        public MembershipUser NewUser(string username, string password, string email, string firstName, string lastName, string address, string city, string phone, string userType, bool isApproved, string zip, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            //status = MembershipCreateStatus.InvalidAnswer;
            MembershipUser user = GetUser(email, false);
            if (user == null)
            {
                string salt = UserBL.getSalt();
                password = UserBL.hashPassword(password, salt);
                if (UserDL.SaveUser(FirstName, LastName, username, password, email, Address, City, Phone, userType, salt, zip) > 0)
                    status = MembershipCreateStatus.Success;
                else
                    status = MembershipCreateStatus.UserRejected;
            }
            else
                status = MembershipCreateStatus.DuplicateUserName;

            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MembershipUser user = null;

            DataTable userTable = UserDL.GetUser(username, 0);
            if (userTable.Rows.Count > 0)
            {
                user = new MembershipUser("CustomMembershipProvider", username, int.Parse(userTable.Rows[0]["userID"].ToString()), string.Empty, null, null, true, false, DateTime.Now.ToUniversalTime(), DateTime.Now.ToUniversalTime(), DateTime.Now.ToUniversalTime(), DateTime.Now.ToUniversalTime(), DateTime.Now.ToUniversalTime());
            }

            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            string username = string.Empty;

            DataTable userTable = UserDL.GetUserByEmail(email);
            if (userTable.Rows.Count > 0)
                username = userTable.Rows[0]["username"].ToString();

            return username;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 1; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 5; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
