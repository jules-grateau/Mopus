using Assets.Scripts.CustomEvents;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour
{
    Dictionary<(float,float),TileType> _mapTiles;
    Dictionary<(float, float), Node> _graph;

    [SerializeField]
    GameObject _playableUnit;

    bool _isUnitMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMapTiles();
        InitializeGraph();
    }

    private void OnEnable()
    {
        CustomEvents.TileClickEvent.AddListener(OnTileClick);
        CustomEvents.TileHoverEvent.AddListener(OnTileHover);
        CustomEvents.TileExitEvent.AddListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.AddListener(OnUnitMovementStatus);
    }

    private void OnDisable()
    {
        CustomEvents.TileClickEvent.RemoveListener(OnTileClick);
        CustomEvents.TileHoverEvent.RemoveListener(OnTileHover);
        CustomEvents.TileExitEvent.RemoveListener(OnTileExit);
        CustomEvents.UnitMovementStatusEvent.RemoveListener(OnUnitMovementStatus);
    }

    void InitializeMapTiles()
    {
        _mapTiles = new Dictionary<(float, float), TileType>();
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("CombatTile");
        foreach (GameObject go in tiles)
        {
            _mapTiles.Add((go.transform.position.x, go.transform.position.z), go.GetComponent<TileType>());
        }
    }

    void InitializeGraph()
    {
        _graph = new Dictionary<(float, float), Node>();

        foreach(var tile in _mapTiles)
        {
            _graph.Add(tile.Key, new Node(tile.Key.Item1, tile.Key.Item2, tile.Value.IsWalkable)) ;
        }

        foreach(Node node in _graph.Values)
        {
            Node left;
            Node right;
            Node top;
            Node bottom;

            if(_graph.TryGetValue((node.x-1, node.z), out left))
            {
                node.AddNeighbor(left);
            };
            if(_graph.TryGetValue((node.x + 1, node.z), out right))
            {
                node.AddNeighbor(right);
            };
            if(_graph.TryGetValue((node.x, node.z+1), out top))
            {
                node.AddNeighbor(top);
            };
            if(_graph.TryGetValue((node.x, node.z - 1), out bottom))
            {
                node.AddNeighbor(bottom);
            };

        }
    }

    public void OnTileClick(Vector3 destination)
    {
        if (_isUnitMoving) return;
        List<Vector3> shortestPathAsVector = GetShortestPathTo(destination);

        CustomEvents.UnitMovementEvent.Invoke(shortestPathAsVector);
    }

    public void OnTileHover(Vector3 target)
    {
        if (_isUnitMoving) return;
        List<Vector3> shortestPathAsVector = GetShortestPathTo(target);

        CustomEvents.UnitMovementPreviewEvent.Invoke(shortestPathAsVector);
    }

    public void OnTileExit()
    {
        if (_isUnitMoving) return;

        CustomEvents.UnitMovementPreviewClearEvent.Invoke();
    }

    public void OnUnitMovementStatus(bool isMoving)
    {
        _isUnitMoving = isMoving;
    }

    List<Vector3> GetShortestPathTo(Vector3 target) 
    {
        Node source = _graph[(_playableUnit.transform.position.x, _playableUnit.transform.position.z)];
        Node targetNode;
        if (!_graph.TryGetValue((target.x, target.z), out targetNode)) return null;
        if (source == targetNode) return null;

        (Dictionary<Node, float> distance, Dictionary<Node, Node> prev) = CalculateShortestPath(source, targetNode);


        if (distance[targetNode] == Mathf.Infinity)
        {
            print($"Could not find path to combatTile : {target.x}-{target.z}");
            return null;

        }

        LinkedList<Node> shortestPath = new LinkedList<Node>();
        shortestPath.AddFirst(targetNode);

        while (prev[shortestPath.First()] != source)
        {
            shortestPath.AddFirst(prev[shortestPath.First()]);
        }

        List<Vector3> shortestPathAsVector = new List<Vector3>();
        foreach (Node node in shortestPath.ToList())
        {
            shortestPathAsVector.Add(new Vector3(node.x, 0, node.z));
        }
        return shortestPathAsVector;


    }

    (Dictionary<Node,float>, Dictionary<Node,Node>) CalculateShortestPath(Node source, Node target)
    {


        Dictionary<Node,float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        //Calculation Setup
        distance.Add(source, 0f);

        foreach(Node node in _graph.Values)
        {
            if(node != source)
            {
                distance.Add(node, Mathf.Infinity);
                prev.Add(node, null);
            }
            unvisited.Add(node);
        }

        while(unvisited.Count > 0)
        {
            Node closest = unvisited.OrderBy(node => distance[node]).First();
            
            //If we reached the target, we can leave
            if (closest == target) break;
            
            unvisited.Remove(closest);

            foreach(Node node in closest.GetNeighbors())
            {
                float distanceTo = distance[closest] + closest.DistanceTo(node);
                if (distanceTo < distance[node])
                {
                    distance[node] = distanceTo;
                    prev[node] = closest;
                }
            }
        }

        return (distance, prev);
    }
}

class Node
{
    List<Node> Neighbors;
    public float x { get; }
    public float z { get; }

    bool isWalkable;
    public Node(float x, float z, bool isWalkable)
    {
        Neighbors = new List<Node>();
        this.x = x;
        this.z = z;
        this.isWalkable = isWalkable;
    }

    public void AddNeighbor(Node node)
    {
        Neighbors.Add(node);
    }

    public List<Node> GetNeighbors()
    {
        return Neighbors;
    }

    public float DistanceTo(Node node)
    {
        if (!isWalkable) return Mathf.Infinity;

        return Vector2.Distance(
            new Vector2(x, z),
            new Vector2(node.x, node.z));
    }
}