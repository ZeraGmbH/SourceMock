using SourceMock.Actions.Source;

namespace SourceMock.Tests.Actions.Source
{
    [TestFixture]
    internal class RangeExtensionTests
    {
        [Test]
        public void TestLowerViolation()
        {
            // Arrange
            Model.Range range = new(1, 3);

            // Act
            var result = range.IsIncluded(0);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestUpperViolation()
        {
            // Arrange
            Model.Range range = new(1, 3);

            // Act
            var result = range.IsIncluded(4);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TestBarelyValidUpperLimit()
        {
            // Arrange
            Model.Range range = new(1, 3);

            // Act
            var result = range.IsIncluded(3);

            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void TestBarelyValidLowerLimit()
        {
            // Arrange
            Model.Range range = new(1, 3);

            // Act
            var result = range.IsIncluded(1);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestValid()
        {
            // Arrange
            Model.Range range = new(1, 3);

            // Act
            var result = range.IsIncluded(2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestListInvalid()
        {
            // Arrange
            List<Model.Range> ranges = new() {
                new(1, 3),
                new(5, 7)
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
            List<Model.Range> ranges = new() {
                new(1, 3),
                new(5, 7)
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