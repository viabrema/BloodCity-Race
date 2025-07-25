using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ObstacleData
{
    public float distance;
    public float y;
    public string prefab;
    public ObstacleExtraData data;
}

[System.Serializable]
public class ObstacleExtraData
{
    public float baseSpeed;
    public bool isStatic;
}

public class ObstacleSpawner : MonoBehaviour
{
    public float spawnOffset = 10f;

    public Transform obstacleParent;

    private List<ObstacleData> obstacleList = new List<ObstacleData>();
    private int nextObstacleIndex = 0;

    private float nextDroneCheck = 100f;
    private float droneCheckStep = 200f;

    void Start()
    {
        LoadObstacleData();
    }

    void Update()
    {
        if (RaceManager.Instance == null || RaceManager.Instance.gameStopped) return;

        // Spawna obstáculos pré-definidos
        if (nextObstacleIndex < obstacleList.Count)
        {
            var current = obstacleList[nextObstacleIndex];

            if (RaceManager.Instance.distanceTraveled >= current.distance)
            {
                SpawnObstacle(current);
                nextObstacleIndex++;
            }
        }

        // Spawna drones com base em frequência e distância
        if (RaceManager.Instance.distanceTraveled >= nextDroneCheck)
        {
            TrySpawnDrones();
            nextDroneCheck += droneCheckStep;
        }
    }

    void LoadObstacleData()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("obstacles");
        if (jsonText != null)
        {
            obstacleList = new List<ObstacleData>(JsonHelper.FromJson<ObstacleData>(jsonText.text));
        }
        else
        {
            Debug.LogError("Arquivo obstacles.json não encontrado na pasta Resources.");
        }
    }

    void SpawnObstacle(ObstacleData data)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/" + data.prefab);
        if (prefab == null)
        {
            Debug.LogError($"Prefab '{data.prefab}' não encontrado em Resources.");
            return;
        }

        Vector3 spawnPos = new Vector3(
            Camera.main.transform.position.x + spawnOffset,
            data.y,
            0f
        );

        GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity, obstacleParent);
        Obstacle obstacleScript = instance.GetComponent<Obstacle>();
        if (obstacleScript != null && data.data != null)
        {
            obstacleScript.baseSpeed = data.data.baseSpeed;
            obstacleScript.isStatic = data.data.isStatic;
        }
    }

    void TrySpawnDrones()
    {
        float x = Camera.main.transform.position.x + spawnOffset;

        if (Random.value <= RaceManager.Instance.nitroFrequency)
        {
            float yBase = Random.Range(-3f, 3f); // ajuste conforme necessário
            SpawnDrone("DroneNitro", x, yBase + 1f);
        }

        if (Random.value <= RaceManager.Instance.pulseFrequency)
        {
            float yBase = Random.Range(-3f, 3f); // ajuste conforme necessário

            SpawnDrone("DronePulse", x, yBase - 1f);
        }
    }

    void SpawnDrone(string prefabName, float x, float y)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/" + prefabName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab '{prefabName}' não encontrado.");
            return;
        }

        Vector3 spawnPos = new Vector3(x, y, 0f);
        Instantiate(prefab, spawnPos, Quaternion.identity, obstacleParent);
    }
}
