using BLL.Services;
using BLL.Exceptions;
using DAL.Entities;

namespace PL
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            TestService service = new TestService();

            while (true)
            {
                Console.WriteLine("\n=== СИСТЕМА ТЕСТУВАННЯ ===");
                Console.WriteLine("1 - Додати тест");
                Console.WriteLine("2 - Переглянути тести");
                Console.WriteLine("3 - Видалити тест");
                Console.WriteLine("4 - Керування питаннями");
                Console.WriteLine("5 - Керування відповідями");
                Console.WriteLine("6 - Пройти тест");
                Console.WriteLine("7 - Пошук тестів");
                Console.WriteLine("8 - Переглянути результати");
                Console.WriteLine("9 - Змінити час на одне питання");
                Console.WriteLine("0 - Вихід");
                Console.Write("Ваш вибір: ");
                string choice = Console.ReadLine() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("Назва тесту: ");
                            string title = Console.ReadLine() ?? "";
                            int time = ReadInt("Час/питання (сек, ≥5): ", 5);
                            service.AddTest(new Test(title, time));
                            Console.WriteLine("✅ Тест додано.");
                            break;

                        case "2":
                            foreach (var t in service.GetAll()) Console.WriteLine(t);
                            break;

                        case "3":
                            Console.Write("Назва тесту для видалення: ");
                            service.DeleteTest(Console.ReadLine() ?? "");
                            Console.WriteLine("✅ Видалено.");
                            break;

                        case "4":
                            ManageQuestions(service);
                            break;

                        case "5":
                            ManageAnswers(service);
                            break;

                        case "6":
                            Console.Write("Назва тесту: ");
                            service.RunTestInteractively(Console.ReadLine() ?? "");
                            break;

                        case "7":
                            Console.Write("Ключове слово: ");
                            foreach (var t in service.Search(Console.ReadLine() ?? ""))
                                Console.WriteLine(t);
                            break;

                        case "8":
                            foreach (var r in service.GetResults())
                                Console.WriteLine(r);
                            break;

                        case "9": // пункт 3.2.2
                            Console.Write("Назва тесту: ");
                            string tTitle = Console.ReadLine() ?? "";
                            int newTime = ReadInt("Новий час (сек, ≥5): ", 5);
                            service.UpdateTimePerQuestion(tTitle, newTime);
                            Console.WriteLine("✅ Час оновлено.");
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Невірний вибір.");
                            break;
                    }
                }
                catch (TestExceptions ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Помилка: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        // === КЕРУВАННЯ ПИТАННЯМИ ===
        static void ManageQuestions(TestService service)
        {
            Console.Write("Назва тесту: ");
            string title = Console.ReadLine() ?? "";
            Console.WriteLine("\n=== КЕРУВАННЯ ПИТАННЯМИ ===");
            Console.WriteLine("1 - Додати питання");
            Console.WriteLine("2 - Видалити питання");
            Console.WriteLine("3 - Редагувати питання");
            Console.WriteLine("4 - Переглянути питання");
            Console.Write("Ваш вибір: ");
            string c = Console.ReadLine() ?? "";

            switch (c)
            {
                case "1":
                    Console.Write("Текст питання: ");
                    string qText = Console.ReadLine() ?? "";
                    List<Answer> answers = new List<Answer>();
                    for (int i = 1; i <= 3; i++)
                    {
                        Console.Write($"Відповідь {i}: ");
                        string aText = Console.ReadLine() ?? "";
                        Console.Write("Правильна? (+/-): ");
                        bool correct = (Console.ReadLine() ?? "") == "+";
                        answers.Add(new Answer(aText, correct));
                    }
                    service.AddQuestion(title, new Question(qText, answers));
                    Console.WriteLine("✅ Питання додано.");
                    break;

                case "2":
                    List<Question> questions = service.GetQuestions(title);
                    for (int i = 0; i < questions.Count; i++)
                        Console.WriteLine($"{i + 1}. {questions[i].Text}");
                    int del = ReadInt("Номер питання для видалення: ", 1, questions.Count) - 1;
                    service.DeleteQuestion(title, del);
                    Console.WriteLine("✅ Питання видалено.");
                    break;

                case "3":
                    List<Question> qs = service.GetQuestions(title);
                    for (int i = 0; i < qs.Count; i++)
                        Console.WriteLine($"{i + 1}. {qs[i].Text}");
                    int edit = ReadInt("Номер питання: ", 1, qs.Count) - 1;
                    Console.Write("Новий текст: ");
                    service.EditQuestion(title, edit, Console.ReadLine() ?? "");
                    Console.WriteLine("✅ Питання оновлено.");
                    break;

                case "4":
                    foreach (var q in service.GetQuestions(title))
                    {
                        Console.WriteLine($"\n{q.Text}");
                        foreach (var a in q.Answers)
                            Console.WriteLine($"  - {a}");
                    }
                    break;
            }
        }

        // === КЕРУВАННЯ ВІДПОВІДЯМИ ===
        static void ManageAnswers(TestService service)
        {
            Console.Write("Назва тесту: ");
            string title = Console.ReadLine() ?? "";
            List<Question> questions = service.GetQuestions(title);
            if (questions.Count == 0)
            {
                Console.WriteLine("❌ У тесті немає питань.");
                return;
            }

            for (int i = 0; i < questions.Count; i++)
                Console.WriteLine($"{i + 1}. {questions[i].Text}");
            int qIndex = ReadInt("Оберіть питання: ", 1, questions.Count) - 1;

            Console.WriteLine("\n=== КЕРУВАННЯ ВІДПОВІДЯМИ ===");
            Console.WriteLine("1 - Додати відповідь");
            Console.WriteLine("2 - Видалити відповідь");
            Console.WriteLine("3 - Редагувати відповідь");
            Console.WriteLine("4 - Переглянути відповіді");
            Console.Write("Ваш вибір: ");
            string c = Console.ReadLine() ?? "";

            switch (c)
            {
                case "1":
                    Console.Write("Текст відповіді: ");
                    string text = Console.ReadLine() ?? "";
                    Console.Write("Правильна? (+/-): ");
                    bool correct = (Console.ReadLine() ?? "") == "+";
                    service.AddAnswer(title, qIndex, text, correct);
                    Console.WriteLine("✅ Відповідь додано.");
                    break;

                case "2":
                    List<Answer> ans = service.GetAnswers(title, qIndex);
                    for (int i = 0; i < ans.Count; i++)
                        Console.WriteLine($"{i + 1}. {ans[i]}");
                    int del = ReadInt("Номер відповіді для видалення: ", 1, ans.Count) - 1;
                    service.DeleteAnswer(title, qIndex, del);
                    Console.WriteLine("✅ Відповідь видалено.");
                    break;

                case "3":
                    List<Answer> answers = service.GetAnswers(title, qIndex);
                    for (int i = 0; i < answers.Count; i++)
                        Console.WriteLine($"{i + 1}. {answers[i]}");
                    int edit = ReadInt("Номер відповіді: ", 1, answers.Count) - 1;
                    Console.Write("Новий текст: ");
                    string newText = Console.ReadLine() ?? "";
                    Console.Write("Правильна? (+/-): ");
                    bool newCorrect = (Console.ReadLine() ?? "") == "+";
                    service.EditAnswer(title, qIndex, edit, newText, newCorrect);
                    Console.WriteLine("✅ Відповідь змінено.");
                    break;

                case "4":
                    foreach (var a in service.GetAnswers(title, qIndex))
                        Console.WriteLine($"- {a}");
                    break;
            }
        }

        // === допоміжний метод ===
        static int ReadInt(string prompt, int min, int? max = null)
        {
            int value;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out value)
                     || value < min
                     || (max.HasValue && value > max));
            return value;
        }
    }
}
