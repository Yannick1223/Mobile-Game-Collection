using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HighscoreGame1Manager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown DifficultDropdown;
    [SerializeField]
    private TextMeshProUGUI HighscoreText;
    [SerializeField]
    private TextMeshProUGUI ScoreText;

    private int Highscore;
    private int CurrentScore;
    private string CurrentDifficult;

    private void Start()
    {
        ChangeHighscore();
    }

    public void ChangeHighscore()
    {
        switch (DifficultDropdown.value)
        {
            case 0://Easy
                CurrentDifficult = "Easy";
                break;
            case 1://Normal
                CurrentDifficult = "Normal";
                break;
            case 2://Hard
                CurrentDifficult = "Hard";
                break;
            case 3://Imposible
                CurrentDifficult = "Imposible";
                break;
            case 4://Custom
                CurrentDifficult = "Custom";
                break;
        }

        if (PlayerPrefs.HasKey(CurrentDifficult + " Highscore Game1"))
        {
            Highscore = PlayerPrefs.GetInt(CurrentDifficult + " Highscore Game1");
        }
        else
        {
            Highscore = 0;
        }
        UpdateText();
    }

    public void AddScore(int value)
    {
        CurrentScore += value;
        Highscore = (Highscore > CurrentScore)? Highscore: CurrentScore;
        PlayerPrefs.SetInt(CurrentDifficult + " Highscore Game1", Highscore);

        UpdateText();
    }

    public void SetScore(int value)
    {
        CurrentScore = value;
        UpdateText();
    }

    private void UpdateText()
    {
        HighscoreText.text = "HighScore: " + Highscore;
        ScoreText.text = CurrentScore.ToString();
    }
}