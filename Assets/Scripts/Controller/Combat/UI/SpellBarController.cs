using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Abilities;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class SpellBarController : MonoBehaviour
    {
        GameObject _abilitySlotPrefab;

        // Use this for initialization
        void Start()
        {
            _abilitySlotPrefab = (GameObject) Resources.Load("Prefabs/UI/AbilitySlot");
        }

        private void OnEnable()
        {
            CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
        }

        private void OnDisable()
        {
            CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
        }

        void OnTurnStart(int instanceId, bool isPlayerControlled)
        {
            if (!isPlayerControlled) return;

            CleanAbilitySlots();

            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            GameObject currentTurnUnit = units.ToList().Find((unit) => unit.gameObject.GetInstanceID() == instanceId);

            if (!currentTurnUnit) return;
            AbilitiesManager abilitiesManager = currentTurnUnit.GetComponent<AbilitiesManager>();

            if (!abilitiesManager) return;
            
            foreach(Tuple<KeyCode,Ability> abilitySlot in abilitiesManager.GetAbilityList())
            {
                AddAbilitySlot(abilitySlot.Item1, abilitySlot.Item2, currentTurnUnit);
            }


        }

        void AddAbilitySlot(KeyCode keyCode, Ability ability, GameObject currentTurnUnit)
        {
            GameObject abilitySlot = Instantiate(_abilitySlotPrefab,transform);

            AbilitySlotController abilitySlotController = abilitySlot.GetComponent<AbilitySlotController>();
            abilitySlotController.Init(ability, keyCode.ToString(), currentTurnUnit);

        }

        void CleanAbilitySlots()
        {
            foreach(Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

    }
}