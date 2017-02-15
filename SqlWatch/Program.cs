#region

using System;
using System.Threading;
using CommandLine;

#endregion

namespace SqlWatch
{
	class Program
	{
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static int _handlingException = 0;

		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			var results =
				Parser.Default
					.ParseArguments<CreateOptions, DeleteOptions, ListOptions, StartOptions, StopOptions, StatusOptions, MonitorOptions>
					(args);

			var texts = results.MapResult(
				(CreateOptions opts) => CmdHandler.RunCreateEventSession(opts),
				(DeleteOptions opts) => CmdHandler.RunDeleteEventSession(opts),
				(ListOptions opts) => CmdHandler.RunListEventSession(opts),
				(StartOptions opts) => CmdHandler.RunStartEventSession(opts),
				(StopOptions opts) => CmdHandler.RunStopEventSession(opts),
				(StatusOptions opts) => CmdHandler.RunStatusEventSession(opts),
				(MonitorOptions opts) => CmdHandler.RunMonitorEventSession(opts),
				CmdHandler.DefaultHandler);

			Environment.Exit(0);
		}

#region UnhandledExceptionHandler
		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
		{
			if (0 == Interlocked.CompareExchange(ref _handlingException, 1, 0))
			{
				try
				{
					var ex = unhandledExceptionEventArgs.ExceptionObject as Exception;

					Log.Error("Unhandled exception handler:");
					Log.ErrorFormat("Is termination =  {0}", unhandledExceptionEventArgs.IsTerminating);
					if (ex != null)
					{
						Log.Error(ex.Message, ex);
					}
					else
					{
						Log.Error("Exception object == null");
					}
				}
				catch
				{
				}
			}
		}
#endregion
	}
}
