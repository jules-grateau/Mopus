using Assets.Scripts.Controller.Combat;
using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Parameters;
using UnityEngine;

public class CombatTileController : CombatMonoBehavior
{
    public string Name;
    public bool IsWalkable;

    [SerializeField]
    GameEvent combatTileClickEvent;
    [SerializeField]
    GameEvent combatTileHoverEvent;

    ColorsParams colorsParams;
    HighlightController highlightController;

    private void Start()
    {
        highlightController = GetComponent<HighlightController>();

        colorsParams = Resources.Load<ColorsParams>("ScriptableObjects/Parameters/ColorsParams");
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

    public void HightLightMovement(bool on)
    {
        if (on)
        {
            highlightController.Highlight(true, colorsParams.MovementPreviewColor);
        }
        else
        {
            highlightController.Highlight(false, colorsParams.MovementPreviewColor);
        }
    }

    public void HightLightRange(bool on)
    {
        if (on)
        {
            highlightController.Highlight(true, colorsParams.RangePreviewColor);
        }
        else
        {
            highlightController.Highlight(false, colorsParams.RangePreviewColor);
        }
    }
}
