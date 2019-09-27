using NUnit.Framework;

namespace Böxtensions.Tests
{
    [TestFixture]
    public class ToggleDollarCommandTests
    { 
        [TestCase("abc\"def", 4, 3)]
        [TestCase("abc\"def", 3, 3)]
        [TestCase("abcde\"", 5, 5)]
        [TestCase("abcde\"", 6, 5)]
        [TestCase("\"xyz hallo welt", 1, 0)]
        [TestCase("\"xyz hallo welt", 0, 0)]
        public void Inserts_Dollar_Before_Quote(string line, int cursor, int expected_dollar_index)
        {
            var cmd = ToggleDollarCommand.ToggleDollar(line, cursor);
            AssertReplacement(cmd, expected_dollar_index, 0, "$");
        }

        [TestCase("abc@\"def", 5, 3)]
        [TestCase("abc@\"def", 4, 3)]
        [TestCase("abcde@\"", 6, 5)]
        [TestCase("abcde@\"", 7, 5)]
        [TestCase("@\"xyz hallo welt", 2, 0)]
        [TestCase("@\"xyz hallo welt", 1, 0)]
        public void Inserts_Dollar_Before_Quote_With_At(string line, int cursor, int expected_dollar_index)
        {
            var cmd = ToggleDollarCommand.ToggleDollar(line, cursor);
            AssertReplacement(cmd, expected_dollar_index, 0, "$");
        }

        [TestCase("abc$\"def", 6, 3)]
        [TestCase("abc$\"def", 5, 3)]
        [TestCase("abc$\"def", 4, 3)]
        [TestCase("ab$\"", 4, 2)]
        [TestCase("$\"ab$\"", 1, 0)]
        public void Removes_Dollar_Before_Quote(string line, int cursor, int expected_dollar_index)
        {
            var cmd = ToggleDollarCommand.ToggleDollar(line, cursor);
            AssertReplacement(cmd, expected_dollar_index, 1, "");
        }

        [TestCase("ab@$\"def", 6, 3)]
        [TestCase("ab@$\"def", 5, 3)]
        [TestCase("ab@$\"def", 4, 3)]
        [TestCase("a@$\"", 4, 2)]
        [TestCase("@$\"ab@$\"", 2, 1)]
        public void Removes_Dollar_Before_Quote_With_At(string line, int cursor, int expected_dollar_index)
        {
            var cmd = ToggleDollarCommand.ToggleDollar(line, cursor);
            AssertReplacement(cmd, expected_dollar_index, 1, "");
        }

        [TestCase("abc@\"def", 3)]
        public void No_Action_If_Cursor_Left_Of_Quote(string line, int cursor)
        {
            var cmd = ToggleDollarCommand.ToggleDollar(line, cursor);
            Assert.True(cmd.do_nothing, "do_nothing");
        }

        static void AssertReplacement(ToggleDollarCommand.ReplaceAction cmd, int start, int length, string sub)
        {
            Assert.False(cmd.do_nothing);
            Assert.That(cmd.replace_start, Is.EqualTo(start), "replace_start");
            Assert.That(cmd.replace_length, Is.EqualTo(length), "replace_length");
            Assert.That(cmd.substitution, Is.EqualTo(sub), "substitution");
        }
    }
}
