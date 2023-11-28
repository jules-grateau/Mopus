using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.Enemies
{
    public class ClickableUnit : MonoBehaviour
    {

        private void OnMouseDown()
        {
            CustomEvents.UnitClickEvent.Invoke(gameObject.GetInstanceID());
        }
    }
}