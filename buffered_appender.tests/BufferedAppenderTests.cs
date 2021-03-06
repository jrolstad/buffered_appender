﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace buffered_appender.tests
{
    [TestFixture]
    public class BufferedAppenderTests
    {
        [SetUp]
        public void BeforeEach()
        {
            MessageSink.ClearLogs();
        }

        [Test]
        public void Send_WithCustomEvaluator_SendsAllMessages()
        {
            // Arrange
            var evaluator = new MyEvaluator();

            var appender = new MyBufferedAppender(evaluator);
            appender.ActivateOptions();

            ConfigureAppender(appender);

            var logger = LogManager.GetLogger(this.GetType());

            // Act
            logger.InfoFormat("Message 1");
            logger.InfoFormat("Message 2");
            logger.InfoFormat("Message 3");

            StopApplication();

            // Assert
            Assert.That(MessageSink.Logs.Count, Is.EqualTo(3), "All remaining messages should be sent at the end");
        }

        [Test]
        public void Send_WithDefaultEvaluator_SendsAllMessages()
        {
            /*
             <appender name="BufferingForwardingAppender" type="buffered_appender.MyBufferedAppender">
              <bufferSize value="3" />
              <Fix value="0"/> <!-- Set Fix flag to NONE -->
             </appender>
              
             see http://stackoverflow.com/questions/11319319/log4net-bufferingforwardingappender-performance-issue
             */

            // Arrange
            var appender = new MyBufferedAppender {BufferSize = 3};
            appender.ActivateOptions();

            ConfigureAppender(appender);

            var logger = LogManager.GetLogger(this.GetType());

            // Act
            logger.InfoFormat("Message 1");
            logger.InfoFormat("Message 2");
            logger.InfoFormat("Message 3");
            logger.InfoFormat("Message 4");
            logger.InfoFormat("Message 5");


            // Assert
            Assert.That(MessageSink.Logs.Count, Is.EqualTo(4),"The fourth message should trigger the Send event since the buffer size of 3 is full");

            StopApplication();
            Assert.That(MessageSink.Logs.Count, Is.EqualTo(5),"All remaining messages should be sent at the end");
        }

        [Test]
        public void Send_WithTimeEvaluator_SendsAllMessages()
        {
            /*
            <appender name="BufferingForwardingAppender" type="buffered_appender.MyBufferedAppender">
             <bufferSize value="3" />
             
              <evaluator type="log4net.Core.TimeEvaluator">
                <threhold value="1"/>
              </evaluator>
             
            </appender>
              
            see http://stackoverflow.com/questions/11319319/log4net-bufferingforwardingappender-performance-issue
            */
            // Arrange
            const int oneSecond = 1;
            var appender = new MyBufferedAppender(new TimeEvaluator(oneSecond));
            appender.ActivateOptions();

            ConfigureAppender(appender);

            var logger = LogManager.GetLogger(this.GetType());

            // Act
            logger.InfoFormat("Message 1");
            logger.InfoFormat("Message 2");
            logger.InfoFormat("Message 3");

            var messagesbeforeWait = MessageSink.Logs.ToList();

            var twoSeconds = new TimeSpan(0,0,0,2);
            Thread.Sleep(twoSeconds);
            logger.InfoFormat("Message 4");

            var messagesAfterWait = MessageSink.Logs.ToList();

            logger.InfoFormat("Message 5");

            StopApplication();

            // Assert
            Assert.That(messagesbeforeWait.Count, Is.EqualTo(0), "No Messages should be sent before the wait time");
            Assert.That(messagesAfterWait.Count, Is.EqualTo(4), "The first four should be sent");
            Assert.That(MessageSink.Logs.Count, Is.EqualTo(5), "All remaining messages should be sent at the end");
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
