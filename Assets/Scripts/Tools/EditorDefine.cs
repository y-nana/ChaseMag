using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Path
{
    // プレハブ取得用のパス
    public static class PrefabPath
    {
        public static readonly string waiPoint = "Assets/Prefabs/Map/WaiPoint.prefab";
        public static readonly string stageRoute = "Assets/Prefabs/Map/StageRoute.prefab";
        public const string jumpRamp = "Assets/Prefabs/StageGimmick/JumpRamp.prefab";
        public const string wall = "Assets/Prefabs/StageGimmick/Wall.prefab";
        public const string normalWall = "Assets/Prefabs/StageGimmick/NormalWall.prefab";
        public const string scaffold = "Assets/Prefabs/StageGimmick/Scaffold.prefab";
        public const string itemBox = "Assets/Prefabs/StageGimmick/itemBox.prefab";

    }



}


namespace Tag
{
    // タグ
    public static class PoleTag
    {
        public static readonly string north = "North";
        public static readonly string south = "South";

    }


}
namespace Layer
{
    public static class LayerNumber
    {
        public const int ignoreRaycast = 2;
        public const int scaffold = 9;
        public const int floor = 10;
        public const int waiPoint = 11;
        public const int wall = 12;
    }
}



