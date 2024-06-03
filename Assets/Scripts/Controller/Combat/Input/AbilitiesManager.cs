using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat
{
    //TODO : Move to SpellBar controller.
    public class AbilitiesManager : CombatMonoBehavior
    {

        Guid _guid;
        Dictionary<KeyCode, Ability> _abilitiesMap;

        public List<KeyCode> KeyBinds => _keybinds;

        [SerializeField]
        List<Ability> _abilities;
        [SerializeField]
        List<KeyCode> _keybinds;

        bool _isPlayingTurn = false;
        Ability _selectedAbility;

        bool _isLockingUnitAction = false;

        #region Event Subcription
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomEvents.StartTurnEvent.AddListener(OnTurnStart);
            CustomEvents.UnitClickEvent.AddListener(OnUnitClick);
            CustomEvents.TileClickEvent.AddListener(OnTileClick);
            CustomEvents.SelectAbilityEvent.AddListener(OnSelectAbility);
            CustomEvents.UnselectAbilityEvent.AddListener(OnUnselectAbility);
            CustomEvents.EndTurnEvent.AddListener(OnEndTurn);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CustomEvents.StartTurnEvent.RemoveListener(OnTurnStart);
            CustomEvents.UnitClickEvent.RemoveListener(OnUnitClick);
            CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
            CustomEvents.SelectAbilityEvent.RemoveListener(OnSelectAbility);
            CustomEvents.UnselectAbilityEvent.RemoveListener(OnUnselectAbility);
            CustomEvents.EndTurnEvent.RemoveListener(OnEndTurn);
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

        private void Start()
        {
            _guid = Guid.NewGuid();
        }

        void Update()
        {
            if (!_isPlayingTurn) return;

            _keybinds.ForEach((key) =>
            {
                if (Input.GetKeyDown(key))
                {
                    SelectAbility(GetAbilityAt(key));

                }
            });

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                UnselectAbility();
            }
        }

        void OnTurnStart(int instanceId, bool isPlayerControlled)
        {
            if (!isPlayerControlled) return;

            if(gameObject.GetInstanceID() != instanceId)
            {
                UnselectAbility();
                _isPlayingTurn = false;
                return;
            }

            _isPlayingTurn = true;
        }

        void OnEndTurn(int instanceId)
        {
            if(instanceId == gameObject.GetInstanceID())
            {
                UnselectAbility();
            }
        }

        void OnSelectAbility(Ability ability) => SelectAbility(ability, true);

        void SelectAbility(Ability ability, bool fromEvent = false)
        {
            if (_selectedAbility) UnselectAbility();
            CustomEvents.LockUnitAction.Invoke(_guid);
            _isLockingUnitAction = true;

            var inRangePosition = CombatController.Instance.Map.GetInRangePosition(gameObject.transform.position, ability.MinRange, ability.MaxRange);
            CombatController.Instance.Map.PreviewRange(inRangePosition);

            _selectedAbility = ability;

            if (!fromEvent) CustomEvents.SelectAbilityEvent.Invoke(ability);
        }

        void OnUnselectAbility() => UnselectAbility(true);

        void UnselectAbility(bool fromEvent = false)
        {
            _selectedAbility = null;
            if (_isLockingUnitAction)
            {
                CustomEvents.UnlockUnitAction.Invoke();
                _isLockingUnitAction = false;
            }
            CombatController.Instance.Map.ClearPreviewRange();

            if (!fromEvent) CustomEvents.UnselectAbilityEvent.Invoke();
        }

        void HandleAbility(GameObject target)
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

        }

        void OnTileClick(Vector3 pos)
        {
            if (!_selectedAbility) return;

            
            if (!CanSelectedAbilityReach(pos))
            {
                Debug.Log("Can't reach clicked tile");
                UnselectAbility();
                return;
            }

            GameObject target = CombatController.Instance.Map.GetUnitAtPos(pos.x,pos.z);
            HandleAbility(target);
            return;
        }

        void OnUnitClick(int unitInstanceId)
        {
            if (!_selectedAbility) return;

            GameObject target = CombatController.Instance.Map.GetUnitByInstanceId(unitInstanceId);

            if (!CanSelectedAbilityReach(target.transform.position))
            {
                Debug.Log("Can't reach clicked unit");
                CustomEvents.UnselectAbilityEvent.Invoke();
                return;
            }

            HandleAbility(target);
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

        bool CanSelectedAbilityReach(Vector3 position)
        {
            var inRangePosition = CombatController.Instance.Map.GetInRangePosition(transform.position, _selectedAbility.MinRange, _selectedAbility.MaxRange);

            return inRangePosition.Contains(new Vector3(position.x, 0, position.z));
        }
    }
}