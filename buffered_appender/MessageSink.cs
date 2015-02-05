using System.Collections.Concurrent;
using System.Collections.Generic;
using log4net.Core;

namespace buffered_appender
{
    public class MessageSink
    {
        public static ConcurrentBag<LoggingEvent> Logs = new ConcurrentBag<LoggingEvent>();

        public static void ClearLogs()
        {
            Logs = new ConcurrentBag<LoggingEvent>();
        }
    }
}