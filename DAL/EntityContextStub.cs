namespace DAL
{
    public class EntityContextStub : EntityContext
    {
        public EntityContextStub(string testsPath, string resultsPath)
            : base(testsPath, resultsPath) {}
    }
}
