using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat
{
    //MonoBehaviour that disable itself once EndCombat event is trigger
    public class CombatMonoBehavior : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            CustomEvents.EndCombat.AddListener(OnEndCombat);
        }

        protected virtual void OnDisable()
        {
            CustomEvents.EndCombat.RemoveListener(OnEndCombat);
        }

        void OnEndCombat()
        {
            Destroy(this);
        }
    }
}