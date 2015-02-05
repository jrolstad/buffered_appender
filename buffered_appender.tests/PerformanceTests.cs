using System;
using System.Diagnostics;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace buffered_appender.tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [SetUp]
        public void BeforeEach()
        {
            MessageSink.ClearLogs();
        }

        [Test]
        [TestCase(1,1000,AppenderType.Asynchronous)]
        [TestCase(1,1000,AppenderType.Synchronous)]
        [TestCase(5, 1000, AppenderType.Asynchronous)]
        [TestCase(5, 1000, AppenderType.Synchronous)]
        [TestCase(10,1000,AppenderType.Asynchronous)]
        [TestCase(10,1000,AppenderType.Synchronous)]
        public void SynchronousAppender_Send_MeasurePerformance(int bufferSize, int messageCount,AppenderType appenderType)
        {
            // Arrange
            var appender = new MyBufferedAppender
            {
                BufferSize = bufferSize,
                SendAsync = appenderType == AppenderType.Asynchronous
            };

            appender.ActivateOptions();

            ConfigureAppender(appender);

            var logger = LogManager.GetLogger(this.GetType());

            // Act
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            for (int i = 0; i < messageCount; i++)
            {
                logger.Info(Guid.NewGuid().ToString());
            }
            stopWatch.Stop();

            StopApplication();

            // Assert
            if(MessageSink.Logs.Count != messageCount)
            {
                // Give it time to finish sending
                Thread.Sleep(2000);    
            }

            Assert.That(MessageSink.Logs.Count,Is.EqualTo(messageCount),"Not all the messages were sent!");

            Console.WriteLine("Type:{3}|BufferSize:{0}|MessageCount:{1}|Time(seconds):{2}",bufferSize,messageCount,stopWatch.Elapsed.TotalSeconds,appenderType);
            
        }

        public enum AppenderType
        {
            Synchronous,
            Asynchronous
        }

        private static void StopApplication()
        {
            LogManager.Shutdown();
        }

        private static void ConfigureAppender(BufferingAppenderSkeleton appender)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

            hierarchy.Root.AddAppender(appender);
        }
    }
}