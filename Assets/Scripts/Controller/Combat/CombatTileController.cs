using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatTileController : MonoBehaviour
{
    [SerializeField]
    GameEvent combatTileClickEvent;
    [SerializeField]
    GameEvent combatTileHoverEvent;

    MeshRenderer meshRenderer;
    bool isShowingPreview = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();    
    }

    public void OnMouseClick(Component component, object data)
    {
        if (data == null) return;
        RaycastHit hitInfo = (RaycastHit) data;

        if (hitInfo.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()) return;

        combatTileClickEvent.Raise(this, transform.position);
    }

    public void OnMouseHover(Component component, object data)
    {
        if (data == null)
        {
            //No data means Hovering nothing, so we clear preview
            if(isShowingPreview) showPreview(false);
            return;
        }

            RaycastHit hitInfo = (RaycastHit)data;

        if (hitInfo.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID()) return;

        combatTileHoverEvent.Raise(this, transform.position);
    }

    public void OnPlayerCombatMouvementPreview(Component component, object data)
    {
        if (data == null)
        {
            //No data we did not find path to the current hovered tile
            if (isShowingPreview) showPreview(false);
            return;
        }
        List<Vector3> path = (List<Vector3>) data;

        if(path.Exists(vec => vec.x == transform.position.x && vec.z == transform.position.z))
        {
            showPreview(true);
        } else if(isShowingPreview)
        {
            showPreview(false);
        }
    }

    void showPreview(bool on)
    {
        if(on)
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetFloat("_IsPreview", 1f);
                isShowingPreview = true;
            }
        } else
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetFloat("_IsPreview", 0f);
                isShowingPreview = false;
            }
        }
    }
}
