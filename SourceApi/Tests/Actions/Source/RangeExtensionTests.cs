using SourceApi.Actions.Source;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Source;

namespace SourceApi.Tests.Actions.Source;

[TestFixture]
internal class RangeExtensionTests
{
    [Test]
    public void TestLowerViolation()
    {
        // Arrange
        QuantizedRange<Voltage> range = new(new(1), new(3), new(0.1));

        // Act
        var result = range.IsIncluded(new(0));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestUpperViolation()
    {
        // Arrange
        QuantizedRange<Voltage> range = new(new(1), new(3), new(0.1));

        // Act
        var result = range.IsIncluded(new(4));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestBarelyValidUpperLimit()
    {
        // Arrange
        QuantizedRange<Voltage> range = new(new(1), new(3), new(0.1));

        // Act
        var result = range.IsIncluded(new(3));

        // Assert
        Assert.That(result, Is.True);

    }

    [Test]
    public void TestBarelyValidLowerLimit()
    {
        // Arrange
        QuantizedRange<Voltage> range = new(new(1), new(3), new(0.1));

        // Act
        var result = range.IsIncluded(new(1));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void TestInvalidQuantisation()
    {
        // Arrange
        var range = new QuantizedRange<Voltage>(new(0), new(10), new(0.1));

        // Act
        var result = range.IsIncluded(new(1.05));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestValid()
    {
        // Arrange
        QuantizedRange<Voltage> range = new(new(1), new(3), new(0.1));

        // Act
        var result = range.IsIncluded(new(2.2));

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void TestListInvalid()
    {
        // Arrange
        List<QuantizedRange<Voltage>> ranges = new() {
            new(new(1), new(3), new(0.1)),
            new(new(5), new(7), new(0.1))
        };

        // Act
        var result = ranges.IsIncluded(new(4));

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestListValid()
    {
        // Arrange
        List<QuantizedRange<Voltage>> ranges = new() {
            new(new(1), new(3), new(0.1)),
            new(new(5), new(7), new(0.1))
        };

        // Act
        var result1 = ranges.IsIncluded(new(2));
        var result2 = ranges.IsIncluded(new(6));

        // Assert
        Assert.That(result1, Is.True);
        Assert.That(result2, Is.True);
    }
}