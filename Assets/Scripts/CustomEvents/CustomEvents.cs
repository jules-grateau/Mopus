using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CustomEvents
{
    // Store all the InGame events
    public static class CustomEvents 
    {
        public static GameEvent<Vector3> TileClickEvent = new GameEvent<Vector3>();
        public static GameEvent<Vector3> TileHoverEvent = new GameEvent<Vector3>();
        public static GameEvent TileExitEvent = new GameEvent();
        public static GameEvent<List<Vector3>> UnitMovementEvent = new GameEvent<List<Vector3>>();
        public static GameEvent<List<Vector3>> UnitMovementPreviewEvent = new GameEvent<List<Vector3>>();
        public static GameEvent UnitMovementPreviewClearEvent = new GameEvent();
        public static GameEvent<bool> UnitMovementStatusEvent = new GameEvent<bool>();
    }
}