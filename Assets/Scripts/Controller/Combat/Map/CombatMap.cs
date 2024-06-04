using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class CombatMap
    {
        Dictionary<(float, float), CombatTileController> _mapTiles;
        Dictionary<(float, float), GameObject> _units;
        Dictionary<(float, float), Node> _graph;

        List<Vector3> _currPreviewedPath = null;
        List<Vector3> _currPreviewRange = null;

        public CombatMap()
        {
            InitializeMapTiles();
            InitializeUnits();
            InitializeGraph();
        }

        public void ResyncMap()
        {
            InitializeUnits();
            InitializeGraph();
        }

        void InitializeMapTiles()
        {
            _mapTiles = new Dictionary<(float, float), CombatTileController>();
            var combatTiles = GameObject.FindGameObjectsWithTag("CombatTile");

            foreach (GameObject go in combatTiles)
            {
                _mapTiles.Add((go.transform.position.x, go.transform.position.z), go.GetComponent<CombatTileController>());
            }
        }

        void InitializeUnits()
        {
            _units = new Dictionary<(float, float), GameObject>();
            var inCombatUnits = GameObject.FindGameObjectsWithTag("Unit").ToList();

            foreach (GameObject go in inCombatUnits)
            {
                _units.Add((go.transform.position.x, go.transform.position.z), go);
            }
        }

        void InitializeGraph()
        {
            _graph = new Dictionary<(float, float), Node>();

            foreach (var tile in _mapTiles)
            {
                (float, float) tilePos = (tile.Key.Item1, tile.Key.Item2);
                
                bool isWalkable = tile.Value.IsWalkable && !_units.Keys.Contains(tilePos);

                _graph.Add(tile.Key, new Node(tile.Key.Item1, tile.Key.Item2, isWalkable));
                

            }

            foreach (Node node in _graph.Values)
            {
                Node left;
                Node right;
                Node top;
                Node bottom;

                if (_graph.TryGetValue((node.x - 1, node.z), out left))
                {
                    node.AddNeighbor(left);
                };
                if (_graph.TryGetValue((node.x + 1, node.z), out right))
                {
                    node.AddNeighbor(right);
                };
                if (_graph.TryGetValue((node.x, node.z + 1), out top))
                {
                    node.AddNeighbor(top);
                };
                if (_graph.TryGetValue((node.x, node.z - 1), out bottom))
                {
                    node.AddNeighbor(bottom);
                };

            }
        }


        // Return shortest path to the target, null if not reacheable
        public List<Vector3> GetShortestPathTo(Vector3 origin, Vector3 target)
        {
            //Before calculation, we make sure the map is synced
            ResyncMap();

            Vector3 currUnitPosition = origin;

            Node source = _graph[(currUnitPosition.x, currUnitPosition.z)];
            Node targetNode;
            if (!_graph.TryGetValue((target.x, target.z), out targetNode)) return null;
            if (source == targetNode) return null;

            (Dictionary<Node, float> distance, Dictionary<Node, Node> prev) = CalculateShortestPath(source, targetNode);


            if (distance[targetNode] == Mathf.Infinity)
            { 
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

        // Return shortest path to adjacents tile to the target, null if not reachable
        public List<Vector3> GetShortestPathToAdjacent(Vector3 origin, Vector3 target)
        {
            //Before calculation, we make sure the map is synced
            ResyncMap();

            Vector3 currUnitPosition = origin;

            Node source = _graph[(currUnitPosition.x, currUnitPosition.z)];
            Node targetNode;
            if (!_graph.TryGetValue((target.x, target.z), out targetNode)) return null;
            if (source == targetNode) return null;

            (Dictionary<Node, float> distance, Dictionary<Node, Node> prev) = CalculateShortestPath(source, targetNode);

            //If node is not Walkable, then the prev[targetNode] will be unrealiable, since all targetNode distance is Infinity.
            Node closestAdjacentToTarget = targetNode.isWalkable ? prev[targetNode] : targetNode.GetNeighbors().OrderBy((neighbor) => distance[neighbor]).First();

            if (distance[closestAdjacentToTarget] == Mathf.Infinity)
            {
                Debug.Log($"Could not find path to combatTile : {target.x}-{target.z}");
                return null;

            }

            if (distance[closestAdjacentToTarget] == 0)
            {
                return null;
            }

            LinkedList<Node> shortestPath = new LinkedList<Node>();
            shortestPath.AddFirst(closestAdjacentToTarget);

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

        public List<Vector3> GetInRangePosition(Vector3 origin, int minRange, int maxRange)
        {
            Node source = _graph[(origin.x, origin.z)];

            var distance = CalculateNodeDistances(source);
            distance = distance.Where(node => node.Value <= maxRange && node.Value >= minRange).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return distance.Select(node => new Vector3(node.Key.x,0,node.Key.z)).ToList();
        }

        public void PreviewPath(List<Vector3> path)
        {
            foreach(Vector3 pos in path)
            {
                var tile = _mapTiles.GetValueOrDefault((pos.x, pos.z));
                tile.HightLightMovement(true);
            }

            _currPreviewedPath = path;
        }

        public void PreviewRange(List<Vector3> range)
        {
            foreach(Vector3 pos in range)
            {
                var tile = _mapTiles.GetValueOrDefault((pos.x, pos.z));
                tile.HightLightRange(true);
            }

            _currPreviewRange = range;
        }

        public void ClearPreviewPath()
        {
            if (_currPreviewedPath == null) return;

            foreach (Vector3 pos in _currPreviewedPath)
            {
                var tile = _mapTiles.GetValueOrDefault((pos.x, pos.z));
                tile.HightLightMovement(false);
            }

            _currPreviewedPath = null;
        }

        public void ClearPreviewRange()
        {
            if (_currPreviewRange == null) return;

            foreach (Vector3 pos in _currPreviewRange)
            {
                var tile = _mapTiles.GetValueOrDefault((pos.x, pos.z));
                tile.HightLightRange(false);
            }

            _currPreviewRange = null;
        }

        (Dictionary<Node, float>, Dictionary<Node, Node>) CalculateShortestPath(Node source, Node target)
        {
            Dictionary<Node, float> distance = new Dictionary<Node, float>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            List<Node> unvisited = new List<Node>();

            //Calculation Setup
            distance.Add(source, 0f);

            foreach (Node node in _graph.Values)
            {
                if (node != source)
                {
                    distance.Add(node, Mathf.Infinity);
                    prev.Add(node, null);
                }
                unvisited.Add(node);
            }

            while (unvisited.Count > 0)
            {
                Node closest = unvisited.OrderBy(node => distance[node]).First();

                //If we reach the target, we can stop searching
                if (closest == target) break;

                unvisited.Remove(closest);

                foreach (Node node in closest.GetNeighbors())
                {
                    float distanceTo = distance[closest] + closest.DistanceTo(node);
                    if (distanceTo <= distance[node])
                    {
                        distance[node] = distanceTo;
                        prev[node] = closest;
                    }
                }
            }

            return (distance, prev);
        }

        Dictionary<Node, float> CalculateNodeDistances(Node source)
        {
            Dictionary<Node, float> distance = new Dictionary<Node, float>();

            foreach(Node node in _graph.Values)
            {
                distance.Add(node, node.DistanceTo(source, true));
            }

            return distance;
        }

        public GameObject GetUnitAtPos(float x, float z)
        {
            InitializeUnits();

            return _units.GetValueOrDefault((x, z));
        }

        public GameObject GetUnitByInstanceId(int instanceId)
        {
            InitializeUnits();

            return _units.Values.First((unit) => unit.GetInstanceID() == instanceId);
        }

        public List<GameObject> GetUnitsInRange(Vector3 origin, int minRange, int maxRange)
        {
            ResyncMap();
            var range = GetInRangePosition(origin, minRange, maxRange);
            var inRangeUnits = new List<GameObject>();

            foreach(Vector3 inRangePosition in range)
            {
                GameObject inRangeGO;
                if(_units.TryGetValue((inRangePosition.x, inRangePosition.z), out inRangeGO)) inRangeUnits.Add(inRangeGO);
            }
            
            return inRangeUnits;
        }
    }

    class Node
    {
        List<Node> Neighbors;
        public float x { get; }
        public float z { get; }

        public bool isWalkable { get; }
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

        public float DistanceTo(Node node, bool ignoreObstacle = false)
        {
            if (!ignoreObstacle && !node.isWalkable) return Mathf.Infinity;

            return Mathf.Abs(x - node.x) + Mathf.Abs(z - node.z);
        }
    }
}
