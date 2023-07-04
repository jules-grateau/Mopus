using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatMouvementController : MonoBehaviour
{
    [SerializeField]
    bool isMoving = false;

    public void OnPlayerCombatMouvement(Component component, object data)
    {
        if (data == null) return;
        if (isMoving) return;

        List<Vector3> path = (List<Vector3>) data;


        StartCoroutine(MoveThrought(path));
    }

    IEnumerator MoveThrought(List<Vector3> path)
    {
        isMoving = true;
        foreach (Vector3 tile in path)
        {
            transform.position = new Vector3(tile.x, transform.position.y, tile.z);
            yield return new WaitForSeconds(0.1f);
        }
        isMoving = false;
    }

}
