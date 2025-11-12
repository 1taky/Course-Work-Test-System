using BLL.Services;
using DAL;

namespace Tests;

public class IsolatedService
{
    public static TestService CreateIsolatedService()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "TestSys_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            string testsPath = Path.Combine(tempDir, "tests.json");
            string resultsPath = Path.Combine(tempDir, "results.json");

            EntityContext ctx = new EntityContextStub(testsPath, resultsPath);
            return new TestService(ctx);
        }
}
