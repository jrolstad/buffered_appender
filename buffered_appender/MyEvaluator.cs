using log4net.Core;

namespace buffered_appender
{
    public class MyEvaluator : ITriggeringEventEvaluator
    {
        private int _eventCount = 0;

        public bool IsTriggeringEvent(LoggingEvent loggingEvent)
        {
            _eventCount++;

            return _eventCount%2 == 0;

        }
    }
}