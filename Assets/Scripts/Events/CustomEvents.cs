using Assets.Scripts.ScriptableObjets.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    // Store all the InGame events
    public static class CustomEvents 
    {
        public static GameEvent<Vector3> TileClickEvent = new GameEvent<Vector3>();
        public static GameEvent<Vector3> TileHoverEvent = new GameEvent<Vector3>();
        public static GameEvent TileExitEvent = new GameEvent();
        public static GameEvent<int,List<Vector3>> UnitMovementEvent = new GameEvent<int,List<Vector3>>();
        public static GameEvent<int,bool> UnitMovementStatusEvent = new GameEvent<int,bool>();

        // int : GO InstanceID - bool : isPlayerControlledUnit
        public static GameEvent<int,bool> StartTurnEvent = new GameEvent<int,bool>();
        public static GameEvent<int> EndTurnEvent = new GameEvent<int>();
        public static GameEvent EndCombat = new GameEvent();

        public static GameEvent<Ability,GameObject> SelectAbilityEvent = new GameEvent<Ability, GameObject>();

        public static GameEvent<int> UnitClickEvent = new GameEvent<int>();
        public static GameEvent UnselectAbilityEvent = new GameEvent();

        public static GameEvent<int, int> DamageUnitEvent = new GameEvent<int, int>();
        // int : GO InstanceId
        public static GameEvent<int> UnitDeathEvent = new GameEvent<int>();
    }
}