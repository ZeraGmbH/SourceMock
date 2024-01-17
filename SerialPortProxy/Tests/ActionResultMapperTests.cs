using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using SharedLibrary.Models;

namespace SerialPortProxyTests;

[TestFixture]
public class ActionResultMapperTests
{
    [TestCase(typeof(ArgumentException), SerialPortErrorCodes.SerialPortBadRequest)]
    [TestCase(typeof(InvalidOperationException), SerialPortErrorCodes.SerialPortBadRequest)]
    [TestCase(typeof(OperationCanceledException), SerialPortErrorCodes.SerialPortAborted)]
    [TestCase(typeof(TimeoutException), SerialPortErrorCodes.SerialPortTimeOut)]
    public async Task Will_Generate_Action_Result(Type exception, SerialPortErrorCodes expected)
    {
        var result = await ActionResultMapper.SafeExecuteSerialPortCommand(() => Task.FromException((Exception)Activator.CreateInstance(exception)!));

        if (result is ObjectResult objectResult)
            if (objectResult.Value is ProblemDetails problemDetails)
            {
                Assert.That(problemDetails.Extensions[SamDetailExtensions.SamErrorCode.ToString()], Is.EqualTo(expected.ToString()));

                return;
            }

        Assert.Fail("no problem details");
    }

    [TestCase(typeof(NotImplementedException))]
    [TestCase(typeof(NotSupportedException))]
    [TestCase(typeof(NullReferenceException))]
    public void Will_Passthough_Unknown_Exception(Type exception)
    {
        var error = Assert.CatchAsync(() => ActionResultMapper.SafeExecuteSerialPortCommand(() => Task.FromException((Exception)Activator.CreateInstance(exception)!)));

        Assert.That(error, Is.TypeOf(exception));
    }
}
