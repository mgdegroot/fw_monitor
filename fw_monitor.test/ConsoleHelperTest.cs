using Xunit;
using Xunit.Sdk;

namespace fw_monitor.test
{
    public class ConsoleHelperTest
    {
        [Fact]
        public void ParseActionKey_YES_Test()
        {
            string input = "y";
            ConsoleHelper.ActionKey expected = ConsoleHelper.ActionKey.YES;
            ConsoleHelper.ActionKey actual = ConsoleHelper.ParseActionKey(input);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ParseActionKey_NO_Test()
        {
            string input = "n";
            ConsoleHelper.ActionKey expected = ConsoleHelper.ActionKey.NO;
            ConsoleHelper.ActionKey actual = ConsoleHelper.ParseActionKey(input);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ParseActionKey_When_e_then_EXIT_Test()
        {
            string input = "e";
            ConsoleHelper.ActionKey expected = ConsoleHelper.ActionKey.EXIT;
            ConsoleHelper.ActionKey actual = ConsoleHelper.ParseActionKey(input);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ParseActionKey_When_NullOrEmpty_then_CANCEL_Test()
        {
            string input = string.Empty;
            ConsoleHelper.ActionKey expected = ConsoleHelper.ActionKey.CANCEL;
            ConsoleHelper.ActionKey actual = ConsoleHelper.ParseActionKey(input);

            Assert.Equal(expected, actual);

            input = null;
            expected = ConsoleHelper.ActionKey.CANCEL;
            actual = ConsoleHelper.ParseActionKey(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseInputAsBool_When_n_then_False_Test()
        {
            string input = "n";
            bool expected = false;
            bool actual = ConsoleHelper.ParseInputAsBool(input);
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PaseInputAsBool_When_y_then_True_Test()
        {
            string input = "y";
            bool expected = true;
            bool actual = ConsoleHelper.ParseInputAsBool(input);

            Assert.Equal(expected, actual);
        }
    }
}