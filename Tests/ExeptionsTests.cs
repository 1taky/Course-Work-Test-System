using BLL.Services;
using BLL.Exceptions;
using DAL.Entities;

namespace Tests;

public class ExeptionsTests
{
    [Fact]
    public void AddTest_EmptyTitle_Testing()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        Test test = new Test("") { TimePerQuestionSeconds = 30 };

        Assert.Throws<TestExceptions>(() => service.AddTest(test));
    }

    [Fact]
    public void AddTest_Duplicate_Testing()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        service.AddTest(new Test("OOP"));

        Assert.Throws<TestExceptions>(() => service.AddTest(new Test("OOP")));
    }

    [Fact]
    public void DeleteTest_NotFound_Testing()
    {
        TestService service = IsolatedService.CreateIsolatedService();

        Assert.Throws<TestExceptions>(() => service.DeleteTest("Missing"));
    }

    [Fact]
    public void UpdateTimePerQuestion_InvalidValue_Testing()
    {
        TestService service = IsolatedService.CreateIsolatedService();

        service.AddTest(new Test("Error"));

        Assert.Throws<TestExceptions>(() => service.UpdateTimePerQuestion("Error", 2));
    }

    [Fact]
    public void EditTestTitle_ShouldThrow_WhenNotFound()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        Assert.Throws<TestExceptions>(() => service.EditTestTitle("Missing", "New"));
    }

    [Fact]
    public void ChangeQuestionsCount_ShouldThrow_WhenLessThanOne()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        service.AddTest(new Test("Invalid"));
        Assert.Throws<TestExceptions>(() => service.ChangeQuestionsCount("Invalid", 0));
    }

    [Fact]
    public void GetQuestions_ShouldThrow_WhenTestNotFound()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        Assert.Throws<TestExceptions>(() => service.GetQuestions("NotExist"));
    }

    [Fact]
    public void DeleteQuestion_ShouldThrow_WhenInvalidIndex()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        service.AddTest(new Test("Bad"));
        service.AddQuestion("Bad", new Question("Q1", new List<Answer> { new("A", true) }));

        Assert.Throws<TestExceptions>(() => service.DeleteQuestion("Bad", 5));
    }

    [Fact]
    public void RunTestInteractively_ShouldThrow_WhenTestNotFound()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        Assert.Throws<TestExceptions>(() => service.RunTestInteractively("Nope"));
    }

    [Fact]
    public void RunTestInteractively_ShouldThrow_WhenNoQuestions()
    {
        TestService service = IsolatedService.CreateIsolatedService();
        service.AddTest(new Test("EmptyTest"));
        Assert.Throws<TestExceptions>(() => service.RunTestInteractively("EmptyTest"));
    }
}
