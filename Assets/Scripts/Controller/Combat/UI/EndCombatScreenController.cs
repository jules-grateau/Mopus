using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class EndCombatScreenController : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
            CustomEvents.EndCombat.AddListener(OnEndCombat);
        }

        private void OnDestroy()
        {
            CustomEvents.EndCombat.RemoveListener(OnEndCombat);
        }

        void OnEndCombat()
        {
            gameObject.SetActive(true);
        }
    }
}