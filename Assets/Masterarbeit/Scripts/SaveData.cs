
using UnityEngine;
using System;
using System.IO;

public class SaveSystem
{
    public static SaveData SAVE_DATA;
    public static string SAVE_PATH;

    public static event Action onSettingsReset;
    public static event Action onSave;
    public static void Init()
    {
        if (SAVE_DATA == null)
        {
            SAVE_PATH = Application.persistentDataPath + "/SaveData.json";
            SAVE_DATA = new SaveData();
        }
    }

    public static void Load()
    {
        if (File.Exists(SAVE_PATH))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(SAVE_PATH), SAVE_DATA);
            return;
        }

        ResetSettings();
        Save();
    }

    public static void Save()
    {
        File.WriteAllText(SAVE_PATH, JsonUtility.ToJson(SAVE_DATA));

        if(onSave != null)
        {
            onSave.Invoke();
        }
    }

    public static void ResetSettings()
    {
        SAVE_DATA.ResetSettings();

        if (onSettingsReset != null)
        {
            onSettingsReset.Invoke();
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int highscore;
    public int lastSeed;
    public int missionTime;
    public float sensitivityX;
    public float sensitivityZ;
    public int volume;
    public int selectedDifficulty;
    public int selectedColorMode;

    public void ResetSettings()
    {
        missionTime = 3;
        sensitivityX = 1.5f;
        sensitivityZ = 2.0f;
        volume = 100;
        selectedDifficulty = 0;
        selectedColorMode = 0;
    }



    public void SetHighscore(int newHighscore)
    {
        highscore = newHighscore;
    }
    public int GetHighscore()
    {
        return highscore;
    }

    public void SetLastSeed(int seed)
    {
        lastSeed = seed;
    }
    public int GetLastSeed()
    {
        return lastSeed;
    }

    public void SetMissionTime(int timeInMinutes)
    {
        missionTime = timeInMinutes;
    }
    public int GetMissionTimeMinutes()
    {
        return missionTime;
    }
    public int GetMissionTimeSeconds()
    {
        return GetMissionTimeMinutes() * 60;
    }

    public void SetPitchSensitivity(float sensitivity)
    {
        sensitivityX = sensitivity;
    }
    public float GetPitchSensitivity()
    {
        return sensitivityX;
    }

    public void SetRollSensitivity(float sensitivity)
    {
        sensitivityZ = sensitivity;
    }
    public float GetRollSensitivity()
    {
        return sensitivityZ;
    }

    public void SetAudioVolume(int audioVolume)
    {
        volume = audioVolume;
    }
    public int GetAudioVolume()
    {
        return volume;
    }
    public float GetAudioVolumeFloat()
    {
        return volume*0.01f;
    }

    public void SetDifficulty(int difficulty)
    {
        selectedDifficulty = difficulty;
    }
    public int GetDifficultyInt()
    {
        return selectedDifficulty;
    }
    public Difficulty GetDifficulty()
    {
        return (Difficulty)selectedDifficulty;
    }

    public void SetColorMode(int colorMode)
    {
        selectedColorMode = colorMode;
    }
    public int GetColorMode()
    {
        return selectedColorMode;
    }


    public int GetSettingInt(SettingType settingType)
    {
        int value = 0;
        switch (settingType)
        {
            case SettingType.HIGHSCORE:
                value = GetHighscore();
                break;

            case SettingType.LAST_SEED:
                value = GetLastSeed();
                break;

            case SettingType.MISSION_TIME_MINUTES:
                value = GetMissionTimeMinutes();
                break;
            case SettingType.MISSION_TIME_SECONDS:
                value = GetMissionTimeSeconds();
                break;
            case SettingType.VOLUME:
                value = GetAudioVolume();
                break;
            case SettingType.DIFFICULTY:
                value = GetDifficultyInt();
                break;
            case SettingType.COLOR_MODE:
                value = GetColorMode();
                break;
        }
        return value;
    }
    public float GetSettingFloat(SettingType settingType)
    {
        float value = -1.0f;
        switch (settingType)
        {
            case SettingType.SENSITIVITY_X:
                value = GetPitchSensitivity();
                break;
            case SettingType.SENSITIVITY_Z:
                value = GetRollSensitivity();
                break;
            case SettingType.VOLUME:
                value = GetAudioVolumeFloat();
                break;
        }
        return value;
    }
    public bool GetSettingBool(SettingType settingType)
    {
        bool value = false;
        //switch (settingType)
        //{
        //}
        return value;
    }
    public string GetSettingAsString(SettingType settingType)
    {
        string str = "";

        switch (settingType)
        {
            case SettingType.HIGHSCORE:
                str = GetHighscore().ToString();
                break;
            case SettingType.LAST_SEED:
                str = GetLastSeed().ToString();
                break;
            case SettingType.MISSION_TIME_MINUTES:
                str = GetMissionTimeMinutes().ToString();
                break;
            case SettingType.MISSION_TIME_SECONDS:
                str = GetMissionTimeSeconds().ToString();
                break;
            case SettingType.SENSITIVITY_X:
                str = GetPitchSensitivity().ToString("n2");
                break;
            case SettingType.SENSITIVITY_Z:
                str = GetRollSensitivity().ToString("n2");
                break;
            case SettingType.VOLUME:
                str = GetAudioVolume().ToString();
                break;
        }
        return str;
    }
}


public enum SettingType
{
    HIGHSCORE,
    LAST_SEED,
    MISSION_TIME_MINUTES,
    MISSION_TIME_SECONDS,
    SENSITIVITY_X,
    SENSITIVITY_Z,
    VOLUME,
    DIFFICULTY,
    COLOR_MODE,
    INSTANT_CRASH_FADE
}