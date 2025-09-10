using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 씬 이름과 씬 베이스를 상속받는 클래스의 이름을 같게 해주세요.
/// </summary>
public abstract class SceneBase : MonoBehaviour
{

    public abstract void SceneEnter();
    public abstract void SceneExit();
    public abstract void SceneAwake();
}
