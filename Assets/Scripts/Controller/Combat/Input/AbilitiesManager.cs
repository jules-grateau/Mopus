using Assets.Scripts.ScriptableObjets.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat
{
    //TODO : Move to SpellBar controller.
    public class AbilitiesManager : MonoBehaviour
    {

        Dictionary<KeyCode, Ability> _abilitiesMap;

        public List<KeyCode> KeyBinds => _keybinds;

        [SerializeField]
        List<Ability> _abilities;
        [SerializeField]
        List<KeyCode> _keybinds;

        bool _isPlayingTurn = false;

        #region Event Subcription
        void OnEnable()
        {
            CustomEvents.CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
        }

        void OnDisable()
        {
            CustomEvents.CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
        }
        #endregion
        void Awake()
        {
            _abilitiesMap = new Dictionary<KeyCode, Ability>();

            foreach (var item in _abilities.Select((ability, index) => new { index, ability }))
            {
                _abilitiesMap.Add(_keybinds[item.index], item.ability);
            }
        }

        void Update()
        {
            if (!_isPlayingTurn) return;

            _keybinds.ForEach((key) =>
            {
                if (Input.GetKeyDown(key))
                {
                    CustomEvents.CustomEvents.SelectAbilityEvent.Invoke(GetAbilityAt(key));
                }
            });

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CustomEvents.CustomEvents.UnselectAbilityEvent.Invoke();
            }
        }

        void OnTurnStart(int instanceId, bool isPlayerControlled)
        {
            if (!isPlayerControlled) return;

            if(gameObject.GetInstanceID() != instanceId)
            {
                _isPlayingTurn = false;
                return;
            }

            _isPlayingTurn = true;
        }

        Ability GetAbilityAt(KeyCode keybind)
        {
            return _abilitiesMap.GetValueOrDefault(keybind);
        }

        public List<Tuple<KeyCode,Ability>> GetAbilityList()
        {
            List<Tuple<KeyCode, Ability>> abilities = new List<Tuple<KeyCode, Ability>>();

            foreach(KeyValuePair<KeyCode,Ability> ability in _abilitiesMap)
            {
                abilities.Add(new Tuple<KeyCode, Ability>(ability.Key, ability.Value));
            }

            return abilities;
        }
    }
}