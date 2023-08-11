using System.Diagnostics;
using System.Runtime.CompilerServices;

using NUnit.Framework.Interfaces;

namespace SourceMock.Tests.Misc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BenchmarkAttribute : Attribute, ITestAction
    {
        private readonly string _testFunctionName;
        private readonly Stopwatch _stopwatch = new();
        private readonly List<SingleBenchmarkResult> _results = new();

        public BenchmarkAttribute([CallerMemberName] string testFunctionName = "")
        {
            _testFunctionName = testFunctionName;
        }

        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            _stopwatch.Start();
        }

        public void AfterTest(ITest test)
        {
            _stopwatch.Stop();

            _results.Add(new()
            {
                FunctionName = _testFunctionName,
                ElapsedMs = _stopwatch.Elapsed.TotalMilliseconds
            });

            _stopwatch.Reset();
        }

        public List<SingleBenchmarkResult> GetResults()
        {
            return _results;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BenchmarkFixtureAttribute : Attribute, ITestAction
    {
        private readonly string _testFixtureName;
        private readonly List<BenchmarkAttribute> _benchmarkAttributes = new();
        private readonly List<SingleBenchmarkResult> _benchmarkResults = new();

        private readonly Type _type;

        public BenchmarkFixtureAttribute(Type type)
        {
            _type = type;
            _testFixtureName = type.FullName ?? "";
            var infos = _type.GetMembers();

            List<object> attributes = new();
            foreach (var info in infos)
            {
                attributes.AddRange(info.GetCustomAttributes(true));
            }

            foreach (var attribute in attributes)
            {
                if (attribute is BenchmarkAttribute benchmarkAttribute)
                {
                    _benchmarkAttributes.Add(benchmarkAttribute);
                }
            }
        }

        public ActionTargets Targets => ActionTargets.Suite;

        public void AfterTest(ITest test)
        {
            foreach (var benchmarkAttribute in _benchmarkAttributes)
            {
                _benchmarkResults.AddRange(benchmarkAttribute.GetResults());
            }

            Console.WriteLine($"*** Benchmark result for {_testFixtureName} ***");
            foreach (var result in _benchmarkResults)
            {
                Console.WriteLine($"{result.FunctionName}: {result.ElapsedMs} ms");
            }
        }

        public void BeforeTest(ITest test) { }
    }

    public class SingleBenchmarkResult
    {
        public string FunctionName = "";
        public double ElapsedMs = 0;
    }
}