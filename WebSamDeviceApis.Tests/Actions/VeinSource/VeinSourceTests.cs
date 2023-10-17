using System.Net;

using WebSamDeviceApis.Actions.VeinSource;
using WebSamDeviceApis.Tests.Misc;

namespace WebSamDeviceApis.Tests.Actions.VeinSource
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
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.AreEqual("_System", result.Value);
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
            Assert.AreEqual(HttpStatusCode.OK, statusCode);
        }
        #endregion
    }
}