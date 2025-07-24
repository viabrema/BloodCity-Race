using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public GameObject[] positionMarkers;
    public int currentPositionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetPosition(int index)
    {
        if (index < 0 || index >= positionMarkers.Length)
        {
            Debug.LogError("Índice de posição inválido: " + index);
            return;
        }

        currentPositionIndex = index;
    }

    // Update is called once per frame
    void Update()
    {
        if (RaceManager.Instance == null || RaceManager.Instance.gameStopped) return;

        for (int i = 0; i < positionMarkers.Length; i++)
        {
            if (i == currentPositionIndex)
            {
                positionMarkers[i].SetActive(true);
            }
            else
            {
                positionMarkers[i].SetActive(false);
            }
        }

    }
}
