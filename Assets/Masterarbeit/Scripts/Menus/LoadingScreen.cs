using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;

    protected float targetProgress;
    public float fillSpeed;

    protected SceneIndex loadingScene;

    public TMPro.TextMeshProUGUI loadingHeaderText;
    public TMPro.TextMeshProUGUI loadingProgressText;


    public void SetLoadingSceneIndex(SceneIndex index)
    {
        loadingScene = index;
        if(loadingHeaderText != null)
        {
            loadingHeaderText.text = GetLoadingHeaderText(loadingScene);
        }
    }

    public string GetLoadingHeaderText(SceneIndex sceneIndex)
    {
        switch (sceneIndex)
        {
            case SceneIndex.MAIN_MENU:
                return "Menu";
            case SceneIndex.GAME_WORLD:
                return "World";
        }
        return "";
    }
    public string GetLoadingProgressText()
    {
        if(!SceneLoader.Instance.isSceneLoaded)
        {
            return "Loading scene: " + ((int)(SceneLoader.Instance.loadingProgress*100.0f)).ToString("n0") + "%";
        }
        if (!SceneLoader.Instance.isSceneSetUp)
        {
            return "Setting up scene: " + ((int)(SceneLoader.Instance.setupProgress * 100.0f)).ToString() + "%";
        }
        return "";
    }



    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    void OnEnable()
    {
        progressBar.value = 0.0f;
        targetProgress = 0.0f;
    }

    void Update()
    {
        UpdateProgress();
        UpdateLoadingProgressText();
    }

    void UpdateProgress()
    {
        targetProgress = SceneLoader.Instance.GetTotalProgress01();
        progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, Time.deltaTime * fillSpeed);
    }
    void UpdateLoadingProgressText()
    {
        loadingProgressText.text = GetLoadingProgressText();
    }

}
