using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;

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
}