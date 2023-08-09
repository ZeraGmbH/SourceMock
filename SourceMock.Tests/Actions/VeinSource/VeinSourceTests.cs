using System.Net;

using SourceMock.Actions.VeinSource;

namespace SourceMock.Tests.Actions.VeinSource
{
    internal class VeinSourceTests
    {
        const string HOST_IP = "127.0.0.1";
        const int HOST_PORT = 8080;
        #region PositiveTestCases
        [Test]
        public void GetSystemEntityNameFromVein()
        {
#pragma warning disable CS0162
            if (!IntegrationTestSwitches.DO_VEIN_TESTS) Assert.Ignore();
#pragma warning restore CS0162
            // Arrange
            HttpClient client = new();
            VeinClient veinClient = new(client, HOST_IP, HOST_PORT);

            // Act
            var result = veinClient.GetSystemEntityName();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.AreEqual("_System", result.Value);
        }

        [Test]
        public void SetCustomerCityToVein()
        {
#pragma warning disable CS0162
            if (!IntegrationTestSwitches.DO_VEIN_TESTS) Assert.Ignore();
#pragma warning restore CS0162
            // Arrange
            HttpClient client = new();
            VeinClient veinClient = new(client, HOST_IP, HOST_PORT);

            // Act

            HttpStatusCode statusCode = veinClient.SetCustomerCity("foo");
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
        }
        #endregion
    }
}