using Assets.Scripts.ScriptableObjets.Abilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.Map
{
    public class AbilityController : MonoBehaviour
    {
        Ability _selectedAbility;
        private void OnEnable()
        {
            CustomEvents.CustomEvents.SelectAbilityEvent.AddListener(OnSelectAbility);
            CustomEvents.CustomEvents.TileClickEvent.AddListener(OnTileClick);
            CustomEvents.CustomEvents.UnitClickEvent.AddListener(OnUnitClick);
        }

        private void OnDisable()
        {
            CustomEvents.CustomEvents.SelectAbilityEvent.RemoveListener(OnSelectAbility);
            CustomEvents.CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
            CustomEvents.CustomEvents.UnitClickEvent.RemoveListener(OnUnitClick);
        }

        void OnTileClick(Vector3 destination)
        {
            if (!_selectedAbility) return;

            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

            GameObject target = units.ToList().Find(unit => unit.transform.position.Equals(destination));

            FireAbility(target);

        }

        void OnUnitClick(int unitInstanceId)
        {
            if (!_selectedAbility) return;

            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

            GameObject target = units.ToList().Find(unit => unit.gameObject.GetInstanceID() == unitInstanceId);

            FireAbility(target);
        }

        void FireAbility(GameObject target)
        {
            if (target)
            {
                Debug.Log($"Hit target {target.gameObject.name} for {_selectedAbility.Damage} with {_selectedAbility.Name}");
                CustomEvents.CustomEvents.DamageUnitEvent.Invoke(target.gameObject.GetInstanceID(), _selectedAbility.Damage);
            }
            else
            {
                Debug.Log($"{_selectedAbility.Name} hit no one");
            }

            CustomEvents.CustomEvents.UnselectAbilityEvent.Invoke();
            _selectedAbility = null;

        }

        void OnSelectAbility(Ability ability)
        {
            Debug.Log($"Ability {ability.name} selected");
            _selectedAbility = ability;
        }


    }
}