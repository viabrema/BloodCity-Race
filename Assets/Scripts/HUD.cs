using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDVelocity : MonoBehaviour
{
    public GameObject velocityBar;
    public RectTransform pointerContainer; // Container onde os ponteiros estão manualmente
    public GameObject pointerPrefab;       // Ainda usado pro player
    public TextMeshProUGUI velocityText;

    [Header("Customização")]
    public Color playerColor = Color.white;

    [Header("Pointers Manuais")]
    public List<RectTransform> opponentPointers; // Arrastados manualmente

    private float barWidth;
    private RectTransform playerPointer;

    void Start()
    {
        if (velocityBar == null || pointerContainer == null || pointerPrefab == null || velocityText == null)
        {
            Debug.LogError("HUDVelocity: Missing references in the inspector.");
            return;
        }

        barWidth = velocityBar.GetComponent<RectTransform>().sizeDelta.x - 100f;

        // Cria ponteiro do jogador
        GameObject p = Instantiate(pointerPrefab, pointerContainer);
        p.GetComponent<Image>().color = playerColor;
        playerPointer = p.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Atualiza texto de velocidade
        int speed = Mathf.RoundToInt(RaceManager.Instance.currentSpeed);
        velocityText.text = speed.ToString();

        // Atualiza ponteiro do player
        float playerProgress = Mathf.Clamp01(RaceManager.Instance.distanceTraveled / RaceManager.Instance.totalRaceDistance);
        float playerPosition = playerProgress * barWidth;
        playerPointer.anchoredPosition = new Vector2(playerPosition, playerPointer.anchoredPosition.y);

        // Atualiza ponteiros dos oponentes
        for (int i = 0; i < opponentPointers.Count; i++)
        {
            if (i < RaceManager.Instance.distancesTraveledOponents.Length)
            {
                float opponentProgress = Mathf.Clamp01(RaceManager.Instance.distancesTraveledOponents[i] / RaceManager.Instance.totalRaceDistance);
                float opponentPosition = opponentProgress * barWidth;
                opponentPointers[i].anchoredPosition = new Vector2(opponentPosition, opponentPointers[i].anchoredPosition.y);
            }
        }
    }
}
