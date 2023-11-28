using Assets.Scripts.Events;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class CurrentUnitTextController : MonoBehaviour
    {
        GameObject[] _unit;
        TextMeshProUGUI _tmp;

        private void Awake()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            CustomEvents.StartTurnEvent.AddListener(OnStartTurn);
        }

        private void OnDisable()
        {
            CustomEvents.StartTurnEvent.RemoveListener(OnStartTurn);
        }

        void OnStartTurn(int instanceId, bool isPlayerControlled)
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            string name = units.ToList().Find((go) => go.GetInstanceID() == instanceId).name;

            _tmp.text = name;
        }
    }
}