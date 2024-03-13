using System.Xml;

namespace ErrorCalculatorApi.Actions.Device;

partial class Mad1ErrorCalculator
{
  private static readonly string VersionRequestXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
      <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
      <!--   MAD_Version.xml -->
      <kmaContainer>
        <kmaVersion>01</kmaVersion>
        <serverVerReq>
        </serverVerReq>
      </kmaContainer>
      </KMA_XML_0_01>
    ";

  private static XmlDocument GetVersionRequest() => LoadXmlFromString(VersionRequestXml);
}