/* CMPM 147 Project: Metroidvania Dungeon Generator 
* Programmed by: Sean Massa 1/22/2026
* Description: A procedural dungeon layout generator that produces grid based room graphs. 
* The system generates spatial layouts and includes two distinct logic modes:
* 1. Standard Mode: Creates linear or branching optimized for flow.
* 2. Metroidvania Mode: Post-processes the graph to lock the Boss Room 
* and hide a Key in a distant dead-end, forcing non-linear exploration and backtracking.
* 3. (new) Added a loot layer that spawns "loot rooms".  
*/

using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

namespace TinyDungeon
{ 
    public enum RoomType { None, Start, Normal, Boss, Key, Treasure }

    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [Range(5, 50)] public int maxRooms = 15;
        [Range(0f, 1f)] public float branchingFactor = 0.5f;
        public bool useMetroidvaniaLogic = true;

        [Header("Visuals")]
        public GameObject roomPrefab;
        public GameObject linePrefab;

        private Dictionary<Vector2Int, RoomType> rooms = new Dictionary<Vector2Int, RoomType>();
        private List<(Vector2Int, Vector2Int)> corridors = new List<(Vector2Int, Vector2Int)>();
        private List<(Vector2Int, Vector2Int)> lockedDoors = new List<(Vector2Int, Vector2Int)>();
        private Transform dungeonContainer;

        void Start()
        {
            GenerateDungeon();
        }

        [ContextMenu("Generate Dungeon")]
        public void GenerateDungeon()
        {
            ClearData();
            Generate();
            
            if (useMetroidvaniaLogic)
            {
                MetroidvaniaLayer();
            }

            LootLayer();
            SpawnGeometry();
            AdjustCamera();
        }

        private void ClearData()
        {
            rooms.Clear();
            corridors.Clear();
            lockedDoors.Clear();

            if (dungeonContainer != null)
            {
                if (Application.isEditor) DestroyImmediate(dungeonContainer.gameObject);
                else Destroy(dungeonContainer.gameObject);
            }
        }

        private void Generate()
        {
            Vector2Int curr = Vector2Int.zero;
            rooms[curr] = RoomType.Start;
            List<Vector2Int> activeRooms = new List<Vector2Int> { curr };
            
            int count = 1;

            while (count < maxRooms && activeRooms.Count > 0)
            {
                int idx = Random.Range(0, activeRooms.Count);
                Vector2Int cx = activeRooms[idx];
                
                List<Vector2Int> moves = new List<Vector2Int> 
                { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                moves = moves.OrderBy(x => Random.value).ToList();

                bool grown = false;
                foreach (Vector2Int dir in moves)
                {
                    if (count >= maxRooms) break;
                    Vector2Int neighbor = cx + dir;

                    if (!rooms.ContainsKey(neighbor))
                    {
                        rooms[neighbor] = RoomType.Normal;
                        corridors.Add((cx, neighbor));
                        activeRooms.Add(neighbor);
                        count++;
                        grown = true;
                        if (Random.value > branchingFactor) break;
                    }
                }
                if (!grown && count < maxRooms) activeRooms.RemoveAt(idx);
            }

            if (rooms.Count > 0)
            {
                var bossPos = rooms.Keys.OrderByDescending(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.y)).First();
                rooms[bossPos] = RoomType.Boss;
            }
        }

        private void MetroidvaniaLayer()
        {
            // Find the edge connected to the Boss Room and lock it
            var bossEdgeIndex = corridors.FindIndex(edge => rooms[edge.Item1] == RoomType.Boss || rooms[edge.Item2] == RoomType.Boss);
            
            if (bossEdgeIndex != -1)
            {
                var edge = corridors[bossEdgeIndex];
                corridors.RemoveAt(bossEdgeIndex);
                lockedDoors.Add(edge);
            }

            // find dead ends for key placement
            List<Vector2Int> candidates = new List<Vector2Int>();
            Dictionary<Vector2Int, int> connectionCounts = new Dictionary<Vector2Int, int>();
            
            foreach(var pos in rooms.Keys) connectionCounts[pos] = 0;

            var allEdges = new List<(Vector2Int, Vector2Int)>(corridors);
            allEdges.AddRange(lockedDoors);

            foreach (var edge in allEdges)
            {
                if(connectionCounts.ContainsKey(edge.Item1)) connectionCounts[edge.Item1]++;
                if(connectionCounts.ContainsKey(edge.Item2)) connectionCounts[edge.Item2]++;
            }

            // find non boss room that only have 1 connection
            foreach (var kvp in connectionCounts)
            {
                if (kvp.Value == 1 && rooms[kvp.Key] == RoomType.Normal)
                {
                    candidates.Add(kvp.Key);
                }
            }

            // place the Key
            if (candidates.Count > 0)
            {
                var keyPos = candidates[Random.Range(0, candidates.Count)];
                rooms[keyPos] = RoomType.Key;
            }
            else
            {
                var normalRooms = rooms.Where(r => r.Value == RoomType.Normal).Select(r => r.Key).ToList();
                if (normalRooms.Count > 0)
                {
                    var keyPos = normalRooms[Random.Range(0, normalRooms.Count)];
                    rooms[keyPos] = RoomType.Key;
                }
            }
        }

        private void LootLayer()
        {
            // get all normal rooms
            var normalRooms = rooms.Where(r => r.Value == RoomType.Normal).Select(r => r.Key).ToList();
            
            // determine how many loot rooms to spawn
            int lootCount = Mathf.Max(2, normalRooms.Count / 5);

            for (int i = 0; i < lootCount; i++)
            {
                if (normalRooms.Count == 0) break;

                int index = Random.Range(0, normalRooms.Count);
                Vector2Int pos = normalRooms[index];
                
                rooms[pos] = RoomType.Treasure;
                
                normalRooms.RemoveAt(index);
            }
        }

        // visuals
        private void SpawnGeometry()
        {
            dungeonContainer = new GameObject("Dungeon Container").transform;
            dungeonContainer.parent = this.transform;

            float spacing = 1.5f;

            // spawn Rooms
            if (roomPrefab != null)
            {
                foreach (var kvp in rooms)
                {
                    Vector3 pos = new Vector3(kvp.Key.x, kvp.Key.y, 0) * spacing;
                    var instance = Instantiate(roomPrefab, pos, Quaternion.identity, dungeonContainer);
                    
                    var renderer = instance.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (kvp.Value == RoomType.Start) renderer.material.color = Color.green;
                        else if (kvp.Value == RoomType.Boss) renderer.material.color = Color.red;
                        else if (kvp.Value == RoomType.Key) renderer.material.color = Color.cyan;
                        else if (kvp.Value == RoomType.Treasure) renderer.material.color = Color.yellow;
                        else renderer.material.color = Color.gray;
                    }
                    // ensure rooms sit ON TOP of lines
                    if (renderer != null) renderer.sortingOrder = 1;
                }
            }

            // spawn Lines
            if (linePrefab != null)
            {
                // draw normal corridors
                foreach (var edge in corridors)
                {
                    SpawnLine(edge.Item1, edge.Item2, Color.black, spacing);
                }

                // draw locked doors (red)
                foreach (var edge in lockedDoors)
                {
                    SpawnLine(edge.Item1, edge.Item2, Color.red, spacing);
                }
            }
        }

        private void SpawnLine(Vector2Int start, Vector2Int end, Color color, float spacing)
        {
            var lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, dungeonContainer);
            var lr = lineObj.GetComponent<LineRenderer>();
            
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, new Vector3(start.x, start.y, 0) * spacing);
                lr.SetPosition(1, new Vector3(end.x, end.y, 0) * spacing);
                
                lr.startColor = color;
                lr.endColor = color;
                
                lr.sortingOrder = 0; 
            }
        }

        // adjust camera to allow larger dungeons to fit onscreen
        private void AdjustCamera()
        {
            if (rooms.Count == 0 || Camera.main == null) return;

            float spacing = 1.5f;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var pos in rooms.Keys)
            {
                float x = pos.x * spacing;
                float y = pos.y * spacing;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            float width = maxX - minX + 5f;
            float height = maxY - minY + 5f;
            
            Camera.main.transform.position = new Vector3((minX + maxX)/2, (minY + maxY)/2, -10);
            Camera.main.orthographicSize = Mathf.Max(height / 2f, (width / Camera.main.aspect) / 2f);
        }
    }
}