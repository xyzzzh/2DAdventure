using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition; // 菜单状态下主角坐标
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;//本cs负责事件的监听
    public VoidEventSO newGameEvent;

    [Header("事件广播")] public VoidEventSO afterSceneLoadEvent;
    public FadeEventSO fadeEvent;
    public GameSceneSO currentLoadedScene;
    public SceneLoadEventSO sceneUnlodedEvent;
    
    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;


    private GameSceneSO sceneToLoad;
    private Vector3 posToGo;
    private bool isFadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        // Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        // currentLoadedScene = firstLoadScene;
        // firstLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        
    }

    private void Start()
    {
        // NewGame();
        loadEventSO.LoadRequestEvent(menuScene, menuPosition, true);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        // OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        loadEventSO.LoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    private void OnLoadRequestEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        if(isLoading)   return;
        
        isLoading = true;
        sceneToLoad = arg0;
        posToGo = arg1;
        isFadeScreen = arg2;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (isFadeScreen)
        {
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        // 广播事件，显示人物状态条
        sceneUnlodedEvent.RaiseLoadRequestEvent(sceneToLoad, posToGo, true);

        // 等待场景卸载结束再继续执行
        yield return currentLoadedScene.sceneReference.UnLoadScene();

        // 关闭人物
        playerTrans.gameObject.SetActive(false);
        // 加载新场景
        LoadNewScene();
    }

    public void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = sceneToLoad;
        playerTrans.position = posToGo;
        playerTrans.gameObject.SetActive(true);
        if (isFadeScreen)
        {
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        if (currentLoadedScene.sceneType == SceneType.Location)
        {
            // 完成场景加载后进行广播
            afterSceneLoadEvent.RaisedEvent();
        }
    }
}
