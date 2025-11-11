using DAL.Entities;
using DAL.Interfaces;
using DAL.Providers;

namespace DAL
{
    public class EntityContext
    {
        public IDataProvider<Test> TestsProvider { get; }
        public IDataProvider<TestResult> ResultsProvider { get; }

        public EntityContext()
        {
            // прості шляхи у корені виконуваного файлу
            TestsProvider   = new JsonProvider<Test>("../../../Data/tests.json");
            ResultsProvider = new JsonProvider<TestResult>("../../../Data/results.json");
        }
    }
}
