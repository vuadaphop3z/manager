using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity
{
    public class MsSqlHelper
    {

        public static int ExecuteNonQuery(SqlConnection conn, string cmdText, Dictionary<string, object> cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(conn != null)
                    conn.Close();
            }
        }

        public static int ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }


        public static IDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return rdr;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, Dictionary<string, object> cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (var param in cmdParms)
                {
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = param.Key;
                    if (param.Value == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = param.Value;
                    }
                    
                    cmd.Parameters.Add(parameter);
                }

            }
        }

    }
}
