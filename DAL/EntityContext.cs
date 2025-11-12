using DAL.Entities;
using DAL.Interfaces;
using DAL.Providers;

namespace DAL
{
    public class EntityContext
    {
        public IDataProvider<Test> TestsProvider { get; }
        public IDataProvider<TestResult> ResultsProvider { get; }

        public EntityContext(string testsPath, string resultsPath)
        {
            TestsProvider = new JsonProvider<Test>(testsPath);
            ResultsProvider = new JsonProvider<TestResult>(resultsPath);
        }

        public EntityContext()
        {
            Directory.CreateDirectory("Data");
            TestsProvider = new JsonProvider<Test>("Data/tests.json");
            ResultsProvider = new JsonProvider<TestResult>("Data/results.json");
        }
    }
}
