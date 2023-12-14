using SerialPortProxy;

namespace SerialPortProxyTests;

[TestFixture]
public class NetworkTests
{
    [Test, Ignore("Please only run manually")]
    public void Can_Connect_To_Network_Bridge()
    {
        using var port = new TcpPortProxy("192.168.7.241:10001");

        port.WriteLine("AAV");

        Assert.That(port.ReadLine(), Is.EqualTo("?"));
    }
}
