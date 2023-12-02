using NUnit.Framework.Interfaces;

namespace SourceApi.Tests.Misc
{
    public class IntegrationTestAttribute : TestAttribute, ITestAction
    {
        private readonly bool _shouldExecute;

        public IntegrationTestAttribute(bool shouldExecute)
        {
            _shouldExecute = shouldExecute;

        }

        public ActionTargets Targets => ActionTargets.Default;

        public void BeforeTest(ITest test)
        {
            if (!_shouldExecute) Assert.Ignore();
        }

        public void AfterTest(ITest test) { }
    }
}