using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Böxtensions.Tests
{
    [TestFixture]
    public class ToggleDollarCommandTests
    {
        [Test]
        public void Inserts_Dollar_Before_Quote()
        {
            var cmd = ToggleDollarCommand.ToggleDollar("abc\"def", 4);
            Assert.False(cmd.do_nothing);
            Assert.That(cmd.replace_start, Is.EqualTo(3));
            Assert.That(cmd.replace_length, Is.EqualTo(0));
            Assert.That(cmd.substitution, Is.EqualTo("$"));
        }

        [Test]
        public void Removes_Dollar_Before_Quote()
        {
            var cmd = ToggleDollarCommand.ToggleDollar("abc$\"def", 5);
            Assert.False(cmd.do_nothing);
            Assert.That(cmd.replace_start, Is.EqualTo(3));
            Assert.That(cmd.replace_length, Is.EqualTo(1));
            Assert.That(cmd.substitution, Is.EqualTo(""));
        }

        [Test]
        public void Removes_Dollar_Before_Quote_With_At()
        {
            var cmd = ToggleDollarCommand.ToggleDollar("abc$@\"def", 6);
            Assert.False(cmd.do_nothing);
            Assert.That(cmd.replace_start, Is.EqualTo(3));
            Assert.That(cmd.replace_length, Is.EqualTo(1));
            Assert.That(cmd.substitution, Is.EqualTo(""));
        }
    }
}
