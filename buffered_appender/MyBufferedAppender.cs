using System;
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
            foreach (var loggingEvent in events)
            {
                MessageSink.Logs.Add(loggingEvent);
            }
           
        }
    }
}