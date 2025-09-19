using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScene : SceneBase
{
    public override void SceneAwake()
    {
        
    }

    public override void SceneEnter()
    {
        StageManager.Instance.InitializeStage();
    }

    public override void SceneExit()
    {
    }
}
