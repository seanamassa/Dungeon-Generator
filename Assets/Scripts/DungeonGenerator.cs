/* CMPM 147 Project: Metroidvania Dungeon Generator 
 * Programmed by: Sean Massa 1/22/2026
 * Description: A procedural dungeon layout generator that produces grid based room graphs. 
 * The system generates spatial layouts and includes two distinct logic modes:
 * 1. Standard Mode: Creates linear or branching optimized for flow.
 * 2. Metroidvania Mode: Post-processes the graph to lock the Boss Room 
 * and hide a Key in a distant dead-end, forcing non-linear exploration and backtracking. */

using System.Collections.Generic;
using System.Linq; 
using UnityEngine;


public enum RoomType { None, Start, Normal, Boss, Key, Treasure }

namespace TinyDungeon
{ 
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Generator Settings")]
        [Range(5, 50)] public int maxRooms = 15;
        [Range(0f, 0.85f)] public float branchingFactor = 0.5f;
        public bool useMetroidvaniaLogic = true;

        // grid coordinates
        private Dictionary<Vector2Int, RoomType> rooms = new Dictionary<Vector2Int, RoomType>();
        private List<(Vector2Int, Vector2Int)> corridors = new List<(Vector2Int, Vector2Int)>();
        private List<(Vector2Int, Vector2Int)> lockedDoors = new List<(Vector2Int, Vector2Int)>();
        
        private Vector2Int startPos = Vector2Int.zero;
        private Vector2Int bossPos;
        private Vector2Int keyPos;

        void Start()
        {
            GenerateDungeon();
        }

        public void GenerateDungeon()
        {
            ClearData();
            Generate();
            
            if (useMetroidvaniaLogic)
            {
                MetroidvaniaLayer();
            }
            AdjustCamera();
        }

        private void ClearData()
        {
            rooms.Clear();
            corridors.Clear();
            lockedDoors.Clear();
        }

        private void Generate()
        {
            // initialize start location
            Vector2Int curr = startPos;
            rooms[curr] = RoomType.Start;
            List<Vector2Int> activeRooms = new List<Vector2Int> { curr };
            
            int count = 1;

            // expansion Loop
            while (count < maxRooms && activeRooms.Count > 0)
            {
                // pick random active room
                int idx = Random.Range(0, activeRooms.Count);
                Vector2Int cx = activeRooms[idx];
                Vector2Int cy = activeRooms[idx];

                // define moves
                List<Vector2Int> moves = new List<Vector2Int> 
                { 
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right 
                };
                
                // shuffle moves
                moves = moves.OrderBy(x => Random.value).ToList();

                bool grown = false;
                foreach (Vector2Int dir in moves)
                {
                    if (count >= maxRooms) break;

                    Vector2Int neighbor = cx + dir;

                    if (!rooms.ContainsKey(neighbor))
                    {
                        // add room
                        rooms[neighbor] = RoomType.Normal;
                        corridors.Add((cx, neighbor));
                        activeRooms.Add(neighbor);
                        count++;
                        grown = true;

                        if (Random.value > branchingFactor) break;
                    }
                }

                if (!grown && count < maxRooms)
                {
                    activeRooms.RemoveAt(idx);
                }
            }

            // boss placement (furthest from start)
            if (rooms.Count > 0)
            {
                bossPos = rooms.Keys.OrderByDescending(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.y)).First();
                rooms[bossPos] = RoomType.Boss;
            }
        }

        private void MetroidvaniaLayer()
        {
            // find boss entrance edge
            var bossEdgeIndex = corridors.FindIndex(edge => edge.Item1 == bossPos || edge.Item2 == bossPos);

            if (bossEdgeIndex != -1)
            {
                var edge = corridors[bossEdgeIndex];
                corridors.RemoveAt(bossEdgeIndex);
                lockedDoors.Add(edge);
            }

            // find dead ends to place key
            List<Vector2Int> candidates = new List<Vector2Int>();
            
            // count connections for every room
            Dictionary<Vector2Int, int> connectionCounts = new Dictionary<Vector2Int, int>();
            foreach(var pos in rooms.Keys) connectionCounts[pos] = 0;

            // combine both normal and locked corridors for connectivity check
            var allEdges = new List<(Vector2Int, Vector2Int)>(corridors);
            allEdges.AddRange(lockedDoors);

            foreach (var edge in allEdges)
            {
                if(connectionCounts.ContainsKey(edge.Item1)) connectionCounts[edge.Item1]++;
                if(connectionCounts.ContainsKey(edge.Item2)) connectionCounts[edge.Item2]++;
            }

            foreach (var kvp in connectionCounts)
            {
                if (kvp.Value == 1 && rooms[kvp.Key] == RoomType.Normal)
                {
                    candidates.Add(kvp.Key);
                }
            }

            // place key
            if (candidates.Count > 0)
            {
                keyPos = candidates[Random.Range(0, candidates.Count)];
            }
            else
            {
                // fallback: pick any normal room
                var normalRooms = rooms.Where(r => r.Value == RoomType.Normal).Select(r => r.Key).ToList();
                if (normalRooms.Count > 0) keyPos = normalRooms[Random.Range(0, normalRooms.Count)];
            }

            if (rooms.ContainsKey(keyPos))
            {
                rooms[keyPos] = RoomType.Key;
            }
        }

        private void AdjustCamera()
        {
            if (rooms.Count == 0) return;

            // 1. Calculate the bounds of the dungeon
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var pos in rooms.Keys)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.y > maxY) maxY = pos.y;
            }

            // 2. Find the center point
            // (We multiply by 2 because your visual logic uses * 2 for spacing)
            float centerX = (minX + maxX) / 2f;
            float centerY = (minY + maxY) / 2f;
            
            // 3. Move the Main Camera to the center
            // (Preserve Z = -10 so it doesn't clip through the world)
            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(centerX, centerY, -10);

                // 4. Zoom out to fit everything
                // Calculate required height and width (plus some padding of 2 units)
                float height = (maxY - minY) + 4f; 
                float width = (maxX - minX) + 4f;

                // Determine needed size based on screen aspect ratio
                float targetHeight = height / 2f;
                float targetWidth = (width / Camera.main.aspect) / 2f;

                // Set size to whichever is larger (to ensure nothing is cut off)
                Camera.main.orthographicSize = Mathf.Max(targetHeight, targetWidth, 5f);
            }
        }

        // draws the map
        private void OnDrawGizmos()
        {
            if (rooms == null || rooms.Count == 0) return;

            // draw corridors
            Gizmos.color = Color.black;
            foreach (var edge in corridors)
            {
                Gizmos.DrawLine(new Vector3(edge.Item1.x, edge.Item1.y, 0), new Vector3(edge.Item2.x, edge.Item2.y, 0));
            }

            // draw locked doors
            Gizmos.color = Color.red;
            foreach (var edge in lockedDoors)
            {
                Vector3 start = new Vector3(edge.Item1.x, edge.Item1.y, 0);
                Vector3 end = new Vector3(edge.Item2.x, edge.Item2.y, 0);
                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere((start + end) / 2, 0.15f);
            }

            // draw rooms
            foreach (var kvp in rooms)
            {
                Vector3 pos = new Vector3(kvp.Key.x, kvp.Key.y, 0);
            
                if (kvp.Value == RoomType.Start) Gizmos.color = Color.green;
                else if (kvp.Value == RoomType.Boss) Gizmos.color = Color.red;
                else if (kvp.Value == RoomType.Key) Gizmos.color = Color.cyan;
                else Gizmos.color = Color.gray;

                Gizmos.DrawCube(pos, Vector3.one * 0.5f);
            }
        }
    }
    
}