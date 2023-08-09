using System.Net;
using System.Net.Http.Headers;

using Newtonsoft.Json.Linq;

namespace SourceMock.Tests.Actions.VeinSource
{
    internal class VeinSourceTests
    {
        #region PositiveTestCases
        [Test]
        public async Task MustConnectToVeinAPI()
        {
#pragma warning disable CS0162
            if (!IntegrationTestSwitches.DO_VEIN_TESTS) Assert.Ignore();
#pragma warning restore CS0162
            // Arrange
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            string payload = @"{ 
                                ""EntityID"": 0,
                                ""componentName"": ""EntityName""
                                }";
            var stringContent = new StringContent(payload);

            // Act
            var response = await client.PostAsync("http://127.0.0.1:8080/api/v1/Vein/GetInfo", stringContent);

            var responseContent = JObject.Parse(await response.Content.ReadAsStringAsync());
            var status = responseContent["status"].ToString();
            var componentValue = responseContent["ReturnInformation"].ToString();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("200", status);
            Assert.AreEqual("_System", componentValue);
        }

        #endregion
    }
}