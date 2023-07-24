using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.Enemies
{
    public class ClickableUnit : MonoBehaviour
    {

        private void OnMouseDown()
        {
            CustomEvents.CustomEvents.UnitClickEvent.Invoke(gameObject.GetInstanceID());
        }
    }
}