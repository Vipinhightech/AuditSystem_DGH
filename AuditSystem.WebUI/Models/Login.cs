using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;

namespace AuditSystem.WebUI.Models
{
    public enum AuthenticationStatus
    {
        Failed = 0,
        Authenticated = 1
    }
    public class UserOtp
    {
        [Required(ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        [StringLength(256, MinimumLength = 4)]
        public string UserId { get; set; }

        public string Otp { get; set; }
        public int AppID { get; set; } = 100;

    }

    public class AuthRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        [StringLength(256, MinimumLength = 4)]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        public string Password { get; set; }
        public int Operator { get; set; } = 1;
        public int AppID { get; set; }
    }

    public class ChangePassword
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Current Password is required")]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "New Password must be between 8 and 16 characters long.")]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DisplayName("Confirm New Password")]
        public string RNewPassword { get; set; }
    }

    [Serializable]
    public class AuthResponse
    {
        public string Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Operator { get; set; }
        public string APPLICATION { get; set; }
        public List<string> UserRole { get; set; }
    }
    public class LoginManager
    {
        public AuthResponse ValidateUser(AuthRequest model)
        {
            Authentication.ExternalAuthenticationClient objAuthenticationClient = new Authentication.ExternalAuthenticationClient();

            var response = objAuthenticationClient.ValidateUser(model.UserId, model.Password, model.Operator, model.AppID);
            if (response != null)
            {
                return new AuthResponse
                {
                    UserId = response.Userid,
                    Status = response.Status,
                    UserName = response.Username,
                    Operator = response.Operator,
                    APPLICATION = response.Application,
                    UserRole = response.UserRole.ToList()
                };
            }
            return new AuthResponse();  
        }
        public int GetAppId(string userid)
        {
            int ret = 0;
            try
            {
                OracleParameter[] paramnew = { new OracleParameter { ParameterName = "FUSERID", Value = userid }
                };

                DataSet ds = new Dbcon().ExecuteDataSet("AUDIT_SYS_GET_USERAPPID", paramnew);
                ret = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception ex) { }
            return ret;
        }

    }

    public class PasswordManager
    {
        //public string ResetPassword(string Userid, string Email)
        //{
        //    Authentication.ExternalAuthenticationClient objAuthenticationClient = new Authentication.ExternalAuthenticationClient();
        //    string genPassword = System.Web.Security.Membership.GeneratePassword(10, 1);
        //    genPassword = Regex.Replace(genPassword, @"[^a-zA-Z0-9]", m => "2");
        //    var response = objAuthenticationClient.ForgotPassword(Userid, Email, null, null, genPassword, 301);
        //    return response;
        //}

        //public string ResetUserPassword(string Userid, string Email)
        //{
        //    Authentication.ExternalAuthenticationClient objAuthenticationClient = new Authentication.ExternalAuthenticationClient();
        //    string genPassword = System.Web.Security.Membership.GeneratePassword(10, 1);
        //    genPassword = Regex.Replace(genPassword, @"[^a-zA-Z0-9]", m => "3");
        //    var response = objAuthenticationClient.ForgotPassword(Userid, Email, null, null, genPassword, 301);
        //    return response;
        //}

        //public string ChangeUserPassword(string Userid, string Email, string Password)
        //{
        //    Authentication.ExternalAuthenticationClient objAuthenticationClient = new Authentication.ExternalAuthenticationClient();
        //    var response = objAuthenticationClient.ForgotPassword(Userid, Email, null, null, Password, 301);
        //    return response;
        //}

        public string ChangePassword(ChangePassword chpwd)
        {
            Authentication.ExternalAuthenticationClient objAuthenticationClient = new Authentication.ExternalAuthenticationClient();
            var response = objAuthenticationClient.ChangePassword(chpwd.UserId, chpwd.CurrentPassword,chpwd.NewPassword, 291);
            return response;
        }
    }
}