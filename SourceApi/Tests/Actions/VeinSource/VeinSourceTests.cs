using System.Net;

using SourceApi.Actions.VeinSource;
using SourceApi.Tests.Misc;

namespace SourceApi.Tests.Actions.VeinSource
{
    [TestFixture]
    internal class VeinSourceTests
    {
        const string HOST_IP = "127.0.0.1";
        const int HOST_PORT = 8080;
        #region PositiveTestCases
        [IntegrationTest(IntegrationTestSwitches.DO_VEIN_TESTS)]
        public void GetSystemEntityNameFromVein()
        {
            // Arrange
            HttpClient client = new();
            VeinClient veinClient = new(client, HOST_IP, HOST_PORT);

            // Act
            var result = veinClient.GetSystemEntityName();

            // Assert
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Value, Is.EqualTo("_System"));
        }

        [IntegrationTest(IntegrationTestSwitches.DO_VEIN_TESTS)]
        public void SetCustomerCityToVein()
        {
            // Arrange
            HttpClient client = new();
            VeinClient veinClient = new(client, HOST_IP, HOST_PORT);

            // Act

            HttpStatusCode statusCode = veinClient.SetCustomerCity("foo");
            // Assert
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        #endregion
    }
}