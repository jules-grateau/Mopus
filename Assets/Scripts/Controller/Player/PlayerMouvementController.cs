using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMouvementController : MonoBehaviour
{
    NavMeshAgent _navAgent;

    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        CustomEvents.TileClickEvent.AddListener(OnTileClick);
    }

    private void OnDisable()
    {
        CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
    }

    public void OnTileClick(Vector3 destination)
    {        
        _navAgent.SetDestination(destination);
    }
}
