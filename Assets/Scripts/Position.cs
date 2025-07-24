using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Importa o DOTween

public class Position : MonoBehaviour
{
    public GameObject[] positionMarkers;
    public int currentPositionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Adiciona animação de subir e descer para cada marcador de posição
        foreach (var marker in positionMarkers)
        {
            if (marker != null)
            {
                marker?.transform?.DOLocalMoveY(marker.transform.localPosition.y + 0.5f, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true); // Ignora Time.timeScale
            }
        }
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
