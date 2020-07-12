using PPBackup.Base.SystemOperations;
using Xunit;

namespace PPBackup.Base.Tests
{
    public class PlaceholdersTests
    {
        [Fact]
        public void PlaceholderFillsInput()
        {
            var helper = new Placeholders();
            helper["dir"] = "bla";
            Assert.Equal("bla", helper.ResolvePlaceholders("${dir}"));
        }

        [Fact]
        public void PlaceholderAtBeginningWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            Assert.Equal(@"bla\file.txt", placeholders.ResolvePlaceholders(@"${dir}\file.txt"));
        }

        [Fact]
        public void PlaceholderAtEndWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text.txt";
            Assert.Equal(@"bla\text.txt", placeholders.ResolvePlaceholders(@"bla\${file}"));
        }

        [Fact]
        public void PlaceholderInTheMiddleWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text";
            Assert.Equal(@"bla\text.txt", placeholders.ResolvePlaceholders(@"bla\${file}.txt"));
        }

        [Fact]
        public void UnknownPlaceholder()
        {
            var placeholders = new Placeholders();
            Assert.Equal(@"bla\${file}.txt", placeholders.ResolvePlaceholders(@"bla\${file}.txt"));
        }

        [Fact]
        public void MultiplePlaceholders()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            placeholders["file"] = "text.txt";
            Assert.Equal(@"bla\text.txt", placeholders.ResolvePlaceholders(@"${dir}\${file}"));
        }

        [Fact]
        public void MultiplePlaceholdersWith1stUnknown()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text.txt";
            Assert.Equal(@"${dir}\text.txt", placeholders.ResolvePlaceholders(@"${dir}\${file}"));
        }

        [Fact]
        public void SamePlaceHolderMultipleTimes()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            Assert.Equal(@"bla\bla", placeholders.ResolvePlaceholders(@"${dir}\${dir}"));
        }

        [Fact]
        public void NoPlaceholder()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            Assert.Equal(@"bla\bla", placeholders.ResolvePlaceholders(@"bla\bla"));
        }


        [Fact]
        public void SyntaxErrorMissingBrace()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            Assert.Equal(@"${dir\file.txt", placeholders.ResolvePlaceholders(@"${dir\file.txt"));
        }

        [Fact]
        public void SyntaxErrorMissingBraceWithOtherVariableStarting()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            placeholders["file"] = "text";
            Assert.Equal(@"${dir\${file}.txt", placeholders.ResolvePlaceholders(@"${dir\${file}.txt"));
        }
    }
}
