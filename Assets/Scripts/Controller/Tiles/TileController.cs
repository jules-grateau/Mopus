using Assets.Scripts.CustomEvents;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Controller.Tiles
{
    public class TileController : MonoBehaviour
    {
        [SerializeField]
        GameEvent mouseClickEvent;

        private void OnMouseDown()
        {
            CustomEvents.CustomEvents.TileClickEvent.Invoke(transform.position);
        }
    }
}