using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : SceneBase
{
    LoadingScreenController loadingScreenController;

    public override void SceneAwake()
    {
        Debug.Log("LoadingScene Awake");

        loadingScreenController = GetComponent<LoadingScreenController>();

        loadingScreenController.LoadResources();
    }

    public override void SceneEnter()
    {
    }

    public override void SceneExit()
    {
    }
}
