using Unity.Multiplayer.Center.Common;
using UnityEngine;

namespace ObjectSystem.StationSystem
{
    [CreateAssetMenu(fileName = "QuizData", menuName = "ScriptableObjects/QuizData")]
    public class QuizData : ScriptableObject
    {
        public int id;
        public string quizQuestion;
        public AnswerData[] quizAnswer;
    }
}
