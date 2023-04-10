using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    /// <summary>
    /// 场景加载请求
    /// </summary>
    /// <param name="locationToGo">要加载的场景</param>
    /// <param name="posToGo">新场景中人物坐标</param>
    /// <param name="isFadeScreen">是否渐入渐出</param>
    public void RaiseLoadRequestEvent(GameSceneSO locationToGo, Vector3 posToGo, bool isFadeScreen)
    {
        LoadRequestEvent?.Invoke(locationToGo, posToGo, isFadeScreen);
    }
}