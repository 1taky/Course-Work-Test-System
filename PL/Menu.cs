using BLL.Services;
using BLL.Exceptions;
using DAL.Entities;

namespace PL;

public class Menu
{
    public void Run()
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
                        int time = ReadInt("Час на питання (мінімум 5 секунд): ", 5);

                        service.AddTest(new Test(title, time));
                        Console.WriteLine("*** Тест додано. ***");
                        break;

                    case "2":
                        foreach (Test test in service.GetAll()) Console.WriteLine(test);
                        break;

                    case "3":
                        Console.Write("Назва тесту для видалення: ");
                        service.DeleteTest(Console.ReadLine() ?? "");
                        Console.WriteLine("*** Тест видалено. ***");
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
                        foreach (Test test in service.Search(Console.ReadLine() ?? ""))
                            Console.WriteLine(test);
                        break;

                    case "8":
                        foreach (TestResult testRes in service.GetResults())
                            Console.WriteLine(testRes);
                        break;

                    case "9":
                        Console.Write("Назва тесту: ");
                        string testTitle = Console.ReadLine() ?? "";
                        int newTime = ReadInt("Новий час на питання (мінімум 5 секунд): ", 5);
                        service.UpdateTimePerQuestion(testTitle, newTime);
                        Console.WriteLine("*** Час оновлено. ***");
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
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

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
        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                Console.Write("Текст питання: ");
                string qestionText = Console.ReadLine() ?? "";
                List<Answer> answers = new List<Answer>();

                for (int i = 1; i <= 3; i++)
                {
                    Console.Write($"Відповідь {i}: ");
                    string ansText = Console.ReadLine() ?? "";
                    Console.Write("Правильна? (+/-): ");
                    bool correct = (Console.ReadLine() ?? "") == "+";
                    answers.Add(new Answer(ansText, correct));
                }
                service.AddQuestion(title, new Question(qestionText, answers));
                Console.WriteLine("*** Питання додано. ***");
                break;

            case "2":
                List<Question> questions = service.GetQuestions(title);

                for (int i = 0; i < questions.Count; i++)
                    Console.WriteLine($"{i + 1}. {questions[i].Text}");

                int del = ReadInt("Номер питання для видалення: ", 1, questions.Count) - 1;
                service.DeleteQuestion(title, del);
                Console.WriteLine("*** Питання видалено. ***");
                break;

            case "3":
                List<Question> qs = service.GetQuestions(title);

                for (int i = 0; i < qs.Count; i++)
                    Console.WriteLine($"{i + 1}. {qs[i].Text}");

                int edit = ReadInt("Номер питання: ", 1, qs.Count) - 1;
                Console.Write("Новий текст: ");
                service.EditQuestion(title, edit, Console.ReadLine() ?? "");
                Console.WriteLine("*** Питання оновлено. ***");
                break;

            case "4":
                foreach (Question question in service.GetQuestions(title))
                {
                    Console.WriteLine($"\n{question.Text}");
                    foreach (Answer ans in question.Answers)
                        Console.WriteLine($"  - {ans}");
                }
                break;
        }
    }

    static void ManageAnswers(TestService service)
    {
        Console.Write("Назва тесту: ");
        string title = Console.ReadLine() ?? "";
        List<Question> questions = service.GetQuestions(title);

        if (questions.Count == 0)
        {
            Console.WriteLine("*** У тесті немає питань. ***");
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
        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                Console.Write("Текст відповіді: ");
                string text = Console.ReadLine() ?? "";
                Console.Write("Правильна? (+/-): ");
                bool correct = (Console.ReadLine() ?? "") == "+";
                service.AddAnswer(title, qIndex, text, correct);
                Console.WriteLine("*** Відповідь додано. ***");
                break;

            case "2":
                List<Answer> ans = service.GetAnswers(title, qIndex);

                for (int i = 0; i < ans.Count; i++)
                    Console.WriteLine($"{i + 1}. {ans[i]}");

                int del = ReadInt("Номер відповіді для видалення: ", 1, ans.Count) - 1;
                service.DeleteAnswer(title, qIndex, del);
                Console.WriteLine("*** Відповідь видалено. ***");
                break;

            case "3":
                List<Answer> answers = service.GetAnswers(title, qIndex);

                for (int i = 1; i <= answers.Count; i++)
                    Console.WriteLine($"{i}. {answers[i]}");

                int edit = ReadInt("Номер відповіді: ", 1, answers.Count) - 1;
                Console.Write("Новий текст: ");
                string newText = Console.ReadLine() ?? "";
                Console.Write("Правильна? (+/-): ");
                bool newCorrect = (Console.ReadLine() ?? "") == "+";
                service.EditAnswer(title, qIndex, edit, newText, newCorrect);
                Console.WriteLine("*** Відповідь змінено. ***");
                break;

            case "4":
                foreach (Answer answer in service.GetAnswers(title, qIndex))
                    Console.WriteLine($"- {answer}");
                break;
        }
    }

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