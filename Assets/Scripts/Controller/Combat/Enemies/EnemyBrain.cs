using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            CustomEvents.CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
            CustomEvents.CustomEvents.UnitMovementStatusEvent.AddListener(OnMovementStatusEvent);
        }

        private void OnDisable()
        {
            CustomEvents.CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
            CustomEvents.CustomEvents.UnitMovementStatusEvent.RemoveListener(OnMovementStatusEvent);
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
                CustomEvents.CustomEvents.EndTurnEvent.Invoke(gameObject.GetInstanceID());
                return;
            }

            Vector3 targetTile;
            if(shortestPath.Count < _combatUnitController.UnitInfo.Stats.MovementPoint)
            {
                targetTile = shortestPath[shortestPath.Count - 1];
            } else
            {
                targetTile = shortestPath[_combatUnitController.UnitInfo.Stats.MovementPoint - 1];
            }

            CustomEvents.CustomEvents.TileClickEvent.Invoke(targetTile);
        }

        void OnMovementStatusEvent(int instanceId, bool isMoving)
        {
            if (gameObject.GetInstanceID() != instanceId) return;

            if(!isMoving)
            {
                CustomEvents.CustomEvents.EndTurnEvent.Invoke(gameObject.GetInstanceID());
            }
        }
    }
}