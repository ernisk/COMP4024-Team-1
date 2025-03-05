using UnityEngine;
using System.Collections.Generic;

public class KruskalMazeGenerator : MonoBehaviour
{
    // Data class to hold spawn prefab information (non-MonoBehaviour)
    [System.Serializable]
    public class SpawnPrefabData
    {
        public GameObject prefab; // Assign in Inspector
        public int count;         // How many instances to spawn
    }

    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float wallThickness = 0.1f;

    [Header("Position Offset")]
    public Vector3 floorOffset = Vector3.zero; // Adjusts the starting coordinates of the floor objects

    [Header("Prefabs (assign in Inspector)")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    [Header("Additional Spawn Prefabs (random cells)")]
    public List<SpawnPrefabData> spawnPrefabs = new List<SpawnPrefabData>();

    [Header("Solution Path Spawn Prefabs")]
    public List<SpawnPrefabData> solutionSpawnPrefabs = new List<SpawnPrefabData>();

    private MazeCell[,] maze;

    // Class representing each maze cell’s wall configuration
    private class MazeCell
    {
        public bool wallTop = true;
        public bool wallBottom = true;
        public bool wallLeft = true;
        public bool wallRight = true;
    }

    // Structure representing an edge between two neighboring cells
    private struct Edge
    {
        public int x1, y1, x2, y2;
        public Edge(int _x1, int _y1, int _x2, int _y2)
        {
            x1 = _x1;
            y1 = _y1;
            x2 = _x2;
            y2 = _y2;
        }
    }

    private int[] parent;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    // Generates the maze using Kruskal's algorithm.
    void GenerateMaze()
    {
        // Create cells.
        maze = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCell();
            }
        }

        // Initialize union-find parent array.
        parent = new int[width * height];
        for (int i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

        // Create edges between neighboring cells.
        List<Edge> edges = new List<Edge>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1)
                    edges.Add(new Edge(x, y, x + 1, y));
                if (y < height - 1)
                    edges.Add(new Edge(x, y, x, y + 1));
            }
        }

        // Shuffle the edges (Fisher-Yates shuffle)
        for (int i = 0; i < edges.Count; i++)
        {
            Edge temp = edges[i];
            int randomIndex = Random.Range(i, edges.Count);
            edges[i] = edges[randomIndex];
            edges[randomIndex] = temp;
        }

        // Process edges and remove walls accordingly.
        foreach (Edge edge in edges)
        {
            int index1 = edge.x1 + edge.y1 * width;
            int index2 = edge.x2 + edge.y2 * width;
            if (Find(index1) != Find(index2))
            {
                Union(index1, index2);

                if (edge.x1 == edge.x2)
                {
                    if (edge.y1 < edge.y2)
                    {
                        maze[edge.x1, edge.y1].wallTop = false;
                        maze[edge.x2, edge.y2].wallBottom = false;
                    }
                    else
                    {
                        maze[edge.x1, edge.y1].wallBottom = false;
                        maze[edge.x2, edge.y2].wallTop = false;
                    }
                }
                else if (edge.y1 == edge.y2)
                {
                    if (edge.x1 < edge.x2)
                    {
                        maze[edge.x1, edge.y1].wallRight = false;
                        maze[edge.x2, edge.y2].wallLeft = false;
                    }
                    else
                    {
                        maze[edge.x1, edge.y1].wallLeft = false;
                        maze[edge.x2, edge.y2].wallRight = false;
                    }
                }
            }
        }

        // Adjust walls for entry and exit.
        maze[0, 0].wallBottom = false;
        maze[width - 1, height - 1].wallTop = false;
    }

    int Find(int i)
    {
        if (parent[i] != i)
            parent[i] = Find(parent[i]);
        return parent[i];
    }

    void Union(int a, int b)
    {
        int rootA = Find(a);
        int rootB = Find(b);
        if (rootA != rootB)
            parent[rootA] = rootB;
    }

    // Draws the maze (floor, walls, and spawns prefabs)
    void DrawMaze()
    {
        GameObject mazeParent = new GameObject("Maze");

        // Create floor objects for each cell (placed at cell centers).
        if (floorPrefab != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0) + floorOffset;
                    GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity);
                    floor.transform.localScale = new Vector3(cellSize, cellSize, 1);
                    floor.transform.parent = mazeParent.transform;
                }
            }
        }

        // Create wall objects.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellOrigin = new Vector3(x * cellSize, y * cellSize, 0) + floorOffset;

                if (maze[x, y].wallTop)
                {
                    Vector3 pos = cellOrigin + new Vector3(cellSize / 2, cellSize, 0);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, wallThickness, 1);
                    wall.transform.parent = mazeParent.transform;
                }

                if (maze[x, y].wallRight)
                {
                    Vector3 pos = cellOrigin + new Vector3(cellSize, cellSize / 2, 0);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.Euler(0, 0, 90));
                    wall.transform.localScale = new Vector3(cellSize, wallThickness, 1);
                    wall.transform.parent = mazeParent.transform;
                }

                if (y == 0 && maze[x, y].wallBottom)
                {
                    Vector3 pos = cellOrigin + new Vector3(cellSize / 2, 0, 0);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                    wall.transform.localScale = new Vector3(cellSize, wallThickness, 1);
                    wall.transform.parent = mazeParent.transform;
                }

                if (x == 0 && maze[x, y].wallLeft)
                {
                    Vector3 pos = cellOrigin + new Vector3(0, cellSize / 2, 0);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.Euler(0, 0, 90));
                    wall.transform.localScale = new Vector3(cellSize, wallThickness, 1);
                    wall.transform.parent = mazeParent.transform;
                }
            }
        }

        // Spawn additional prefabs in random empty cells.
        SpawnAdditionalPrefabs(mazeParent);

        // Spawn prefabs on the maze's solution path.
        SpawnSolutionPrefabs(mazeParent);
    }

    // Spawns additional prefabs on random available cell centers.
    void SpawnAdditionalPrefabs(GameObject mazeParent)
    {
        // Create a list of available positions (cell centers).
        List<Vector3> availablePositions = new List<Vector3>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                availablePositions.Add(new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0) + floorOffset);
            }
        }

        // For each prefab, spawn 'count' instances in random empty cells.
        foreach (SpawnPrefabData spawnData in spawnPrefabs)
        {
            for (int i = 0; i < spawnData.count; i++)
            {
                if (availablePositions.Count == 0)
                {
                    Debug.LogWarning("No available positions left for additional prefabs!");
                    return;
                }

                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector3 spawnPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                if (spawnData.prefab != null)
                {
                    GameObject spawnedObject = Instantiate(spawnData.prefab, spawnPos, Quaternion.identity);
                    spawnedObject.transform.parent = mazeParent.transform;
                }
            }
        }
    }

    // Spawns prefabs from the solutionSpawnPrefabs list along the maze's solution path.
    void SpawnSolutionPrefabs(GameObject mazeParent)
    {
        List<Vector3> solutionPositions = GetSolutionPathPositions();
        if (solutionPositions.Count == 0)
        {
            Debug.LogWarning("No available solution path positions for solution prefabs!");
            return;
        }

        // For each prefab in the solution list, spawn 'count' instances on random positions along the solution path.
        foreach (SpawnPrefabData spawnData in solutionSpawnPrefabs)
        {
            for (int i = 0; i < spawnData.count; i++)
            {
                if (solutionPositions.Count == 0)
                {
                    Debug.LogWarning("No more available positions on the solution path for solution prefabs!");
                    return;
                }

                int randomIndex = Random.Range(0, solutionPositions.Count);
                Vector3 spawnPos = solutionPositions[randomIndex];
                solutionPositions.RemoveAt(randomIndex);

                if (spawnData.prefab != null)
                {
                    GameObject spawnedObject = Instantiate(spawnData.prefab, spawnPos, Quaternion.identity);
                    spawnedObject.transform.parent = mazeParent.transform;
                }
            }
        }
    }

    // Computes and returns the list of cell positions (in world coordinates) that form the maze's solution path.
    List<Vector3> GetSolutionPathPositions()
    {
        List<Vector2Int> cellPath = SolveMazePath();
        List<Vector3> positions = new List<Vector3>();
        foreach (Vector2Int cell in cellPath)
        {
            positions.Add(new Vector3(cell.x * cellSize + cellSize / 2, cell.y * cellSize + cellSize / 2, 0) + floorOffset);
        }
        return positions;
    }

    // Solves the maze using DFS and returns the cell coordinates (from (0,0) to (width-1,height-1)).
    List<Vector2Int> SolveMazePath()
    {
        bool[,] visited = new bool[width, height];
        List<Vector2Int> path = new List<Vector2Int>();
        bool found = DFS(0, 0, path, visited);
        if (!found)
            Debug.LogWarning("No solution path found!");
        return path;
    }

    // Depth-first search to find a path from (x,y) to the exit.
    bool DFS(int x, int y, List<Vector2Int> path, bool[,] visited)
    {
        // Check if reached exit cell.
        if (x == width - 1 && y == height - 1)
        {
            path.Add(new Vector2Int(x, y));
            return true;
        }

        visited[x, y] = true;
        path.Add(new Vector2Int(x, y));

        // Move Up if no top wall exists.
        if (!maze[x, y].wallTop && y + 1 < height && !visited[x, y + 1])
        {
            if (DFS(x, y + 1, path, visited))
                return true;
        }
        // Move Right if no right wall exists.
        if (!maze[x, y].wallRight && x + 1 < width && !visited[x + 1, y])
        {
            if (DFS(x + 1, y, path, visited))
                return true;
        }
        // Move Down if no bottom wall exists.
        if (!maze[x, y].wallBottom && y - 1 >= 0 && !visited[x, y - 1])
        {
            if (DFS(x, y - 1, path, visited))
                return true;
        }
        // Move Left if no left wall exists.
        if (!maze[x, y].wallLeft && x - 1 >= 0 && !visited[x - 1, y])
        {
            if (DFS(x - 1, y, path, visited))
                return true;
        }

        // Backtrack if no path found.
        path.RemoveAt(path.Count - 1);
        return false;
    }
}
