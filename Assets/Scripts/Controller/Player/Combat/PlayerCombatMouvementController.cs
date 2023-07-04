using Assets.Scripts.CustomEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatMouvementController : MonoBehaviour
{
    [SerializeField]
    bool isMoving = false;

    private void OnEnable()
    {
        CustomEvents.UnitMovementEvent.AddListener(OnPlayerMovement);
    }

    private void OnDisable()
    {
        CustomEvents.UnitMovementEvent.RemoveListener(OnPlayerMovement);
    }

    public void OnPlayerMovement(List<Vector3> path)
    {
        if (isMoving) return;
        StartCoroutine(MoveThrought(path));
    }

    IEnumerator MoveThrought(List<Vector3> path)
    {
        isMoving = true;
        CustomEvents.UnitMovementStatusEvent.Invoke(true);
        while (path.Count > 0)
        {
            Vector3 tile = path[0];
            transform.position = new Vector3(tile.x, transform.position.y, tile.z);
            path.Remove(tile);
            yield return new WaitForSeconds(0.1f);
            CustomEvents.UnitMovementPreviewEvent.Invoke(path);
        }
        isMoving = false;
        CustomEvents.UnitMovementStatusEvent.Invoke(false);
    }

}
