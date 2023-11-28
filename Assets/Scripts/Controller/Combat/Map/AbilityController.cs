using Assets.Scripts.ScriptableObjets.Abilities;
using Assets.Scripts.Events;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.Map
{
    /*public class AbilityController : MonoBehaviour
    {
        Ability _selectedAbility;
        private void OnEnable()
        {
            CustomEvents.SelectAbilityEvent.AddListener(OnSelectAbility);
            CustomEvents.TileClickEvent.AddListener(OnTileClick);
            CustomEvents.UnitClickEvent.AddListener(OnUnitClick);
        }

        private void OnDisable()
        {
            CustomEvents.SelectAbilityEvent.RemoveListener(OnSelectAbility);
            CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
            CustomEvents.UnitClickEvent.RemoveListener(OnUnitClick);
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
                CustomEvents.DamageUnitEvent.Invoke(target.gameObject.GetInstanceID(), _selectedAbility.Damage);
            }
            else
            {
                Debug.Log($"{_selectedAbility.Name} hit no one");
            }

            CustomEvents.UnselectAbilityEvent.Invoke();
            _selectedAbility = null;

        }

        void OnSelectAbility(Ability ability, GameObject gameObject)
        {
            Debug.Log($"Ability {ability.name} selected");
            _selectedAbility = ability;
        }


    }*/
}