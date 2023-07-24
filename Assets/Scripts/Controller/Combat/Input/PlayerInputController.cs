using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class PlayerInputController : MonoBehaviour
    {
        private bool _isCurrentUnitPlayerControlled = false;
        private int _currUnitInstanceId = 0;

        #region Event Subscription

        // Use this for initialization
        void OnEnable()
        {
            CustomEvents.CustomEvents.StartTurnEvent.AddListener(OnStartTurn);
        }

        private void OnDisable()
        {
            CustomEvents.CustomEvents.StartTurnEvent.RemoveListener(OnStartTurn);
        }

        #endregion


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown("space") && _isCurrentUnitPlayerControlled)
            {
                CustomEvents.CustomEvents.EndTurnEvent.Invoke(_currUnitInstanceId);
            }

        }

        #region Event Listeners
        void OnStartTurn(int instanceId, bool isPlayerControlled)
        {
            _isCurrentUnitPlayerControlled = isPlayerControlled;
            _currUnitInstanceId = instanceId;
        }
        #endregion
    }
}