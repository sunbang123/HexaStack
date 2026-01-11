using UnityEngine;
using TMPro;

namespace HexaStack.Models
{
    public class ScoreModel
    {
        private int score;

        public int Score
        {
            get => score;
            private set => score = value;
        }

        public void AddScore(int points)
        {
            score += points;
        }

        public void ResetScore()
        {
            score = 0;
        }
    }

    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        
        private ScoreModel scoreModel;

        private void Start()
        {
            scoreModel = new ScoreModel();
            UpdateScoreText();
        }

        public void AddScore(int points)
        {
            scoreModel.AddScore(points);
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + scoreModel.Score.ToString();
            }
        }

        public void ResetScore()
        {
            scoreModel.ResetScore();
            UpdateScoreText();
        }
    }
}