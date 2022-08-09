using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface SceneCaller : IEventSystemHandler
{

    // ゲットコンポーネントせずに
    // メソッドを呼び出すためのインタフェース
    // SceneDirectorに実装
    void ToGameOver();

    void ToGameClear();

}
