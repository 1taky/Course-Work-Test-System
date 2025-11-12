using BLL.Services;
using BLL.Exceptions;
using DAL.Entities;
using DAL;

namespace Tests;

public class TestServiceTests
{
    private TestService CreateIsolatedService()
    {
        return IsolatedService.CreateIsolatedService();
    }

    [Fact]
    public void AddTest_Testing()
    {
        TestService service = CreateIsolatedService();
        Test test = new Test("Math") { TimePerQuestionSeconds = 30 };

        service.AddTest(test);

        List<Test> all = service.GetAll();
        Assert.Single(all);
        Assert.Equal("Math", all[0].Title);
    }

    [Fact]
    public void DeleteTest_Testing()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Del"));
        service.DeleteTest("Del");

        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void UpdateTimePerQuestion_Testing()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Speed") { TimePerQuestionSeconds = 10 });

        service.UpdateTimePerQuestion("Speed", 25);

        Assert.Equal(25, service.GetAll().First().TimePerQuestionSeconds);
    }

    [Fact]
    public void AddQuestion_WithAnswers_Testing()
    {
        TestService service = CreateIsolatedService();

        service.AddTest(new Test("Math"));

        Question question = new Question("2+2=?", new List<Answer>
            {
                new("3", false),
                new("4", true),
                new("5", false)
            });

        service.AddQuestion("Math", question);

        List<Question> added = service.GetQuestions("Math");

        Assert.Single(added);
        Assert.Equal("2+2=?", added[0].Text);
    }

    [Fact]
    public void AddQuestion_NoAnswers_GeneratesDefault()
    {
        TestService service = CreateIsolatedService();

        service.AddTest(new Test("Geo"));
        Question question = new Question("Столиця України?");
        service.AddQuestion("Geo", question);

        Question added = service.GetQuestions("Geo")[0];

        Assert.NotEmpty(added.Answers);
        Assert.Contains(added.Answers, a => a.Text.Contains("Правильна") && a.IsCorrect);
    }

    [Fact]
    public void DeleteQuestion_Testing()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("History"));
        service.AddQuestion("History", new Question("Q1", new List<Answer> { new("A", true) }));
        service.AddQuestion("History", new Question("Q2", new List<Answer> { new("B", true) }));

        service.DeleteQuestion("History", 0);

        List<Question> remaining = service.GetQuestions("History");

        Assert.Single(remaining);
        Assert.Equal("Q2", remaining[0].Text);
    }

    [Fact]
    public void EditQuestion_Testing()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Bio"));
        service.AddQuestion("Bio", new Question("Old text", new List<Answer> { new("A", true) }));

        service.EditQuestion("Bio", 0, "New text");

        Question updated = service.GetQuestions("Bio")[0];

        Assert.Equal("New text", updated.Text);
    }

    [Fact]
    public void AddAnswer_AddsSuccessfully()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Chem"));
        service.AddQuestion("Chem", new Question("Q1", new List<Answer> { new("Yes", true) }));

        service.AddAnswer("Chem", 0, "No", false);

        List<Answer> answers = service.GetAnswers("Chem", 0);

        Assert.Equal(2, answers.Count);
        Assert.Contains(answers, a => a.Text == "No");
    }

    [Fact]
    public void EditAnswer_ChangesTextAndFlag()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Physics"));
        service.AddQuestion("Physics", new Question("Q1", new List<Answer> { new("Old", false) }));

        service.EditAnswer("Physics", 0, 0, "New", true);

        Answer answer = service.GetAnswers("Physics", 0)[0];
        Assert.Equal("New", answer.Text);
        Assert.True(answer.IsCorrect);
    }

    [Fact]
    public void DeleteAnswer_RemovesCorrectly()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("IT"));
        service.AddQuestion("IT", new Question("Q", new List<Answer>
            {
                new("A", true),
                new("B", false)
            }));

        service.DeleteAnswer("IT", 0, 1);

        List<Answer> ans = service.GetAnswers("IT", 0);
        Assert.Single(ans);
        Assert.Equal("A", ans[0].Text);
    }

    [Fact]
    public void Search_Testing()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Java Basics"));
        service.AddTest(new Test("C# Advanced"));

        List<Test> result = service.Search("C#");

        Assert.Single(result);
        Assert.Equal("C# Advanced", result.First().Title);
    }

    [Fact]
    public void GetResults_Testing()
    {
        TestService service = CreateIsolatedService();
        List<TestResult> results = service.GetResults();

        Assert.Empty(results);
    }

    [Fact]
    public void TestResult_ToString_ReturnsExpectedFormat()
    {
        TestResult res = new TestResult { TestTitle = "Math", StudentName = "Alice", Percent = 95 };
        string text = res.ToString();
        Assert.Contains("Math", text);
        Assert.Contains("Alice", text);
    }

    [Fact]
    public void ChangeQuestionsCount_ShouldTrimQuestions()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("Trim"));
        service.AddQuestion("Trim", new Question("Q1", new List<Answer> { new("A", true) }));
        service.AddQuestion("Trim", new Question("Q2", new List<Answer> { new("B", false) }));

        service.ChangeQuestionsCount("Trim", 1);
        Assert.Single(service.GetQuestions("Trim"));
    }

    [Fact]
    public void EditTestTitle_ShouldChangeTitle()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("OldTitle"));

        service.EditTestTitle("OldTitle", "NewTitle");

        Test updated = service.GetAll().First();
        Assert.Equal("NewTitle", updated.Title);
    }

    [Fact]
    public void AddQuestion_ShouldAutoSetFirstCorrect_WhenNoCorrectAnswers()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("AutoFix"));
        Question question = new Question("Q?", new List<Answer>
            {
                new("1", false),
                new("2", false)
            });

        service.AddQuestion("AutoFix", question);
        Question added = service.GetQuestions("AutoFix")[0];

        Assert.Contains(added.Answers, a => a.IsCorrect);
    }

    [Fact]
    public void AddQuestion_DefaultAnswers_AutoGenerated()
    {
        TestService service = CreateIsolatedService();
        service.AddTest(new Test("AutoGen"));

        Question question = new Question("Столиця України?");
        service.AddQuestion("AutoGen", question);

        List<Answer> answers = service.GetAnswers("AutoGen", 0);

        Assert.Equal(3, answers.Count);
        Assert.Contains(answers, a => a.IsCorrect);
        Assert.Contains(answers, a => a.Text.Contains("Відповідь"));
    }
}