using Assets.Scripts.CustomEvents;
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

    private void OnEnable()
    {
        CustomEvents.UnitMovementPreviewEvent.AddListener(OnUnitMovementPreview);
        CustomEvents.UnitMovementPreviewClearEvent.AddListener(OnClearPreview);
    }

    private void OnDisable()
    {
        CustomEvents.UnitMovementPreviewEvent.RemoveListener(OnUnitMovementPreview);
        CustomEvents.UnitMovementPreviewClearEvent.RemoveListener(OnClearPreview);
    }

    private void OnMouseDown()
    {
        CustomEvents.TileClickEvent.Invoke(transform.position);
    }

    private void OnMouseEnter()
    {
        CustomEvents.TileHoverEvent.Invoke(transform.position);
    }

    private void OnMouseExit()
    {
        CustomEvents.TileExitEvent.Invoke();
    }

    private void OnClearPreview()
    {
        if(isShowingPreview) showPreview(false);
    }

    public void OnUnitMovementPreview(List<Vector3> path)
    {
        if (path == null)
        {
            //No data we did not find path to the current hovered tile
            if (isShowingPreview) showPreview(false);
            return;
        }

        if (path.Exists(vec => vec.x == transform.position.x && vec.z == transform.position.z))
        {
            showPreview(true);
        }
        else if (isShowingPreview)
        {
            showPreview(false);
        }
    }

    void showPreview(bool on)
    {
        if (on)
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetFloat("_IsPreview", 1f);
                isShowingPreview = true;
            }
        }
        else
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.SetFloat("_IsPreview", 0f);
                isShowingPreview = false;
            }
        }
    }
}
