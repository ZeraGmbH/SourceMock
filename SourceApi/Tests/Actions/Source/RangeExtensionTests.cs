using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Tests.Actions.Source
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
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestUpperViolation()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(4);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestBarelyValidUpperLimit()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(3);

            // Assert
            Assert.That(result, Is.True);

        }

        [Test]
        public void TestBarelyValidLowerLimit()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TestInvalidQuantisation()
        {
            // Arrange
            QuantizedRange range = new QuantizedRange(0, 10, 0.1);

            // Act
            var result = range.IsIncluded(1.05);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void TestValid()
        {
            // Arrange
            QuantizedRange range = new(1, 3, 0.1);

            // Act
            var result = range.IsIncluded(2.2);

            // Assert
            Assert.That(result, Is.True);
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
            Assert.That(result, Is.False);
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
            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        }
    }
}