using System;
using System.Linq;
using Xunit;
using NSubstitute;


namespace fw_monitor.test
{
    public class FeedbackProviderTest
    {
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
        }

        [Fact]
        public void WhenNoOutputAddedThenNoOutputAvailable()
        {
            const int expectedOutputCount = 0;
            const string expectedOutput = null;
            
            IFeedbackProvider subject = new FeedbackProvider();
            
            Assert.Equal(expectedOutputCount, subject.Output.Count());
            Assert.Equal(expectedOutput, subject.LastOutput);
        }
        
        [Fact]
        public void WhenNoErrorAddedThenNoErrorAvailable()
        {
            const int expectedErrorCount = 0;
            const string expectedError = null;
            
            IFeedbackProvider subject = new FeedbackProvider();
            
            Assert.Equal(expectedErrorCount, subject.Errors.Count());
            Assert.Equal(expectedError, subject.LastError);
        }

        [Fact]
        public void WhenOutputAddedThenOutputAvailable()
        {
            const string expectedFirstMsg = "test 1";
            const int expectedFirstOutputCount = 1;
            const int expectedErrorCount = 0;
            IFeedbackProvider subject = new FeedbackProvider();

            subject.AddOutput(expectedFirstMsg);
            Assert.Equal(expectedFirstOutputCount, subject.Output.Count());
            Assert.Equal(expectedErrorCount, subject.Errors.Count());
            Assert.Equal(expectedFirstMsg, subject.Output.First());
            Assert.Equal(expectedFirstMsg, subject.LastOutput);

            const string expectedSecondMsg = "test 2";
            const int expectedSecondOutputCount = 2;

            subject.AddOutput(expectedSecondMsg);
            Assert.Equal(expectedSecondOutputCount, subject.Output.Count());
            Assert.Equal(expectedErrorCount, subject.Errors.Count());
            Assert.Equal(expectedSecondMsg, subject.Output.ToArray()[1]);
            Assert.Equal(expectedSecondMsg, subject.LastOutput);

        }

        [Fact]
        public void WhenErrorAddedThenErrorAvailable()
        {
            const string expectedFirstMsg = "test 1";
            const int expectedFirstErrorCount = 1;
            const int expectedOutputCount = 0;
            IFeedbackProvider subject=  new FeedbackProvider();

            subject.AddError(expectedFirstMsg);
            Assert.Equal(expectedFirstErrorCount, subject.Errors.Count());
            Assert.Equal(expectedOutputCount, subject.Output.Count());
            Assert.Equal(expectedFirstMsg, subject.Errors.First());
            Assert.Equal(expectedFirstMsg, subject.LastError);
            
            const string expectedSecondMsg = "test 2";
            const int expectedSecondCount = 2;

            subject.AddError(expectedSecondMsg);
            Assert.Equal(expectedSecondCount, subject.Errors.Count());
            Assert.Equal(expectedOutputCount, subject.Output.Count());
            Assert.Equal(expectedSecondMsg, subject.Errors.ToArray()[1]);
            Assert.Equal(expectedSecondMsg, subject.LastError);
        }

        [Fact]
        public void WhenOutputAddedThenOutputEventRaised()
        {
            const string expectedMsg1 = "test 1";
            const int expectedNrOfOutputAddedEvents = 1;
            const int expectedNrOfErrorAddedEvents = 0;
            int actualNrOfOutputEvents = 0;
            int actualNrOfErrorEvents = 0;
            
            IFeedbackProvider subject = new FeedbackProvider();

            subject.OutputAdded += (sender, msg) =>
            {
                Assert.Same(subject, sender);
                Assert.Equal(expectedMsg1, msg);
                actualNrOfOutputEvents++;
            };
            subject.ErrorAdded += (sender, msg) => {actualNrOfErrorEvents++; };
            
            subject.AddOutput(expectedMsg1);
            Assert.Equal(expectedNrOfOutputAddedEvents, actualNrOfOutputEvents);
            Assert.Equal(expectedNrOfErrorAddedEvents, actualNrOfErrorEvents);
        }
        
        [Fact]
        public void WhenErrorAddedThenErrorEventRaised()
        {
            const string expectedMsg1 = "test 1";
            const int expectedNrOfErrorEvents = 1;
            const int expectedNrOfOutputEvents = 0;
            int actualNrOfErrorEvents = 0;
            int actualNrOfOutputEvents = 0;
            
            IFeedbackProvider subject = new FeedbackProvider();

            subject.ErrorAdded += (sender, msg) =>
            {
                Assert.Same(subject, sender);
                Assert.Equal(expectedMsg1, msg);
                actualNrOfErrorEvents++;
            };

            subject.OutputAdded += (sender, msg) => actualNrOfOutputEvents++;
            
            subject.AddError(expectedMsg1);
            Assert.Equal(expectedNrOfErrorEvents, actualNrOfErrorEvents);
            Assert.Equal(expectedNrOfOutputEvents, actualNrOfOutputEvents);
        }

        [Fact]
        public void WhenOwnerSetThenOwnerReported()
        {
            const string expectedOwner = "owner";
            IFeedbackProvider subject = new FeedbackProvider(expectedOwner);

            Assert.Equal(expectedOwner, subject.Owner);
        }
        
    }
}