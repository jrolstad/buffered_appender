using System;
using System.Collections.Generic;
using System.IO;
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

        protected override void SendBuffer(LoggingEvent[] events)
        {
            MessageSink.Logs.AddRange(events);
        }
    }

    public class MessageSink
    {
        public static List<LoggingEvent> Logs = new List<LoggingEvent>(); 
    }
}