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

        // === КЕРУВАННЯ ТЕСТАМИ ===
        public void AddTest(Test test)
        {
            if (string.IsNullOrWhiteSpace(test.Title))
                throw new TestExceptions("Назва тесту не може бути порожньою.");
            if (_tests.Any(t => t.Title.Equals(test.Title, StringComparison.OrdinalIgnoreCase)))
                throw new TestExceptions("Такий тест вже існує.");
            _tests.Add(test);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditTestTitle(string oldTitle, string newTitle)
        {
            var t = _tests.FirstOrDefault(x => x.Title == oldTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            t.Title = newTitle;
            _ctx.TestsProvider.Save(_tests);
        }

        public void ChangeQuestionsCount(string title, int newCount)
        {
            var t = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            if (newCount < 1) throw new TestExceptions("Має бути хоча б 1 питання.");
            if (newCount < t.Questions.Count)
                t.Questions.RemoveRange(newCount, t.Questions.Count - newCount);
            _ctx.TestsProvider.Save(_tests);
        }

        public void UpdateTimePerQuestion(string title, int seconds)
        {
            var t = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            if (seconds < 5)
                throw new TestExceptions("Час на питання має бути ≥ 5 сек.");
            t.TimePerQuestionSeconds = seconds;
            _ctx.TestsProvider.Save(_tests);
        }

        public void DeleteTest(string title)
        {
            var t = _tests.FirstOrDefault(x => x.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            _tests.Remove(t);
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Test> GetAll() => _tests;
        public List<Test> Search(string keyword)
            => _tests.Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

        // === КЕРУВАННЯ ПИТАННЯМИ ===
        public void AddQuestion(string testTitle, Question question)
        {
            var t = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");

            // --- пункт 2.5.1 ---
            if (question.Answers == null || question.Answers.Count == 0)
            {
                question.Answers = new List<Answer>
                {
                    new Answer("Варіант 1", false),
                    new Answer("Варіант 2", false),
                    new Answer("Правильна відповідь", true)
                };
            }
            else if (!question.Answers.Any(a => a.IsCorrect))
            {
                question.Answers[0].IsCorrect = true;
            }
            // --------------------

            t.Questions.Add(question);
            _ctx.TestsProvider.Save(_tests);
        }

        public void DeleteQuestion(string testTitle, int index)
        {
            var t = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            if (index < 0 || index >= t.Questions.Count)
                throw new TestExceptions("Невірний номер питання.");
            t.Questions.RemoveAt(index);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditQuestion(string testTitle, int index, string newText)
        {
            var t = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            t.Questions[index].Text = newText;
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Question> GetQuestions(string testTitle)
        {
            var t = _tests.FirstOrDefault(x => x.Title == testTitle)
                ?? throw new TestExceptions("Тест не знайдено.");
            return t.Questions;
        }

        // === КЕРУВАННЯ ВІДПОВІДЯМИ ===
        public void AddAnswer(string testTitle, int qIndex, string text, bool isCorrect)
        {
            var t = _tests.First(x => x.Title == testTitle);
            t.Questions[qIndex].Answers.Add(new Answer(text, isCorrect));
            _ctx.TestsProvider.Save(_tests);
        }

        public void DeleteAnswer(string testTitle, int qIndex, int aIndex)
        {
            var t = _tests.First(x => x.Title == testTitle);
            t.Questions[qIndex].Answers.RemoveAt(aIndex);
            _ctx.TestsProvider.Save(_tests);
        }

        public void EditAnswer(string testTitle, int qIndex, int aIndex, string newText, bool isCorrect)
        {
            var a = _tests.First(x => x.Title == testTitle).Questions[qIndex].Answers[aIndex];
            a.Text = newText;
            a.IsCorrect = isCorrect;
            _ctx.TestsProvider.Save(_tests);
        }

        public List<Answer> GetAnswers(string testTitle, int qIndex)
        {
            var t = _tests.First(x => x.Title == testTitle);
            return t.Questions[qIndex].Answers;
        }

        // === ПРОХОДЖЕННЯ ТЕСТУ ===
        public TestResult RunTestInteractively(string title)
        {
            var test = _tests.FirstOrDefault(t => t.Title == title)
                ?? throw new TestExceptions("Тест не знайдено.");
            if (test.Questions.Count == 0)
                throw new TestExceptions("У тесті немає питань.");

            Console.Write("\nВведіть ім’я: ");
            string name = Console.ReadLine() ?? "Студент";

            int correct = 0;
            for (int i = 0; i < test.Questions.Count; i++)
            {
                var q = test.Questions[i];
                Console.WriteLine($"\nПитання {i + 1}: {q.Text}");
                for (int j = 0; j < q.Answers.Count; j++)
                    Console.WriteLine($"{j + 1}. {q.Answers[j].Text}");

                Console.Write("Ваша відповідь (0 - вихід): ");
                if (!int.TryParse(Console.ReadLine(), out int ch) || ch < 0 || ch > q.Answers.Count)
                {
                    Console.WriteLine("Некоректно, пропущено.");
                    continue;
                }
                if (ch == 0)
                {
                    Console.WriteLine("Тест завершено достроково.");
                    break;
                }
                if (q.Answers[ch - 1].IsCorrect) correct++;
            }

            double percent = 100.0 * correct / test.Questions.Count;
            var result = new TestResult { TestTitle = title, StudentName = name, Percent = percent };
            _results.Add(result);
            _ctx.ResultsProvider.Save(_results);
            Console.WriteLine($"\nРезультат: {percent:F2}%");
            return result;
        }

        public List<TestResult> GetResults() => _results;
    }
}
