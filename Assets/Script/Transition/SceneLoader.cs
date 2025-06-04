using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Event Listening")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO map01Event;
    public VoidEventSO map02Event;
    public VoidEventSO menuEvent;
    public FadeEventSO fadeEvent;

    [Header("Scene")]
    public GameSceneSO Map01Scene;
    public GameSceneSO Map02Scene;
    public GameSceneSO menuScene;

    private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;

    private bool fadeScreen;
    public float fadeDuration;

    private bool isLoading;

    private void Awake()
    {
        //currentLoadedScene = menuScene;
        //currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);

        //TODO
    }
    private void Start()
    {
        Debug.Log("SceneLoader Start: Attempting to load menuScene");
        if (loadEventSO == null)
        {
            Debug.LogError("loadEventSO is null!");
        }
        loadEventSO.RaiseLoadRequestEvent(menuScene, true);
        //Map01Game();
    }

    private void OnEnable()
    {

        Debug.Log("Attempting to subscribe");

        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        map01Event.OnEventRaised += Map01Game;
        map02Event.OnEventRaised += Map02Game;
        menuEvent.OnEventRaised += ReturnToMenu;

    }


    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        map01Event.OnEventRaised -= Map01Game;
        map02Event.OnEventRaised -= Map02Game;
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, bool fadeScreen)
    {
        Debug.Log("subscribe function");

        if (isLoading)
        {
            return;
        }

        isLoading = true;
        sceneToLoad = locationToLoad;
        this.fadeScreen = fadeScreen;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //Turn Black
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);


        yield return currentLoadedScene.sceneReference.UnLoadScene();
        
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = sceneToLoad;

        if (fadeScreen)
        {
            //Turn Transparent
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;
    }

    private void Map01Game()
    {
        Debug.Log("map01");
        sceneToLoad = Map01Scene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad,true);
    }
    private void Map02Game()
    {
        Debug.Log("map02");
        sceneToLoad = Map02Scene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, true);
    }

    private void ReturnToMenu()
    {
        Debug.Log("menu");
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, true);
    }

}

