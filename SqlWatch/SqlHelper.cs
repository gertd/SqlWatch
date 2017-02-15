#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace SqlWatch
{
    internal static class SqlHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string SqlConnectionString(ISqlConnectionOptions opts)
        {
            return new SqlConnectionStringBuilder()
            {
                DataSource = opts.Server,
                InitialCatalog = opts.Database,
                IntegratedSecurity = opts.WindowsAuth,
                AsynchronousProcessing = true,
                MultiSubnetFailover = true,
                ApplicationName = "SqlWatch"
            }.ConnectionString;
        }

        public static string SqlConnectionString(string connectionstring)
        {
            return new SqlConnectionStringBuilder(connectionstring)
            {
                AsynchronousProcessing = true,
                MultiSubnetFailover = true,
                ApplicationName = "SqlWatch"
            }.ConnectionString;
        }

        public static int SqlExecStatement(string connectionstring, string stmt)
        {
            const int failed = -1;
            try
            {
                using (var conn = new SqlConnection(connectionstring))
                using (var cmd = new SqlCommand(stmt))
                {
#if TRACESQL
                    conn.InfoMessage +=
                        (sender, args) =>
                        {
                            log.DebugFormat("InfoMessage {0} {1}", args.Source, args.Message);
                        };
                    conn.StateChange +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StateChange org: {0} cur: {1}", args.OriginalState, args.CurrentState);
                        };
                    cmd.StatementCompleted +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StatementCompleted count: {0}", args.RecordCount);
                        };
#endif
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        return failed;
                    }
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException x)
            {
                log.ErrorFormat("SqlException {0}", x.Message);
                log.Error(x);
            }
            return failed;
        }
        public static T SqlExecScalar<T>(string connectionstring, string stmt)
        {
            try
            {
                using (var conn = new SqlConnection(connectionstring))
                using (var cmd = new SqlCommand(stmt))
                {
#if TRACESQL
                    conn.InfoMessage +=
                        (sender, args) =>
                        {
                            log.DebugFormat("InfoMessage {0} {1}", args.Source, args.Message);
                        };
                    conn.StateChange +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StateChange org: {0} cur: {1}", args.OriginalState, args.CurrentState);
                        };
                    cmd.StatementCompleted +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StatementCompleted count: {0}", args.RecordCount);
                        };
#endif
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        return (T)Convert.ChangeType(null, typeof(T));
                    }
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    var value = cmd.ExecuteScalar();
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch (SqlException x)
            {
                log.ErrorFormat("SqlException {0}", x.Message);
                log.Error(x);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static IList<T>SqlExecList<T>(string connectionstring, string stmt)
        {
            var result = new List<T>();
            try
            {
                using (var conn = new SqlConnection(connectionstring))
                using (var cmd = new SqlCommand(stmt))
                {
#if TRACESQL
                    conn.InfoMessage +=
                        (sender, args) =>
                        {
                            log.DebugFormat("InfoMessage {0} {1}", args.Source, args.Message);
                        };
                    conn.StateChange +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StateChange org: {0} cur: {1}", args.OriginalState, args.CurrentState);
                        };
                    cmd.StatementCompleted +=
                        (sender, args) =>
                        {
                            log.DebugFormat("StatementCompleted count: {0}", args.RecordCount);
                        };
#endif
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        return result;
                    }
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result.Add(reader.GetFieldValue<T>(0));
                    }
                }
            }
            catch (SqlException x)
            {
                log.ErrorFormat("SqlException {0}", x.Message);
                log.Error(x);
            }
            return result;
        }
    }
}
