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
}

public class ObstacleSpawner : MonoBehaviour
{
    public float spawnOffset = 10f;

    public Transform obstacleParent;

    private List<ObstacleData> obstacleList = new List<ObstacleData>();
    private int nextObstacleIndex = 0;

    void Start()
    {
        LoadObstacleData();
    }

    void Update()
    {
        if (nextObstacleIndex >= obstacleList.Count) return;

        var current = obstacleList[nextObstacleIndex];

        if (RaceManager.Instance.distanceTraveled >= current.distance)
        {
            SpawnObstacle(current);
            nextObstacleIndex++;
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
        }
    }
}
