using UnityEngine;

namespace ObjectSystem.StationSystem
{
    [CreateAssetMenu(fileName = "AnswerData", menuName = "ScriptableObjects/AnswerData")]
    public class AnswerData : ScriptableObject
    {
        public string answerText;
        public bool isCorrectAnswer;
    }
}
