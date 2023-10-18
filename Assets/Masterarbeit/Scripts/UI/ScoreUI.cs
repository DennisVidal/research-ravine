using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Slider highscoreProgressSlider;

    protected int score = 0;

    public float addRate = 10.0f;
    protected float scoreToAdd;


    void Update()
    {
        scoreToAdd += addRate * Time.deltaTime;
        int scoreToAddInt = (int)scoreToAdd;

        if(scoreToAddInt != 0)
        {
            SetScore(Mathf.Clamp(score + scoreToAddInt, 0, GameManager.Instance.currentScore));
            scoreToAdd -= scoreToAddInt;
        }
    }
    public void SetScore(int score)
    {
        this.score = score;
        textComponent.text = score.ToString();
        highscoreProgressSlider.normalizedValue = score / Mathf.Max(GameManager.Instance.GetHighscore(), 1.0f);
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        SetScore(0);
    }
}
