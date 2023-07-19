using Assets.Scripts.Controller;
using Assets.Scripts.Controller.Types;
using Assets.Scripts.CustomEvents;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatMap Map;

    GameObject[] _units;

    int _currUnitIndex = 0;
    CombatStatistics _currUnitCombatStats;
    int _currUnitUsedMovementPoint = 0;

    bool _isUnitMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("CombatTile");
        _units = GameObject.FindGameObjectsWithTag("Unit");

        Map = new CombatMap(tiles, _units);

        InitGameTurn();
    }

    void InitGameTurn()
    {
        GameObject currGameObjet = _units[_currUnitIndex];
        Debug.Log("New turn for unit "+currGameObjet.name);

        CombatUnitController currCombatUnitController = currGameObjet.GetComponent<CombatUnitController>();
        _currUnitCombatStats = currCombatUnitController.UnitInfo.Stats;
        _currUnitUsedMovementPoint = 0;

        CustomEvents.StartTurnEvent.Invoke(currGameObjet.GetInstanceID(), currCombatUnitController.UnitInfo.IsPlayerControlled);
    }



    #region Event Listeners
    private void OnEnable()
    {
        CustomEvents.TileClickEvent.AddListener(OnTileClick);
        CustomEvents.TileHoverEvent.AddListener(OnTileHover);
        CustomEvents.TileExitEvent.AddListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.AddListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.AddListener(OnEndTurn);
    }

    private void OnDisable()
    {
        CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
        CustomEvents.TileHoverEvent.RemoveListener(OnTileHover);
        CustomEvents.TileExitEvent.RemoveListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.RemoveListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.RemoveListener(OnEndTurn);
    }

    public void OnTileClick(Vector3 destination)
    {
        if (_isUnitMoving) return;

        Map.UpdateObstacles(_units);
        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(), destination);


        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;

        _currUnitUsedMovementPoint += shortestPathAsVector.Count;

        CustomEvents.UnitMovementEvent.Invoke(_units[_currUnitIndex].gameObject.GetInstanceID(), shortestPathAsVector);
    }

    public void OnTileHover(Vector3 target)
    {
        if (_isUnitMoving) return;

        Map.UpdateObstacles(_units);
        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(),target);

        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;    

        CustomEvents.UnitMovementPreviewEvent.Invoke(shortestPathAsVector);
    }

    public void OnTileExit()
    {
        if (_isUnitMoving) return;

        CustomEvents.UnitMovementPreviewClearEvent.Invoke();
    }

    public void OnUnitMovementStatus(int instanceId,bool isMoving)
    {
        _isUnitMoving = isMoving;
    }

    public void OnEndTurn(int instanceId)
    {
        Debug.Log("End of turn");

        _currUnitIndex = _currUnitIndex == _units.Length-1 ? 0 : _currUnitIndex+1;
        InitGameTurn();
    }

    #endregion

    #region Current Unit Functions
    int GetCurrentUnitMovementPointLeft()
    {
        return _currUnitCombatStats.MovementPoint - _currUnitUsedMovementPoint;
    }

    Vector3 GetCurrentUnitPosition()
    {
        return _units[_currUnitIndex].transform.position;
    }
    #endregion
}