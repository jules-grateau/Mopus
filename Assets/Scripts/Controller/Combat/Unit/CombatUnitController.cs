using Assets.Scripts.Controller.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    using CustomEvents;

    public class CombatUnitController : MonoBehaviour
    {

        public UnitInfo UnitInfo { get { return _unitInfo; } }
        [SerializeField]
        private UnitInfo _unitInfo;

        bool isMoving = false;

        private void OnEnable()
        {
            CustomEvents.UnitMovementEvent.AddListener(MoveTo);
        }

        private void OnDisable()
        {
            CustomEvents.UnitMovementEvent.RemoveListener(MoveTo);
        }

        private void MoveTo(int instanceId, List<Vector3> path)
        {
            if (gameObject.GetInstanceID() != instanceId) return;
            if (isMoving) return;
            StartCoroutine(MoveThrought(path));
        }

        IEnumerator MoveThrought(List<Vector3> path)
        {
            isMoving = true;
            CustomEvents.UnitMovementStatusEvent.Invoke(gameObject.GetInstanceID(),true);
            while (path.Count > 0)
            {
                Vector3 tile = path[0];
                transform.position = new Vector3(tile.x, transform.position.y, tile.z);
                path.Remove(tile);
                yield return new WaitForSeconds(0.1f);
                CustomEvents.UnitMovementPreviewEvent.Invoke(path);
            }
            isMoving = false;
            CustomEvents.UnitMovementStatusEvent.Invoke(gameObject.GetInstanceID(),false);
        }
    }
}