using UnityEngine;
using System.Collections.Generic;

public class KruskalMazeGenerator : MonoBehaviour
{
    // Inspector'da listede gözükecek veri sınıfı: component değil, sadece veri tutucu.
    [System.Serializable]
    public class SpawnPrefabData
    {
        public GameObject prefab; // Inspector üzerinden atayacağınız prefab
        public int count;         // Bu prefab'dan kaç tane spawnlanacak
    }

    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float wallThickness = 0.1f;

    [Header("Position Offset")]
    public Vector3 floorOffset = Vector3.zero; // İlk floor nesnesinin başlangıç koordinatlarını ayarlar.

    [Header("Prefabs (assign in Inspector)")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    [Header("Additional Spawn Prefabs")]
    public List<SpawnPrefabData> spawnPrefabs = new List<SpawnPrefabData>();

    private MazeCell[,] maze;

    // Labirent hücresinin duvar bilgilerini tutan sınıf (component değildir)
    private class MazeCell
    {
        public bool wallTop = true;
        public bool wallBottom = true;
        public bool wallLeft = true;
        public bool wallRight = true;
    }

    // Hücreler arasındaki bağlantıyı temsil eden kenar yapısı
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

    // Labirenti, Kruskal algoritmasıyla oluşturur.
    void GenerateMaze()
    {
        // Hücreleri oluştur.
        maze = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCell();
            }
        }

        // Union-Find için parent dizisini hazırla.
        parent = new int[width * height];
        for (int i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

        // Komşu hücreler arasındaki kenarları oluştur.
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

        // Kenarları karıştır (Fisher-Yates shuffle)
        for (int i = 0; i < edges.Count; i++)
        {
            Edge temp = edges[i];
            int randomIndex = Random.Range(i, edges.Count);
            edges[i] = edges[randomIndex];
            edges[randomIndex] = temp;
        }

        // Kenarları kontrol edip, duvarları kaldır.
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

        // Giriş ve çıkış için duvar düzenlemeleri.
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

    // Labirenti ve duvarları oluşturur, ayrıca ekstra prefab'ları rastgele spawnlar.
    void DrawMaze()
    {
        GameObject mazeParent = new GameObject("Maze");

        // Zemin (floor) nesnelerini oluştur: Hücre merkezlerine yerleştiriyoruz.
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

        // Duvar nesnelerini oluştur.
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

        // Ekstra spawnlanacak prefab'ları, labirentin rastgele boş hücrelerine yerleştir.
        SpawnAdditionalPrefabs(mazeParent);
    }

    // Ekstra prefab'ları labirentte rastgele boş hücrelere spawnlar.
    void SpawnAdditionalPrefabs(GameObject mazeParent)
    {
        // Tüm hücre merkezlerini içeren boş pozisyon listesini oluştur.
        List<Vector3> availablePositions = new List<Vector3>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                availablePositions.Add(new Vector3(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2, 0) + floorOffset);
            }
        }

        // Her bir prefab için belirlenen sayıda, rastgele boş hücre seçip spawn yap.
        foreach (SpawnPrefabData spawnData in spawnPrefabs)
        {
            for (int i = 0; i < spawnData.count; i++)
            {
                if (availablePositions.Count == 0)
                {
                    Debug.LogWarning("Ekstra prefab spawnlanacak boş pozisyon kalmadı!");
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
}
