namespace DAL.Entities
{
    [Serializable]
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<Answer> Answers { get; set; } = new();

        public Question() { }

        public Question(string text)
        {
            Text = text;
        }
        public Question(string text, List<Answer> answers)
        {
            Text = text;
            Answers = answers;
        }

        public override string ToString() => $"Питання: {Text} | (Відповідей: {Answers.Count})";
    }
}
