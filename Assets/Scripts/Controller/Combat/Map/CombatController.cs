using Assets.Scripts.Controller;
using Assets.Scripts.Controller.Combat;
using Assets.Scripts.Controller.Types;
using Assets.Scripts.Events;
using Assets.Scripts.ScriptableObjets.Abilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatController : CombatMonoBehavior
{
    public static CombatMap Map;

    List<GameObject> _units;

    int _currUnitIndex = 0; 
    CombatStatistics _currUnitCombatStats;
    int _currUnitUsedMovementPoint = 0;

    bool _isUnitMoving = false;
    Ability _selectedAbility;
    GameObject _selectedAbilityUnit;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("CombatTile");
        _units = GameObject.FindGameObjectsWithTag("Unit").ToList();
        var _obstacles = new GameObject[] { };

        Map = new CombatMap(tiles, _obstacles, _units.ToArray());

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
        CustomEvents.SelectAbilityEvent.AddListener(OnSelectAbility);
        CustomEvents.TileClickEvent.AddListener(OnTileClick);
        CustomEvents.TileHoverEvent.AddListener(OnTileHover);
        CustomEvents.TileExitEvent.AddListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.AddListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.AddListener(OnEndTurn);
        CustomEvents.UnitClickEvent.AddListener(OnUnitClick);
        CustomEvents.UnselectAbilityEvent.AddListener(OnUnselectAbility);
        CustomEvents.UnitDeathEvent.AddListener(OnUnitDie);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CustomEvents.SelectAbilityEvent.RemoveListener(OnSelectAbility);
        CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
        CustomEvents.TileHoverEvent.RemoveListener(OnTileHover);
        CustomEvents.TileExitEvent.RemoveListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.RemoveListener(OnUnitMovementStatus);
        CustomEvents.EndTurnEvent.RemoveListener(OnEndTurn);
        CustomEvents.UnitClickEvent.RemoveListener(OnUnitClick);
        CustomEvents.UnselectAbilityEvent.RemoveListener(OnUnselectAbility);
        CustomEvents.UnitDeathEvent.RemoveListener(OnUnitDie);
    }

    void OnTileClick(Vector3 pos)
    {
        if (_selectedAbility)
        {
            if(!CanSelectedAbilityReach(pos))
            {
                Debug.Log("Can't reach clicked tile");
                CustomEvents.UnselectAbilityEvent.Invoke();
                return;
            }

            GameObject target = _units.ToList().Find(unit => unit.transform.position.x == pos.x && unit.transform.position.z == pos.z);
            HandleAbility(target);
            return;
        }

        HandleMovement(pos);
    }

    void OnUnitClick(int unitInstanceId)
    {
        if (!_selectedAbility) return;

        GameObject target = _units.ToList().Find(unit => unit.gameObject.GetInstanceID() == unitInstanceId);

        if (!CanSelectedAbilityReach(target.transform.position))
        {
            Debug.Log("Can't reach clicked unit");
            CustomEvents.UnselectAbilityEvent.Invoke();
            return;
        }

        HandleAbility(target);
    }

    void HandleMovement(Vector3 destination)
    {
        if (_isUnitMoving) return;

        Map.UpdateObstacles(_units.ToArray());
        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(), destination);


        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;

        _currUnitUsedMovementPoint += shortestPathAsVector.Count;

        CustomEvents.UnitMovementEvent.Invoke(_units[_currUnitIndex].gameObject.GetInstanceID(), shortestPathAsVector);
        Map.ClearPreviewPath();
    }

    void HandleAbility(GameObject target)
    {
        if (target)
        {
            Debug.Log($"Hit target {target.gameObject.name} for {_selectedAbility.Damage} with {_selectedAbility.Name}");
            CustomEvents.DamageUnitEvent.Invoke(target.gameObject.GetInstanceID(), _selectedAbility.Damage);
        }
        else
        {
            Debug.Log($"{_selectedAbility.Name} hit no one");
        }

        CustomEvents.UnselectAbilityEvent.Invoke();

    }

    public void OnTileHover(Vector3 target)
    {
        if (_isUnitMoving || _selectedAbility) return;

        Map.UpdateObstacles(_units.ToArray());
        List<Vector3> shortestPathAsVector = Map.GetShortestPathTo(GetCurrentUnitPosition(), target);

        if (shortestPathAsVector == null || shortestPathAsVector.Count > GetCurrentUnitMovementPointLeft()) return;

        Map.PreviewPath(shortestPathAsVector);
    }

    void OnSelectAbility(Ability ability, GameObject unit)
    {
        if (_selectedAbility) Map.ClearPreviewRange();

        Map.UpdateObstacles(_units.ToArray());

        var inRangePosition = Map.GetInRangePosition(unit.transform.position, ability.MinRange, ability.MaxRange);
        Map.PreviewRange(inRangePosition);

        _selectedAbility = ability;
        _selectedAbilityUnit = unit;
    }

    void OnUnselectAbility()
    {
        _selectedAbility = null;
        _selectedAbilityUnit = null;
        Map.ClearPreviewRange();
    }


    void OnUnitDie(int instanceId)
    {
        _units.Remove(_units.Single((unit) => unit.GetInstanceID() == instanceId));

        if (isCombatOver()) EndCombat();
    }

    public void OnTileExit()
    {
        if (_isUnitMoving) return;

        Map.ClearPreviewPath();
    }

    public void OnUnitMovementStatus(int instanceId, bool isMoving)
    {
        _isUnitMoving = isMoving;
    }

    public void OnEndTurn(int instanceId)
    {
        _currUnitIndex = _currUnitIndex == _units.Count - 1 ? 0 : _currUnitIndex + 1;

        if(_selectedAbility && instanceId == _selectedAbilityUnit.GetInstanceID())
        {
            CustomEvents.UnselectAbilityEvent.Invoke();
        }
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

    #region Utility


    bool CanSelectedAbilityReach(Vector3 position)
    {
        var inRangePosition = Map.GetInRangePosition(_selectedAbilityUnit.transform.position, _selectedAbility.MinRange, _selectedAbility.MaxRange);

        return inRangePosition.Contains(new Vector3(position.x, 0, position.z));
    }

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