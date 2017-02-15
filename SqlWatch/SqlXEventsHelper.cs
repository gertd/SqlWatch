#region

using System.Collections.Generic;

#endregion

namespace SqlWatch
{
	internal static class SqlXEventsHelper
	{
		public static void StartEventSession(string connectionstring, string session)
		{
			const string stmt = "ALTER EVENT SESSION [{0}] ON SERVER STATE = START";
			SqlHelper.SqlExecStatement(connectionstring, string.Format(stmt, session));
		}

		public static void StopEventSession(string connectionstring, string session)
		{
			const string stmt = "ALTER EVENT SESSION [{0}] ON SERVER STATE = STOP";
			SqlHelper.SqlExecStatement(connectionstring, string.Format(stmt, session));
		}

	    public static List<string> ListEventSessions(string connectionstring)
	    {
	        const string stmt = "SELECT name FROM sys.server_event_sessions";
	        return SqlHelper.SqlExecList<string>(connectionstring, stmt) as List<string>;
	    }

		public static bool IsEventSessionRunning(string connectionstring, string session)
		{
			const string stmt = "SELECT	ISNULL(COUNT(*), 0) FROM sys.dm_xe_sessions WHERE name = N'{0}'";
			return SqlHelper.SqlExecScalar<bool>(connectionstring, string.Format(stmt, session));
		}

		public static bool ServerSessionExist(string connectionstring, string session)
		{
			const string stmt = "SELECT ISNULL(COUNT(*), 0) FROM sys.server_event_sessions WHERE name = N'{0}'";
			return SqlHelper.SqlExecScalar<bool>(connectionstring, string.Format(stmt, session));
		}

		public static void CreateEventSession(string connectionstring, string session)
		{
			const string stmt = @"
IF NOT EXISTS(SELECT * FROM sys.server_event_sessions WHERE name = N'{0}')
BEGIN
	CREATE EVENT SESSION [{0}] ON SERVER 
	ADD EVENT sqlserver.attention(
		ACTION(package0.last_error,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.sql_text)),
	ADD EVENT sqlserver.error_reported(
		ACTION(package0.last_error,sqlserver.client_app_name,sqlserver.client_hostname,sqlserver.database_id,sqlserver.database_name,sqlserver.sql_text))
	WITH (MAX_MEMORY=4096 KB,EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=OFF)
END
";
			SqlHelper.SqlExecStatement(connectionstring, string.Format(stmt, session));
		}

		public static void DeleteEventSession(string connectionstring, string session)
		{
			const string stmt = @"
IF EXISTS(SELECT * FROM sys.server_event_sessions WHERE name = N'{0}')
BEGIN
	DROP EVENT SESSION [{0}] ON SERVER
END";
			SqlHelper.SqlExecStatement(connectionstring, string.Format(stmt, session));
		}
		public static bool IsSqlAdmin(string connectionstring)
		{
			const string stmt = "SELECT IS_SRVROLEMEMBER(N'sysadmin')";
		    return SqlHelper.SqlExecScalar<bool>(connectionstring, stmt);
		}
	}
}
