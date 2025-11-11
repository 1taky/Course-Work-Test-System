using DAL.Entities;

namespace BLL.Interfaces
{
    public interface ITestService
    {
        // Тести
        void AddTest(Test test);
        void EditTestTitle(string oldTitle, string newTitle);
        void DeleteTest(string title);
        void UpdateTimePerQuestion(string title, int seconds);
        void ChangeQuestionsCount(string title, int newCount);
        List<Test> GetAll();
        List<Test> Search(string keyword);

        // Питання
        void AddQuestion(string testTitle, Question question);
        void DeleteQuestion(string testTitle, int index);
        void EditQuestion(string testTitle, int index, string newText);
        List<Question> GetQuestions(string testTitle);

        // Відповіді
        void AddAnswer(string testTitle, int qIndex, string text, bool isCorrect);
        void DeleteAnswer(string testTitle, int qIndex, int aIndex);
        void EditAnswer(string testTitle, int qIndex, int aIndex, string newText, bool isCorrect);
        List<Answer> GetAnswers(string testTitle, int qIndex);

        // Тестування
        TestResult RunTestInteractively(string title);
        List<TestResult> GetResults();
    }
}
