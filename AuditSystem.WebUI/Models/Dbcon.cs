using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AuditSystem.WebUI.Models
{
    public class Dbcon
    {
        string conn = "";
        public Dbcon()
        {
            //conn = config.GetValue<string>("ConnectionStrings:ServiceConnection");
            conn = "User ID=xuser; Password=xuser; Data Source=192.168.0.134:1521/dgh;";
        }
        public DataSet ExecuteDataSet(string ProcName, OracleParameter[] param)
        {
            DataSet ds = new DataSet();

            using (OracleConnection oc = new OracleConnection(conn))
            {

                OracleCommand cmd = new OracleCommand(ProcName, oc);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);

                OracleDataAdapter oda = new OracleDataAdapter(cmd);
                oda.Fill(ds);

            }


            return ds;
        }
    }
}