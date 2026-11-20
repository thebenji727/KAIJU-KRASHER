using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> buildingPrefabs = new List<GameObject>();
    public List<GameObject> carPrefabs = new List<GameObject>();
    public List<GameObject> propPrefabs = new List<GameObject>();   // Fountains, lights, trash, bushes, trees, etc.

    [Header("Grid")]
    public int gridWidth = 6;
    public int gridDepth = 6;
    public float cellSize = 26f;
    public float roadWidth = 10f;

    [Header("Buildings")]
    public float minBuildingScale = 0.8f;
    public float maxBuildingScale = 1.3f;
    [Range(0f, 1f)] public float buildingChance = 0.85f;

    [Header("Cars")]
    public int minCarsPerBuilding = 1;
    public int maxCarsPerBuilding = 3;
    public float carHeightOffset = 0.15f;
    public float carSpawnRadius = 10f;

    [Header("Props")]
    public int minPropsPerCell = 2;
    public int maxPropsPerCell = 5;
    public float propHeightOffset = 0f;
    public float propSpawnRadius = 12f;          // How far from cell center they can appear
    public float propOverlapCheckRadius = 1.5f;  // How strict the "don't clip" check is

    [Header("Seed")]
    public int seed = 42;
    public bool generateOnStart = true;
    [Tooltip("If true, a new random seed is chosen every time the app starts so the city looks different each load.")]
    public bool randomizeSeedOnStart = true;

    private Transform cityRoot;

    void Start()
    {
        if (generateOnStart)
        {
            if (randomizeSeedOnStart)
            {
                // Unity's Random is already non-deterministic across app launches,
                // so this gives a fresh seed every time the player is run.
                seed = Random.Range(int.MinValue, int.MaxValue);
            }
            GenerateCity();
        }
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        ClearCity();

        Random.InitState(seed);
        cityRoot = new GameObject("GeneratedCity").transform;
        cityRoot.SetParent(transform);

        float step = cellSize + roadWidth;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 cellCenter = new Vector3(x * step, 0f, z * step);

                // Building
                if (buildingPrefabs.Count > 0 && Random.value < buildingChance)
                {
                    GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
                    GameObject building = Instantiate(prefab, cityRoot);

                    float scale = Random.Range(minBuildingScale, maxBuildingScale);
                    building.transform.localScale = Vector3.one * scale;
                    building.transform.position = cellCenter;
                    building.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

                    SpawnCarsAroundBuilding(cellCenter);
                }

                // Props scattered in the cell (and nearby roads)
                SpawnPropsInCell(cellCenter);
            }
        }

        Debug.Log($"City generated with seed {seed}");
    }

    void SpawnCarsAroundBuilding(Vector3 buildingPos)
    {
        if (carPrefabs.Count == 0) return;

        int carCount = Random.Range(minCarsPerBuilding, maxCarsPerBuilding + 1);
        float minDistance = 7f;
        float maxDistance = carSpawnRadius;

        for (int i = 0; i < carCount; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);

            Vector3 spawnPos = buildingPos + offset;
            spawnPos.y = carHeightOffset;

            GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
            GameObject car = Instantiate(carPrefab, cityRoot);
            car.transform.position = spawnPos;
            car.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
    }

    void SpawnPropsInCell(Vector3 cellCenter)
    {
        if (propPrefabs.Count == 0) return;

        int propCount = Random.Range(minPropsPerCell, maxPropsPerCell + 1);

        for (int i = 0; i < propCount; i++)
        {
            // Random position inside the cell + a bit of the surrounding roads
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(0f, propSpawnRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);

            Vector3 spawnPos = cellCenter + offset;
            spawnPos.y = propHeightOffset;

            // Overlap check — if anything is already too close, skip this prop
            if (Physics.CheckSphere(spawnPos, propOverlapCheckRadius))
            {
                continue;   // Something is already here, discard this prop
            }

            GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Count)];
            GameObject prop = Instantiate(propPrefab, cityRoot);
            prop.transform.position = spawnPos;
            prop.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Optional: give props a collider so later props also avoid them
            // (most prefabs already have colliders; this is just a safety net)
            if (prop.GetComponent<Collider>() == null)
            {
                var col = prop.AddComponent<SphereCollider>();
                col.radius = propOverlapCheckRadius * 0.6f;
                col.isTrigger = true;
            }
        }
    }

    [ContextMenu("Clear City")]
    public void ClearCity()
    {
        if (cityRoot != null)
        {
            DestroyImmediate(cityRoot.gameObject);
            cityRoot = null;
        }
    }
}