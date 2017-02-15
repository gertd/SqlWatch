#region

using CommandLine;

#endregion

namespace SqlWatch
{
    [Verb("create", HelpText = "Create event session")]
    class CreateOptions : EventSessionOptions
    {
    }

    [Verb("delete", HelpText = "Delete event session")]
    class DeleteOptions : EventSessionOptions
    {
    }

    [Verb("list", HelpText = "List available event sessions")]
    class ListOptions : SqlConnectionOptions
    {
    }

    [Verb("start", HelpText = "Start event session")]
    class StartOptions : EventSessionOptions
    {
    }

    [Verb("stop", HelpText = "Stop event session")]
    class StopOptions : EventSessionOptions
    {
    }

    [Verb("status", HelpText = "Status of event session")]
    class StatusOptions : EventSessionOptions
    {
    }

    [Verb("monitor", HelpText = "Monitor event session")]
    class MonitorOptions : EventSessionOptions
    {
    }

    interface ISqlConnectionOptions
    {
        [Option('S', "server")]
        string Server { get; set; }

        [Option('d', "database", Default = "master")]
        string Database { get; set; }

        [Option('E', "windowsAuth", Default = true)]
        bool WindowsAuth { get; set; }
    }

    interface IOptions
    {
    }

    interface IEventSessionOptions
    {
        [Option('s', "session")]
        string Session { get; set; }
    }

    class EventSessionOptions : SqlConnectionOptions, IEventSessionOptions
    {
        public string Session { get; set; }
    }

    class SqlConnectionOptions : ISqlConnectionOptions, IOptions
    {
        public string Database { get; set; }
        public string Server { get; set; }
        public bool WindowsAuth { get; set; }
    }
}
