using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;//本cs负责事件的监听

    public GameSceneSO currentLoadedScene;
    public GameSceneSO firstLoadScene;

    private GameSceneSO tempGameSceneSO;
    private Vector3 tempVector3;
    private bool tempFadeScreen;
    public float fadeDuration;

    private void Awake()
    {
        // Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        currentLoadedScene = firstLoadScene;
        firstLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        tempGameSceneSO = arg0;
        tempVector3 = arg1;
        tempFadeScreen = arg2;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }

        
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (tempFadeScreen)
        {
            // TODO:实现渐入渐出
        }

        yield return new WaitForSeconds(fadeDuration);

        // 等待场景卸载结束再继续执行
        yield return currentLoadedScene.sceneReference.UnLoadScene();

        LoadNewScene();
    }

    public void LoadNewScene()
    {
        tempGameSceneSO.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
    }
}
