using SourceMock.Actions.Source;
using SourceMock.Model;

namespace SourceMock.Tests.Actions.Source
{
    [TestFixture]
    internal class RangeExtensionTests
    {
        [Test]
        public void TestLowerViolation()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(0);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestUpperViolation()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(4);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestBarelyValidUpperLimit()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(3);

            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void TestBarelyValidLowerLimit()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(1);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestValid()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(2.2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestListInvalid()
        {
            // Arrange
            List<QuantizedRange> ranges = new() {
                new(1, 3, 0.1),
                new(5, 7, 0.1)
            };

            // Act
            var result = ranges.IsIncluded(4);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestListValid()
        {
            // Arrange
            List<QuantizedRange> ranges = new() {
                new(1, 3, 0.1),
                new(5, 7, 0.1)
            };

            // Act
            var result1 = ranges.IsIncluded(2);
            var result2 = ranges.IsIncluded(6);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }
    }
}