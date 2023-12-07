global using NUnit.Framework;

using System.Globalization;

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void Setup()
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-us");
    }
}