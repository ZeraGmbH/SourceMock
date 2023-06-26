namespace SourceMock.Tests.Actions.Source
{
    public static class SimulatedSourceTestData
    {
        public static IEnumerable<TestCaseData> ValidLoadpointsWithOneOrThreePhases
        {
            get
            {
                yield return new TestCaseData(LoadpointValidator.LoadpointValidatorTestData.Loadpoint001_3AC_valid);
                yield return new TestCaseData(LoadpointValidator.LoadpointValidatorTestData.Loadpoint003_1AC_valid);
            }
        }
    }
}