using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.XEvent.Linq;

namespace SqlWatch
{
    public static class PublishEventExtensions
    {
        public static string ToStr(this PublishedEvent.FieldList list)
        {
            var sb = new StringBuilder();
            foreach (PublishedEventField f in list)
            {
                sb.AppendFormat("{0}:{1}|", f.Name, f.Value);
            }
            return sb.ToString();
        }

        public static string ToStr(this PublishedEvent.ActionList list)
        {
            var sb = new StringBuilder();
            foreach (PublishedAction a in list)
            {
                sb.AppendFormat("{0}:{1}|", a.Name, a.Value);
            }
            return sb.ToString();
        }
    }
}
