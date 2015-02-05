using System;
using System.Threading.Tasks;
using log4net.Core;

namespace buffered_appender
{
    public class MyAsyncBufferedAppender:log4net.Appender.BufferingAppenderSkeleton
    {
        public MyAsyncBufferedAppender()
        {
           
        }

        public MyAsyncBufferedAppender(ITriggeringEventEvaluator evaluator)
        {
            this.Evaluator = evaluator;
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            var task = Task.Run(() =>
            {
                foreach (var loggingEvent in events)
                {
                    MessageSink.Logs.Add(loggingEvent);
                }
            });
            
        }
    }
}