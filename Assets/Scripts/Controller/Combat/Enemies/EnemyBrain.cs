using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Abilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.Enemies
{
    public class EnemyBrain : CombatMonoBehavior
    {
        CombatUnitController _combatUnitController;
        [SerializeField]
        List<Ability> _abilities;

        private void Start()
        {
            _combatUnitController = GetComponent<CombatUnitController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
            CustomEvents.UnitMovementStatusEvent.AddListener(OnMovementStatusEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
            CustomEvents.UnitMovementStatusEvent.RemoveListener(OnMovementStatusEvent);
        }

        void OnTurnStart(int instanceId, bool isPlayerControlled) 
        {
            if (!_combatUnitController) return;
            if (instanceId != gameObject.GetInstanceID() || isPlayerControlled) return;

            HandleMovement();
        }
        
        void HandleMovement()
        {
            //Find a target
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            List<GameObject> targets = units.ToList().FindAll((unit) => unit.GetComponent<CombatUnitController>().UnitInfo.TeamId != _combatUnitController.UnitInfo.TeamId);

            List<Vector3> shortestPath = null;

            foreach (GameObject target in targets)
            {
                var targetPath = CombatController.Instance.Map.GetShortestPathToAdjacent(gameObject.transform.position, target.transform.position);
                if (shortestPath == null)
                {
                    shortestPath = targetPath;
                }
                else
                {
                    shortestPath = targetPath.Count < shortestPath.Count ? targetPath : shortestPath;
                }
            }

            if (shortestPath == null)
            {
                HandleAttack();
                return;
            }


            if (shortestPath.Count > _combatUnitController.UnitInfo.Stats.MovementPoint)
            {
                shortestPath = shortestPath.Take(_combatUnitController.UnitInfo.Stats.MovementPoint).ToList();
            }

            CustomEvents.UnitMovementEvent.Invoke(gameObject.GetInstanceID(), shortestPath);
        }

        void HandleAttack()
        {
            foreach(Ability ability in _abilities)
            {
                var inRangeUnits = CombatController.Instance.Map.GetUnitsInRange(gameObject.transform.position, ability.MinRange, ability.MaxRange).Where(unit => unit.GetComponent<CombatUnitController>().UnitInfo.TeamId != _combatUnitController.UnitInfo.TeamId).ToList();

                if(inRangeUnits.Count > 0)
                {
                    UseAbility(ability, inRangeUnits[0]);
                }
            }

            StartCoroutine(EndTurn());
        }

        //TemporaryFix to avoid multiple action being received at once 
        IEnumerator EndTurn()
        {
            yield return new WaitForSeconds(0.1f);
            CustomEvents.EndTurnEvent.Invoke(gameObject.GetInstanceID());

        }

        void UseAbility(Ability ability, GameObject target)
        {
            CustomEvents.DamageUnitEvent.Invoke(target.gameObject.GetInstanceID(), ability.Damage);
        }

        void OnMovementStatusEvent(int instanceId, bool isMoving)
        {
            if (gameObject.GetInstanceID() != instanceId) return;

            if(!isMoving)
            {
                HandleAttack();
            }
        }
    }
}