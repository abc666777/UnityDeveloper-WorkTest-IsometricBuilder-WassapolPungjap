using System.Collections.Generic;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace ObjectSystem.StationSystem
{
    public class Station2Ui : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private QuizData[] quizData;
        [SerializeField] private GameObject quitPrefabs;
        
        [SerializeField] private Button closeButton;

        [SerializeField] private RectTransform quizRoot;

        [SerializeField] private TextMeshProUGUI quizText;
        [SerializeField] private GameObject passText;
        [SerializeField] private RectTransform answerRoot;
        
        public static UnityAction ShowMenu;
        
        private Dictionary<int, QuizData> _quizDatas = new Dictionary<int, QuizData>();
        private Dictionary<int, bool> _isAnswerCorrectly = new Dictionary<int, bool>();
        private QuizData _currentQuiz;

        private void Awake()
        {
            foreach (var quiz in quizData)
            {
                _quizDatas.Add(quiz.id, quiz);
                
                var newButton = Instantiate(quitPrefabs, quizRoot);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = quiz.quizQuestion;
                newButton.GetComponent<Button>().onClick.AddListener(() => ShowDisplay(quiz));
            }
            
            ShowMenu += Show;
            closeButton.onClick.AddListener(Close);

            var loadQuiz = SaveController.LoadQuiz();
            
            if(loadQuiz != null)
                _isAnswerCorrectly = new Dictionary<int, bool>(loadQuiz);

            rootObject.SetActive(false);
        }
        
        private void Show()
        {
            rootObject.SetActive(true);
            ShowDisplay(_currentQuiz ? _currentQuiz : quizData[0]);
        }
        
        private void Close()
        {
            rootObject.SetActive(false);
            Helper.onCloseUi.Invoke();
        }

        private void ShowDisplay(QuizData quizData)
        {
            quizText.text = quizData.quizQuestion;
            _currentQuiz = quizData;
            UpdateAnswerButton();
            UpdateQuestionState();
        }

        private void UpdateAnswerButton()
        {
            for (var i = answerRoot.childCount - 1; i >= 0; i--)
                Destroy(answerRoot.GetChild(i).gameObject);

            foreach (var answer in _currentQuiz.quizAnswer)
            {
                var answerButton = Instantiate(quitPrefabs, answerRoot);
                answerButton.GetComponentInChildren<TextMeshProUGUI>().text = answer.answerText;
                answerButton.GetComponent<Button>().onClick.AddListener(() => Answer(answer.isCorrectAnswer));
            }
        }

        private void Answer(bool isAnswerCorrect)
        {
            if(!isAnswerCorrect) return;
            if(!_isAnswerCorrectly.ContainsKey(_currentQuiz.id))
                _isAnswerCorrectly.Add(_currentQuiz.id, true);

            UpdateQuestionState();
            SaveController.SaveQuiz(_isAnswerCorrectly);
        }

        private void UpdateQuestionState()
        {
            var isAnswerCorrect = false;

            if (_isAnswerCorrectly.ContainsKey(_currentQuiz.id))
                isAnswerCorrect = _isAnswerCorrectly.ContainsKey(_currentQuiz.id);

            answerRoot.gameObject.SetActive(!isAnswerCorrect);
            passText.gameObject.SetActive(isAnswerCorrect);  
        }
    }
}
