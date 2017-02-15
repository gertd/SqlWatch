#region

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using CommandLine;

#endregion

namespace SqlWatch
{
    internal static class CmdHandler
    {
        public static object RunCreateEventSession(CreateOptions opts)
        {
            SqlXEventsHelper.CreateEventSession(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);
            return 0;
        }

        public static object RunDeleteEventSession(DeleteOptions opts)
        {
            SqlXEventsHelper.DeleteEventSession(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);
            return 0;
        }

        public static object RunListEventSession(ListOptions opts)
        {
            SqlXEventsHelper.ListEventSessions(
                SqlHelper.SqlConnectionString(opts))
                .ForEach(x => Console.Out.WriteLine(x));

            return 0;
        }

        public static object RunStartEventSession(StartOptions opts)
        {
            SqlXEventsHelper.StartEventSession(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);
            return 0;
        }

        public static object RunStopEventSession(StopOptions opts)
        {
            SqlXEventsHelper.StopEventSession(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);
            return 0;
        }

        public static object RunStatusEventSession(StatusOptions opts)
        {
            var isRunning = SqlXEventsHelper.IsEventSessionRunning(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);

            Console.Out.WriteLine("Session [{0}] is {1}running", opts.Session, (isRunning ? string.Empty : "not "));
            return 0;
        }
        public static object RunMonitorEventSession(MonitorOptions opts)
        {
            GCSettings.LatencyMode = GCLatencyMode.Batch;
            var exit = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, args) => { exit.Set(); };

            var sub = SqlXEventMonitor.Monitor(
                SqlHelper.SqlConnectionString(opts),
                opts.Session);

            exit.WaitOne();
            sub.Dispose();
            return 0;
        }

        public static object DefaultHandler(IEnumerable<Error> err)
        {
            // Console.Error.WriteLine("DefaultHandler");
            return 1;
        }
    }
}
