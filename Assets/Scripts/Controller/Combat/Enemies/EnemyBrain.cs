﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Events;

namespace Assets.Scripts.Controller.Combat.Enemies
{
    [RequireComponent(typeof(CombatUnitController))]
    public class EnemyBrain : MonoBehaviour
    {
        CombatUnitController _combatUnitController;

        private void Start()
        {
            _combatUnitController = GetComponent<CombatUnitController>();
        }

        private void OnEnable()
        {
            CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
            CustomEvents.UnitMovementStatusEvent.AddListener(OnMovementStatusEvent);
        }

        private void OnDisable()
        {
            CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
            CustomEvents.UnitMovementStatusEvent.RemoveListener(OnMovementStatusEvent);
        }

        void OnTurnStart(int instanceId, bool isPlayerControlled) 
        {
            var i = gameObject.GetInstanceID();
            var e = gameObject.name;

            if (instanceId != gameObject.GetInstanceID() || isPlayerControlled) return;

            //Find a target
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            List<GameObject> targets = units.ToList().FindAll((unit) => unit.GetComponent<CombatUnitController>().UnitInfo.TeamId != _combatUnitController.UnitInfo.TeamId);

            List<Vector3> shortestPath = null;

            foreach(GameObject target in targets)
            {
                var targetPath = CombatController.Map.GetShortestPathToAdjacent(gameObject.transform.position, target.transform.position);
                if (shortestPath == null)
                {
                    shortestPath = targetPath;
                } else
                {
                    shortestPath = targetPath.Count < shortestPath.Count ? targetPath : shortestPath;
                }
            }

            if(shortestPath == null)
            {
                CustomEvents.EndTurnEvent.Invoke(gameObject.GetInstanceID());
                return;
            }


            if(shortestPath.Count > _combatUnitController.UnitInfo.Stats.MovementPoint)
            {
                shortestPath = shortestPath.Take(_combatUnitController.UnitInfo.Stats.MovementPoint).ToList();
            }

            CustomEvents.UnitMovementEvent.Invoke(gameObject.GetInstanceID(), shortestPath);

        }

        void OnMovementStatusEvent(int instanceId, bool isMoving)
        {
            if (gameObject.GetInstanceID() != instanceId) return;

            if(!isMoving)
            {
                CustomEvents.EndTurnEvent.Invoke(gameObject.GetInstanceID());
            }
        }
    }
}