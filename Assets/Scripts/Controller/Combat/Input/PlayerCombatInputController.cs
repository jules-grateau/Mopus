using Assets.Scripts.Controller.Combat;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class PlayerCombatInputController : CombatMonoBehavior
    {
        private bool _isCurrentUnitPlayerControlled = false;
        private int _currUnitInstanceId = 0;

        #region Event Subscription

        // Use this for initialization
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomEvents.StartTurnEvent.AddListener(OnStartTurn);
            CustomEvents.EndCombat.AddListener(OnCombatEnd);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CustomEvents.StartTurnEvent.RemoveListener(OnStartTurn);
            CustomEvents.EndCombat.RemoveListener(OnCombatEnd);
        }

        #endregion


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown("space") && _isCurrentUnitPlayerControlled)
            {
                CustomEvents.EndTurnEvent.Invoke(_currUnitInstanceId);
            }

        }

        #region Event Listeners
        void OnStartTurn(int instanceId, bool isPlayerControlled)
        {
            _isCurrentUnitPlayerControlled = isPlayerControlled;
            _currUnitInstanceId = instanceId;
        }

        void OnCombatEnd()
        {
            enabled = false;
        }
        #endregion
    }
}