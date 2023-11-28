using Assets.Scripts.Events;
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
            CustomEvents.TileClickEvent.Invoke(transform.position);
        }
    }
}