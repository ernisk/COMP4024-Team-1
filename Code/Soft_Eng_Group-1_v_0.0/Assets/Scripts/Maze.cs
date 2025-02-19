using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Her hücre için zemin prefab’ı (Floor)")]
    public GameObject floorPrefab;
    [Tooltip("Duvar prefab’ı (Wall) – Kruskal algoritması sonucunda kaldırılmayan kenarlar burada oluşturulacak.")]
    public GameObject wallPrefab;
    [Tooltip("Ereaser prefab’ı (Hareket edip duvarları yok edecek)")]
    public GameObject ereaserPrefab;

    [Header("Maze Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    [Tooltip("Izgaranın başlangıç (sol alt) konumu")]
    public Vector3 startPosition = new Vector3(-6f, -3f, 0f);

    private float cellSize = 1f; // Floor prefab'ından belirlenecek

    void Start()
    {
        if (floorPrefab == null || wallPrefab == null)
        {
            Debug.LogError("Floor veya Wall prefab'ı atanmamış!");
            return;
        }

        // Hücre boyutunu belirlemek için floorPrefab'ın SpriteRenderer'ını kullanıyoruz.
        SpriteRenderer sr = floorPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            cellSize = sr.bounds.size.x; // kare varsayımına göre
        }

        // 1) Tüm hücreler için Floor prefab'larını oluşturuyoruz.
        GameObject[,] floors = new GameObject[gridWidth, gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Vector3 pos = new Vector3(startPosition.x + i * cellSize, startPosition.y + j * cellSize, startPosition.z);
                floors[i, j] = Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                floors[i, j].name = $"Floor_{i}_{j}";
            }
        }

        // 2) Hücreler arası potansiyel duvar kenarlarını (edge) listeye ekleyelim.
        List<Edge> edges = new List<Edge>();

        // Dikey kenarlar: (i,j) ile (i+1,j)
        for (int i = 0; i < gridWidth - 1; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Edge e = new Edge();
                e.cellA = i * gridHeight + j;
                e.cellB = (i + 1) * gridHeight + j;
                e.isVertical = true;
                e.cellAIndex = new Vector2Int(i, j);
                e.cellBIndex = new Vector2Int(i + 1, j);
                edges.Add(e);
            }
        }
        // Yatay kenarlar: (i,j) ile (i,j+1)
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight - 1; j++)
            {
                Edge e = new Edge();
                e.cellA = i * gridHeight + j;
                e.cellB = i * gridHeight + (j + 1);
                e.isVertical = false;
                e.cellAIndex = new Vector2Int(i, j);
                e.cellBIndex = new Vector2Int(i, j + 1);
                edges.Add(e);
            }
        }

        // 3) Kenar listesini karıştıralım (Fisher–Yates shuffle)
        Shuffle(edges);

        // 4) Union-Find (Disjoint Set) kullanarak Kruskal algoritmasını uygulayalım.
        DisjointSet ds = new DisjointSet(gridWidth * gridHeight);
        List<Edge> wallsToKeep = new List<Edge>();

        foreach (Edge edge in edges)
        {
            if (ds.Find(edge.cellA) != ds.Find(edge.cellB))
            {
                // Eğer iki hücre aynı kümeye ait değilse, kenarı kaldır (yani, geçit oluştur)
                ds.Union(edge.cellA, edge.cellB);
            }
            else
            {
                // Aksi halde kenarı sakla (duvar olarak bırak)
                wallsToKeep.Add(edge);
            }
        }

        // 5) Kruskal sonucuna göre duvar (wall) prefab'larını oluştur.
        foreach (Edge wall in wallsToKeep)
        {
            Vector3 wallPos = Vector3.zero;
            Quaternion wallRot = Quaternion.identity;
            if (wall.isVertical)
            {
                // Dikey duvar: iki hücre arasındaki orta nokta (yatay konumda)
                int i = wall.cellAIndex.x;
                int j = wall.cellAIndex.y;
                wallPos = new Vector3(startPosition.x + (i + 1) * cellSize, startPosition.y + j * cellSize, startPosition.z);
                // Varsayalım wall prefab'ı yatay olarak tasarlandı, bu nedenle 90° döndür
                wallRot = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                // Yatay duvar: (i,j) ve (i,j+1) hücreleri arasındaki orta nokta
                int i = wall.cellAIndex.x;
                int j = wall.cellAIndex.y;
                wallPos = new Vector3(startPosition.x + i * cellSize, startPosition.y + (j + 1) * cellSize, startPosition.z);
                wallRot = Quaternion.identity;
            }
            GameObject w = Instantiate(wallPrefab, wallPos, wallRot, transform);
            w.name = "Wall";
            // İsteğe bağlı: w.transform.localScale = new Vector3(cellSize, wallThickness, 1); gibi ayarlamalar yapılabilir.
        }

        // 6) Dış sınır duvarlarını ekleyelim.
        // Sol ve sağ sınır
        for (int j = 0; j < gridHeight; j++)
        {
            Vector3 posLeft = new Vector3(startPosition.x, startPosition.y + j * cellSize, startPosition.z);
            GameObject wLeft = Instantiate(wallPrefab, posLeft, Quaternion.Euler(0, 0, 90), transform);
            wLeft.name = "Wall";

            Vector3 posRight = new Vector3(startPosition.x + gridWidth * cellSize, startPosition.y + j * cellSize, startPosition.z);
            GameObject wRight = Instantiate(wallPrefab, posRight, Quaternion.Euler(0, 0, 90), transform);
            wRight.name = "Wall";
        }
        // Alt ve üst sınır
        for (int i = 0; i < gridWidth; i++)
        {
            Vector3 posBottom = new Vector3(startPosition.x + i * cellSize, startPosition.y, startPosition.z);
            GameObject wBottom = Instantiate(wallPrefab, posBottom, Quaternion.identity, transform);
            wBottom.name = "Wall";

            Vector3 posTop = new Vector3(startPosition.x + i * cellSize, startPosition.y + gridHeight * cellSize, startPosition.z);
            GameObject wTop = Instantiate(wallPrefab, posTop, Quaternion.identity, transform);
            wTop.name = "Wall";
        }

        // 7) Ereaser prefab'ını, Floor ile aynı boyutta ve başlangıç konumunda oluştur.
        if (ereaserPrefab != null)
        {
            GameObject eraser = Instantiate(ereaserPrefab, startPosition, Quaternion.identity);
            // Floor prefab'ının ölçeğini baz alalım:
            eraser.transform.localScale = floorPrefab.transform.localScale;
        }
    }

    // Fisher-Yates shuffle algoritması
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Kenar (edge) yapısı: iki hücre arasındaki duvarı temsil eder.
    struct Edge
    {
        public int cellA;
        public int cellB;
        public bool isVertical;
        public Vector2Int cellAIndex;
        public Vector2Int cellBIndex;
    }

    // Disjoint Set (Union-Find) sınıfı: Kruskal algoritması için
    class DisjointSet
    {
        int[] parent;
        public DisjointSet(int size)
        {
            parent = new int[size];
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
            }
        }

        public int Find(int i)
        {
            if (parent[i] != i)
                parent[i] = Find(parent[i]);
            return parent[i];
        }

        public void Union(int i, int j)
        {
            int rootI = Find(i);
            int rootJ = Find(j);
            if (rootI != rootJ)
            {
                parent[rootJ] = rootI;
            }
        }
    }
}
