namespace DAL.Entities
{
    [Serializable]
    public class Test
    {
        public string Title { get; set; } = string.Empty;
        public int TimePerQuestionSeconds { get; set; } = 60;
        public List<Question> Questions { get; set; } = new();

        public Test() { }

        public Test(string str)
        {
            Title = str;
        }
        public Test(string title, int timeForQuestion) 
        {
            Title = title;
            TimePerQuestionSeconds = timeForQuestion;
        }

        public override string ToString()
            => $"Тест: {Title} | Питань: {Questions.Count} | Час на питання: {TimePerQuestionSeconds}сек.";
    }
}
