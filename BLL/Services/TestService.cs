using DAL;
using DAL.Entities;
using BLL.Interfaces;
using BLL.Exceptions;

namespace BLL.Services
{
    public class TestService : ITestService
    {
        private readonly EntityContext _ctx;
        private readonly List<Test> _tests;
        private readonly List<TestResult> _results;

        public TestService()
        {
            _ctx = new EntityContext();
            _tests = _ctx.TestsProvider.Load();
            _results = _ctx.ResultsProvider.Load();
        }

        public TestService(EntityContext context)
        {
            _ctx = context;
            _tests = _ctx.TestsProvider.Load();
            _results = _ctx.ResultsProvider.Load();
        }


        public void AddTest(Test test)
        {
            if (string.IsNullOrWhiteSpace(test.Title))
                throw new TestExceptions("Назва тесту не може бути порожньою.");

            if (_tests.Any(test => test.Title.Equals(test.Title, StringComparison.OrdinalIgnoreCase)))
                throw new TestExceptions("Такий тест вже існує.");

            _tests.Add(test);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditTestTitle(string oldTitle, string newTitle)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == oldTitle)
                ?? throw new TestExceptions("Тест не знайдено.");

            test.Title = newTitle;
            _ctx.TestsProvider.Save(_tests);
        }

        public void ChangeQuestionsCount(string title, int newCount)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");

            if (newCount < 1) throw new TestExceptions("Має бути хоча б 1 питання.");

            if (newCount < test.Questions.Count)
                test.Questions.RemoveRange(newCount, test.Questions.Count - newCount);
            _ctx.TestsProvider.Save(_tests);
        }

        public void UpdateTimePerQuestion(string title, int seconds)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            if (seconds < 5)
                throw new TestExceptions("Час на питання має бути ≥ 5 сек.");
            test.TimePerQuestionSeconds = seconds;
            _ctx.TestsProvider.Save(_tests);
        }

        public void DeleteTest(string title)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            _tests.Remove(test);
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Test> GetAll() => _tests;
        public List<Test> Search(string keyword)
            => _tests.Where(test => test.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

        public void AddQuestion(string testTitle, Question question)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");

            if (question.Answers != null)
            {
                question.Answers = question.Answers
                    .Where(ans => !string.IsNullOrWhiteSpace(ans.Text))
                    .ToList();
            }

            if (question.Answers == null || question.Answers.Count == 0)
            {
                question.Answers = new List<Answer>
        {
            new Answer("Відповідь 1", false),
            new Answer("Відповідь 2", false),
            new Answer("Правильна відповідь", true)
        };
            }
            else if (!question.Answers.Any(a => a.IsCorrect))
            {
                question.Answers[0].IsCorrect = true;
            }

            test.Questions.Add(question);
            _ctx.TestsProvider.Save(_tests);
        }



        public void DeleteQuestion(string testTitle, int index)
        {
            Test test = _tests.FirstOrDefault(test => test.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");

            if (index < 0 || index >= test.Questions.Count)
                throw new TestExceptions("Невірний номер питання.");

            test.Questions.RemoveAt(index);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditQuestion(string testTitle, int index, string newText)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            test.Questions[index].Text = newText;
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Question> GetQuestions(string testTitle)
        {
            Test test = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            return test.Questions;
        }

        public void AddAnswer(string testTitle, int qIndex, string text, bool isCorrect)
        {
            Test test = _tests.First(x => x.Title == testTitle);
            test.Questions[qIndex].Answers.Add(new Answer(text, isCorrect));
            _ctx.TestsProvider.Save(_tests);
        }

        public void DeleteAnswer(string testTitle, int qIndex, int aIndex)
        {
            Test test = _tests.First(x => x.Title == testTitle);
            test.Questions[qIndex].Answers.RemoveAt(aIndex);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditAnswer(string testTitle, int qIndex, int aIndex, string newText, bool isCorrect)
        {
            Answer ans = _tests.First(x => x.Title == testTitle).Questions[qIndex].Answers[aIndex];
            ans.Text = newText;
            ans.IsCorrect = isCorrect;
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Answer> GetAnswers(string testTitle, int qIndex)
        {
            Test test = _tests.First(x => x.Title == testTitle);
            return test.Questions[qIndex].Answers;
        }

        public TestResult RunTestInteractively(string title)
        {
            Test test = _tests.FirstOrDefault(t => t.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");

            if (test.Questions.Count == 0)
                throw new TestExceptions("У тесті немає питань.");

            Console.Write("\nВведіть ім’я: ");
            string name = Console.ReadLine() ?? "Студент";

            int correct = 0;
            for (int i = 0; i < test.Questions.Count; i++)
            {
                Question question = test.Questions[i];
                Console.WriteLine($"\nПитання {i + 1}: {question.Text}");
                for (int j = 0; j < question.Answers.Count; j++)
                    Console.WriteLine($"{j + 1}. {question.Answers[j].Text}");

                Console.Write("Ваша відповідь (0 - вихід): ");
                if (!int.TryParse(Console.ReadLine(), out int ch) || ch < 0 || ch > question.Answers.Count)
                {
                    Console.WriteLine("Некоректно, пропущено.");
                    continue;
                }
                if (ch == 0)
                {
                    Console.WriteLine("Тест завершено достроково.");
                    break;
                }
                if (question.Answers[ch - 1].IsCorrect) correct++;
            }

            double percent = 100.0 * correct / test.Questions.Count;

            TestResult result = new TestResult { TestTitle = title, StudentName = name, Percent = percent };
            
            _results.Add(result);
            _ctx.ResultsProvider.Save(_results);

            Console.WriteLine($"\nРезультат: {percent:F2}%");

            return result;
        }

        public List<TestResult> GetResults() => _results;
    }
}
