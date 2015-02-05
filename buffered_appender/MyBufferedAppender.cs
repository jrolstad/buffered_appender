using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net.Core;

namespace buffered_appender
{
    public class MyBufferedAppender:log4net.Appender.BufferingAppenderSkeleton
    {
        public MyBufferedAppender()
        {
           
        }

        public MyBufferedAppender(ITriggeringEventEvaluator evaluator)
        {
            this.Evaluator = evaluator;
        }

        public bool SendAsync { get; set; }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            if (SendAsync)
            {
                Task.Run(()=> { SendMessages(events); });
            }
            else
            {
                SendMessages(events);
            }

        }

        private static void SendMessages(IEnumerable<LoggingEvent> events)
        {
            foreach (var loggingEvent in events)
            {
                MessageSink.Logs.Add(loggingEvent);
            }
        }
    }
}