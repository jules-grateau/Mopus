using Assets.Scripts.Controller;
using Assets.Scripts.Controller.Combat;
using Assets.Scripts.Controller.Types;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatController : CombatMonoBehavior
{
    public static CombatController Instance { get => _instance; }

    static CombatController _instance;
    public CombatMap Map { get => _map; }
    public List<GameObject> Units { get => _units; }

    Guid _guid;

    CombatMap _map;
    List<GameObject> _units;

    int _currUnitIndex = 0; 
    CombatStatistics _currUnitCombatStats;
    int _currUnitUsedMovementPoint = 0;

    bool _isUnitMoving = false;
    bool _isUnitActionLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        _units = GameObject.FindGameObjectsWithTag("Unit").ToList();
        _instance = this;
        _guid = Guid.NewGuid();

        _map = new CombatMap();

        InitGameTurn();
    }

    void InitGameTurn()
    {
        GameObject currGameObjet = _units[_currUnitIndex];

        CombatUnitController currCombatUnitController = currGameObjet.GetComponent<CombatUnitController>();
        _currUnitCombatStats = currCombatUnitController.UnitInfo.Stats;
        _currUnitUsedMovementPoint = 0;

        CustomEvents.StartTurnEvent.Invoke(currGameObjet.GetInstanceID(), currCombatUnitController.UnitInfo.IsPlayerControlled);
    }



    #region Event Listeners
    protected override void OnEnable()
    {
        base.OnEnable();
        CustomEvents.TileClickEvent.AddListener(OnTileClick);
        CustomEvents.TileHoverEvent.AddListener(OnTileHover);
        CustomEvents.TileExitEvent.AddListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.AddListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.AddListener(OnEndTurn);
        CustomEvents.UnitDeathEvent.AddListener(OnUnitDie);
        CustomEvents.LockUnitAction.AddListener(OnLockUnitAction);
        CustomEvents.UnlockUnitAction.AddListener(OnUnlockUnitAction);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
        CustomEvents.TileHoverEvent.RemoveListener(OnTileHover);
        CustomEvents.TileExitEvent.RemoveListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.RemoveListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.RemoveListener(OnEndTurn);
        CustomEvents.UnitDeathEvent.RemoveListener(OnUnitDie);
        CustomEvents.LockUnitAction.RemoveListener(OnLockUnitAction);
        CustomEvents.UnlockUnitAction.RemoveListener(OnUnlockUnitAction);
    }

    void OnLockUnitAction(Guid guid)
    {
        //If it's our own lock, we don't consider the action to be locked
        if (guid == _guid) return;

        _isUnitActionLocked = true;
    }

    void OnUnlockUnitAction()
    {
        _isUnitActionLocked = false;
    }

    void OnTileClick(Vector3 pos)
    {
        HandleMovement(pos);
    }

    void HandleMovement(Vector3 destination)
    {
        if (_isUnitMoving || _isUnitActionLocked || !IsCurrentUnitControlledByPlayer()) return;

        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(), destination);


        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;

        _currUnitUsedMovementPoint += shortestPathAsVector.Count;

        CustomEvents.UnitMovementEvent.Invoke(_units[_currUnitIndex].gameObject.GetInstanceID(), shortestPathAsVector);
        Map.ClearPreviewPath();
    }


    public void OnTileHover(Vector3 target)
    {
        if (_isUnitMoving || _isUnitActionLocked || !IsCurrentUnitControlledByPlayer()) return;

        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(), target);

        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;

        Map.PreviewPath(shortestPathAsVector);
    }

    void OnUnitDie(int instanceId)
    {
        _units.Remove(_units.Single((unit) => unit.GetInstanceID() == instanceId));

        if (isCombatOver()) EndCombat();
    }

    public void OnTileExit()
    {
        if (_isUnitMoving || _isUnitActionLocked) return;

        Map.ClearPreviewPath();
    }

    public void OnUnitMovementStatus(int instanceId, bool isMoving)
    {
        _isUnitMoving = isMoving;
    }

    public void OnEndTurn(int instanceId)
    {
        Debug.Log("End turn of " + instanceId);
        _currUnitIndex = _currUnitIndex == _units.Count - 1 ? 0 : _currUnitIndex + 1;

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

    bool IsCurrentUnitControlledByPlayer()
    {
        return _units[_currUnitIndex].GetComponent<CombatUnitController>().UnitInfo.IsPlayerControlled;
    }
    #endregion

    #region Utility
    bool isCombatOver()
    {
        if(_units.Count <= 0) return true;
        var firstUnitTeam = _units[0].GetComponent<CombatUnitController>().UnitInfo.TeamId;
        
        //If all units present in the combat are in the same team, then the combat is over
        return _units.All((unit) => unit.GetComponent<CombatUnitController>().UnitInfo.TeamId == firstUnitTeam);
    }

    void EndCombat()
    {
        CustomEvents.UnselectAbilityEvent.Invoke();
        CustomEvents.EndCombat.Invoke();
    }
    #endregion
}