#region

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Security.AccessControl;
using Microsoft.SqlServer.XEvent.Linq;
using Newtonsoft.Json;

#endregion

namespace SqlWatch
{
    public static class SqlXEventMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ConsoleLogger");
        private static readonly log4net.ILog filelog = log4net.LogManager.GetLogger("FileLogger");


        public static IDisposable Monitor(string connectionstring, string session)
        {
            ValidateSqlXEventSession(connectionstring, session);

            var filter = new int[] { 5701, 5703 };

            var xe = new QueryableXEventData(
                connectionstring,
                session,
                EventStreamSourceOptions.EventStream,
                EventStreamCacheOptions.DoNotCache);

            var xs = from x in xe.ToObservable().Where(x => Exclude(x.Fields, "error_number", filter)) select x;
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Converters = new JsonConverterCollection()
                {
                    new PublishEventConverter(),
                    // new CallStackConverter(),
                    new XmlDataConverter(),
                    new MapValueConverter()
                }
            };

            var sub = xs.Subscribe(
                x =>
                {
                    log.InfoFormat("{0}|{1}|{2}{3}", x.Name, x.UUID, x.Fields.ToStr(), x.Actions.ToStr());
                    filelog.Info(JsonConvert.SerializeObject(x, jsonSerializerSettings));
                },
                ex =>
                {
                    log.ErrorFormat("Exception {0}\n{1}", ex.Message, ex);
                },
                () =>
                {
                    log.WarnFormat("Subscription OnCompleted()");
                });

            return sub;
        }

        private static void ValidateSqlXEventSession(string connectionString, string sessionName)
        {
            if (!SqlXEventsHelper.IsSqlAdmin(connectionString))
            {
                throw new PrivilegeNotHeldException("User not in sysadmin role");
            }
            log.InfoFormat("User is in sysadmin role");

            if (!SqlXEventsHelper.ServerSessionExist(connectionString, sessionName))
            {
                throw new ApplicationException("SQL Server Event Session does not exist");
            }
            log.InfoFormat("Session {0} exists", sessionName);

            if (!SqlXEventsHelper.IsEventSessionRunning(connectionString, sessionName))
            {
                SqlXEventsHelper.StartEventSession(connectionString, sessionName);
                if (!SqlXEventsHelper.IsEventSessionRunning(connectionString, sessionName))
                {
                    throw new ApplicationException("SQL Server Event Session is not running");
                }
                log.InfoFormat("Session {0} started", sessionName);
            }
            log.InfoFormat("Session {0} is running", sessionName);
        }

        public static bool Exclude(PublishedEvent.FieldList fieldList, string name, int[] l)
        {
            foreach (var f in from PublishedEventField f in fieldList where f.Name == name select f)
            {
                int i = int.TryParse(f.Value.ToString(), out i) ? i : 0;
                return !l.Contains(i);
            }
            return true;
        }
    }
}
