using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerializableQuestionsData
{
    public List<QuestionEntry> questions;

    public Dictionary<string, Dictionary<string, string>> ToDictionary()
    {
        var dict = new Dictionary<string, Dictionary<string, string>>();
        foreach (var entry in questions)
        {
            dict[entry.question] = entry.answers.ToDictionary(a => a.animal, a => a.answer);
        }
        return dict;
    }
    
    [Serializable]
    public class QuestionEntry
    {
        public string question;
        public List<AnswerEntry> answers;
    }

    [Serializable]
    public class AnswerEntry
    {
        public string animal;
        public string answer;
    }
}
