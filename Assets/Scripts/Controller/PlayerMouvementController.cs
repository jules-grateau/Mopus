using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMouvementController : MonoBehaviour
{
    NavMeshAgent _navAgent;
    [SerializeField]
    LayerMask whatCanBeClickedOn;
    // Start is called before the first frame update
    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
    }

    public void OnMouseClick(Component component, object data)
    {

        RaycastHit hitInfo = (RaycastHit) data;
        
        _navAgent.SetDestination(hitInfo.point);
    }
}
