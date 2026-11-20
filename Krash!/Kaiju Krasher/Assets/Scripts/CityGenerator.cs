using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> buildingPrefabs = new List<GameObject>();
    public List<GameObject> carPrefabs = new List<GameObject>();
    public List<GameObject> propPrefabs = new List<GameObject>();

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
    public float propSpawnRadius = 12f;
    public float propOverlapCheckRadius = 1.5f;

    [Header("Player Spawn")]
    [Tooltip("World position the player starts at. The city will be centered on this point.")]
    public Vector3 playerSpawnPoint = Vector3.zero;
    [Tooltip("Radius around the player that stays clear of buildings.")]
    public float safeSpawnRadius = 18f;

    [Header("Seed")]
    public int seed = 42;
    public bool generateOnStart = true;
    public bool randomizeSeedOnStart = true;

    private Transform cityRoot;
    private List<Vector3> spawnedPropPositions = new List<Vector3>();

    void Start()
    {
        if (generateOnStart)
        {
            if (randomizeSeedOnStart)
                seed = Random.Range(int.MinValue, int.MaxValue);
            GenerateCity();
        }
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        ClearCity();
        spawnedPropPositions.Clear();

        Random.InitState(seed);
        cityRoot = new GameObject("GeneratedCity").transform;
        cityRoot.SetParent(transform);

        float step = cellSize + roadWidth;

        // Offset so the center of the grid sits on the player spawn point
        float offsetX = -(gridWidth - 1) * step * 0.5f;
        float offsetZ = -(gridDepth - 1) * step * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 cellCenter = new Vector3(
                    x * step + offsetX + playerSpawnPoint.x,
                    playerSpawnPoint.y,
                    z * step + offsetZ + playerSpawnPoint.z
                );

                // Skip buildings too close to the player
                bool tooClose = Vector3.Distance(cellCenter, playerSpawnPoint) < safeSpawnRadius;

                if (!tooClose && buildingPrefabs.Count > 0 && Random.value < buildingChance)
                {
                    GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
                    GameObject building = Instantiate(prefab, cityRoot);

                    float scale = Random.Range(minBuildingScale, maxBuildingScale);
                    building.transform.localScale = Vector3.one * scale;
                    building.transform.position = cellCenter;
                    building.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

                    SpawnCarsAroundBuilding(cellCenter);
                }

                // Props still spawn (including near the player, if you want)
                SpawnPropsInCell(cellCenter);
            }
        }

        Debug.Log($"City generated with seed {seed} (centered on {playerSpawnPoint})");
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
            spawnPos.y = buildingPos.y + carHeightOffset;

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
        float minPropDistance = propOverlapCheckRadius;   // reuse the existing slider as the min distance

        for (int i = 0; i < propCount; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(0f, propSpawnRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);

            Vector3 spawnPos = cellCenter + offset;
            spawnPos.y = cellCenter.y + propHeightOffset;

            // Check distance against every prop already spawned
            bool tooClose = false;
            foreach (Vector3 existing in spawnedPropPositions)
            {
                if (Vector3.Distance(spawnPos, existing) < minPropDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            // Optional: still do a quick physics check against buildings/ground if you want
            Vector3 checkPos = spawnPos + Vector3.up * 0.75f;
            if (Physics.CheckSphere(checkPos, 0.5f, ~0, QueryTriggerInteraction.Ignore))
            {
                // Uncomment the next line if you also want to avoid buildings
                // continue;
            }

            GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Count)];
            GameObject prop = Instantiate(propPrefab, cityRoot);
            prop.transform.position = spawnPos;
            prop.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Remember this prop’s position so later props avoid it
            spawnedPropPositions.Add(spawnPos);
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