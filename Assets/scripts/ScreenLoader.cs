using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ScreenLoader : MonoBehaviour
{
    public GameObject eventObj;
    public Button btnA;
    public Button btnB;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject.DontDestroyOnLoad(this.gameObject);
        //GameObject.DontDestroyOnLoad(this.eventObj);
        btnA.onClick.AddListener(LoadSceneA);
        btnB.onClick.AddListener(LoadSceneB);
    }

    private void LoadSceneB()
    {
        StartCoroutine(LoadScene(2));
    }

    private void LoadSceneA()
    {
        StartCoroutine(LoadScene(1));
    }
    IEnumerator LoadScene(int index)
    {
        animator.SetBool("Fadein", true);
        animator.SetBool("Fadeout", false);
        yield return new WaitForSeconds(0);
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        async.completed += OnLoadedScene;
    }

    private void OnLoadedScene(AsyncOperation obj)
    {
        animator.SetBool("Fadein", false);
        animator.SetBool("Fadeout", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
