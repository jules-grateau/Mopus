using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Controller
{
    public class CombatMap
    {
        Dictionary<(float, float), TileType> _mapTiles;
        List<(float,float)> _obstacles;
        Dictionary<(float, float), Node> _graph;

        public CombatMap(GameObject[] tiles, GameObject[] obstacles)
        {
            InitializeMapTiles(tiles);
            InitializeObstacles(obstacles);
            InitializeGraph();
        }

        public void UpdateObstacles(GameObject[] obstacles)
        {
            InitializeObstacles(obstacles);
            InitializeGraph();
        }

        void InitializeMapTiles(GameObject[] tiles)
        {
            _mapTiles = new Dictionary<(float, float), TileType>();

            foreach (GameObject go in tiles)
            {
                _mapTiles.Add((go.transform.position.x, go.transform.position.z), go.GetComponent<TileType>());
            }
        }

        void InitializeObstacles(GameObject[] obstacles)
        {
            _obstacles = new List<(float, float)>();

            foreach (GameObject go in obstacles)
            {
                _obstacles.Add((go.transform.position.x, go.transform.position.z));
            }
        }

        void InitializeGraph()
        {
            _graph = new Dictionary<(float, float), Node>();

            foreach (var tile in _mapTiles)
            {
                (float, float) tilePos = (tile.Key.Item1, tile.Key.Item2);
                
                bool hasObstacle = _obstacles.Contains(tilePos);

                _graph.Add(tile.Key, new Node(tile.Key.Item1, tile.Key.Item2, !hasObstacle && tile.Value.IsWalkable));
                

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
            Vector3 currUnitPosition = origin;

            Node source = _graph[(currUnitPosition.x, currUnitPosition.z)];
            Node targetNode;
            if (!_graph.TryGetValue((target.x, target.z), out targetNode)) return null;
            if (source == targetNode) return null;

            (Dictionary<Node, float> distance, Dictionary<Node, Node> prev) = CalculateShortestPath(source, targetNode);


            if (distance[targetNode] == Mathf.Infinity)
            {
                Debug.Log($"Could not find path to combatTile : {target.x}-{target.z}");
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
            Vector3 currUnitPosition = origin;

            Node source = _graph[(currUnitPosition.x, currUnitPosition.z)];
            Node targetNode;
            if (!_graph.TryGetValue((target.x, target.z), out targetNode)) return null;
            if (source == targetNode) return null;

            (Dictionary<Node, float> distance, Dictionary<Node, Node> prev) = CalculateShortestPath(source, targetNode);

            Node adjacentTarget = prev[targetNode];


            if (distance[adjacentTarget] == Mathf.Infinity)
            {
                Debug.Log($"Could not find path to combatTile : {target.x}-{target.z}");
                return null;

            }

            LinkedList<Node> shortestPath = new LinkedList<Node>();
            shortestPath.AddFirst(adjacentTarget);

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

                //If we reached the target, we can leave
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
            if (!node.isWalkable) return Mathf.Infinity;

            return Vector2.Distance(
                new Vector2(x, z),
                new Vector2(node.x, node.z));
        }
    }
}
