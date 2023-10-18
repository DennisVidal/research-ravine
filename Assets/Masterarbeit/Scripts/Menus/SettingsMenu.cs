using UnityEngine;


public class SettingsMenu : MonoBehaviour
{
    public void ClearHighscore()
    {
        SaveSystem.SAVE_DATA.SetHighscore(0);
    }

    public void SetMissionLength(int minutes)
    {
        SaveSystem.SAVE_DATA.SetMissionTime(minutes);
    }

    public void SetSensitivityX(float sensitivity)
    {
        SaveSystem.SAVE_DATA.SetPitchSensitivity(sensitivity);
    }
    public void SetSensitivityZ(float sensitivity)
    {
        SaveSystem.SAVE_DATA.SetRollSensitivity(sensitivity);
    }

    public void SetVolume(int volume)
    {
        SaveSystem.SAVE_DATA.SetAudioVolume(volume);
        AudioListener.volume = SaveSystem.SAVE_DATA.GetAudioVolumeFloat();
    }

    public void SetDifficulty(int difficulty)
    {
        SaveSystem.SAVE_DATA.SetDifficulty(difficulty);
        GameManager.Instance.selectedDifficulty = SaveSystem.SAVE_DATA.GetDifficulty();
    }
    public void SetColorMode(int colorMode)
    {
        SaveSystem.SAVE_DATA.SetColorMode(colorMode);
        GameManager.Instance.selectedColorMode = SaveSystem.SAVE_DATA.GetColorMode();
    }

    public void SaveSettings()
    {
        SaveSystem.Save();
    }
    public void ResetSettings()
    {
        SaveSystem.ResetSettings();
    }
}
