using System.Linq;
using Taskington.Base.SystemOperations;
using Xunit;

namespace Taskington.Base.Tests
{
    public class ExtractPlaceholdersTests
    {
        [Fact]
        public void PlaceholderFillsInput()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";

            var extracted = placeholders.ExtractPlaceholders("${dir}");
            Assert.Single(extracted);
            Assert.Equal(("dir", "bla"), extracted.First());
        }

        [Fact]
        public void PlaceholderAtBeginningWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";

            var extracted = placeholders.ExtractPlaceholders(@"${dir}\file.txt");
            Assert.Single(extracted);
            Assert.Equal(("dir", "bla"), extracted.First());
        }

        [Fact]
        public void PlaceholderAtEndWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text.txt";

            var extracted = placeholders.ExtractPlaceholders(@"bla\${file}");
            Assert.Single(extracted);
            Assert.Equal(("file", "text.txt"), extracted.First());
        }

        [Fact]
        public void PlaceholderInTheMiddleWithOtherParts()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text";

            var extracted = placeholders.ExtractPlaceholders(@"bla\${file}.txt");
            Assert.Single(extracted);
            Assert.Equal(("file", "text"), extracted.First());
        }

        [Fact]
        public void UnknownPlaceholder()
        {
            var placeholders = new Placeholders();

            var extracted = placeholders.ExtractPlaceholders(@"bla\${file}.txt");
            Assert.Single(extracted);
            Assert.Equal(("file", null), extracted.First());
        }

        [Fact]
        public void MultiplePlaceholders()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            placeholders["file"] = "text.txt";

            var extracted = placeholders.ExtractPlaceholders(@"${dir}\${file}");
            Assert.Collection(extracted,
                placeholder => Assert.Equal(("dir", "bla"), placeholder),
                placeholder => Assert.Equal(("file", "text.txt"), placeholder));
        }

        [Fact]
        public void MultiplePlaceholdersWith1stUnknown()
        {
            var placeholders = new Placeholders();
            placeholders["file"] = "text.txt";

            var extracted = placeholders.ExtractPlaceholders(@"${dir}\${file}");
            Assert.Collection(extracted,
                placeholder => Assert.Equal(("dir", null), placeholder),
                placeholder => Assert.Equal(("file", "text.txt"), placeholder));
        }

        [Fact]
        public void SamePlaceHolderMultipleTimes()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";

            var extracted = placeholders.ExtractPlaceholders(@"${dir}\${dir}");
            Assert.Single(extracted);
            Assert.Equal(("dir", "bla"), extracted.First());
        }

        [Fact]
        public void NoPlaceholder()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";

            var extracted = placeholders.ExtractPlaceholders(@"bla\bla");
            Assert.Empty(extracted);
        }


        [Fact]
        public void SyntaxErrorMissingBrace()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            Assert.Equal(@"${dir\file.txt", placeholders.ResolvePlaceholders(@"${dir\file.txt"));

            var extracted = placeholders.ExtractPlaceholders(@"${dir\file.txt");
            Assert.Empty(extracted);
        }

        [Fact]
        public void SyntaxErrorMissingBraceWithOtherVariableStarting()
        {
            var placeholders = new Placeholders();
            placeholders["dir"] = "bla";
            placeholders["file"] = "text";

            var extracted = placeholders.ExtractPlaceholders(@"${dir\${file}.txt");
            Assert.Single(extracted);
            Assert.Equal((@"dir\${file", null), extracted.First());
        }
    }
}
