using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneIndex
{
    MAIN_MENU = 0,
    GAME_WORLD = 1
}
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public SceneIndex lastSceneIndex;
    public SceneIndex currentSceneIndex;
    public bool switchingScene;
    public bool isSceneLoaded;
    public bool isSceneSetUp;
    public float loadingProgress;
    public float setupProgress;
    //                  Old,        New
    public event Action<SceneIndex, SceneIndex> onStartedLoading; 
    public event Action<SceneIndex, SceneIndex> onFinishedLoading;
    public event Action<SceneIndex, SceneIndex> onFinishedSetup;  

    public LoadingScreen loadingScreen;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMainMenu()
    {
        LoadScene(SceneIndex.MAIN_MENU);
    }
    public void LoadWorld()
    {
        LoadScene(SceneIndex.GAME_WORLD);
    }


    public void LoadScene(SceneIndex sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    IEnumerator LoadSceneCoroutine(SceneIndex sceneIndex)
    {
        loadingProgress = 0.0f;
        setupProgress = 0.0f;
        isSceneLoaded = false;
        isSceneSetUp = false;
        OnStartedLoading(sceneIndex);

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync((int)sceneIndex);

        while(!isSceneLoaded)
        {
            loadingProgress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            isSceneLoaded = loadingOperation.isDone;
            yield return null;
        }
        OnFinishedLoading(sceneIndex);
    }


    void StartSceneSetup(SceneIndex sceneIndex)
    {
        StartCoroutine(SceneSetupCoroutine(sceneIndex));
    }

    IEnumerator SceneSetupCoroutine(SceneIndex sceneIndex)
    {
        while (!isSceneSetUp)
        {
            UpdateSetupProgress(sceneIndex);
            yield return null;
        }
        OnFinishedSetup(sceneIndex);
    }

    void UpdateSetupProgress(SceneIndex sceneIndex)
    {
        switch (sceneIndex)
        {
            case SceneIndex.MAIN_MENU:
                setupProgress = 1.0f;
                isSceneSetUp = true;
                break;
            case SceneIndex.GAME_WORLD:
                setupProgress = ProceduralWorld.Instance != null ? ProceduralWorld.Instance.worldGenerationProgress : 0.0f;
                isSceneSetUp = ProceduralWorld.Instance != null ? ProceduralWorld.Instance.isWorldGenerated : false;
                break;
        }
    }

    public float GetTotalProgress01()
    {
        return Mathf.Clamp01(0.5f * (loadingProgress + setupProgress));
    }




    void OnStartedLoading(SceneIndex sceneIndex)
    {
        lastSceneIndex = currentSceneIndex;
        currentSceneIndex = sceneIndex;
        if (onStartedLoading != null)
        {
            onStartedLoading.Invoke(lastSceneIndex, sceneIndex);
        }
        loadingScreen.Show(true);
    }
    void OnFinishedLoading(SceneIndex sceneIndex)
    {
        if (onFinishedLoading != null)
        {
            onFinishedLoading.Invoke(lastSceneIndex, sceneIndex);
        }
        StartSceneSetup(sceneIndex);
    }

    void OnFinishedSetup(SceneIndex sceneIndex)
    {
        loadingScreen.Show(false);
        if (onFinishedSetup != null)
        {
            onFinishedSetup.Invoke(lastSceneIndex, sceneIndex);
        }
    }
}
