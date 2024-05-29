using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat
{
    //TODO : Move to SpellBar controller.
    public class AbilitiesManager : CombatMonoBehavior
    {

        Dictionary<KeyCode, Ability> _abilitiesMap;

        public List<KeyCode> KeyBinds => _keybinds;

        [SerializeField]
        List<Ability> _abilities;
        [SerializeField]
        List<KeyCode> _keybinds;

        bool _isPlayingTurn = false;

        #region Event Subcription
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
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
                    CustomEvents.SelectAbilityEvent.Invoke(GetAbilityAt(key), gameObject);
                }
            });

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                CustomEvents.UnselectAbilityEvent.Invoke();
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