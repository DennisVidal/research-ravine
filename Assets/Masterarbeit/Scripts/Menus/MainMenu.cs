using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;

    public TMPro.TextMeshProUGUI highscoreValueText;


    void OnEnable()
    {
        UpdateHighscoreText();
    }
    public void Play()
    {
        SceneLoader.Instance.LoadWorld();
    }
    public void PlayLast()
    {
        Play();
    }
    public void PlayRandom()
    {
        SaveSystem.SAVE_DATA.SetLastSeed(Random.Range(int.MinValue, int.MaxValue));
        SaveSystem.Save();
        Play();
    }

    public void Quit()
    {
        SaveSystem.Save();
        Application.Quit();
    }

    public void UpdateHighscoreText()
    {
        highscoreValueText.text = SaveSystem.SAVE_DATA.GetHighscore().ToString();
    }
}
