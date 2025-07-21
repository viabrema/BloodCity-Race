using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDVelocity : MonoBehaviour
{
    public GameObject velocityBar;
    public RectTransform pointerContainer;
    public GameObject pointerPrefab;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI itemText;

    [Header("Customização")]
    public Color playerColor = Color.white;

    public List<Color> opponentColors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow };
    private float barWidth;
    private RectTransform playerPointer;
    private List<RectTransform> opponentPointers = new List<RectTransform>();

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

        // Cria ponteiros dos oponentes com base na quantidade no RaceManager
        if (RaceManager.Instance != null)
        {

            for (int i = 0; i < opponentColors.Count; i++)
            {
                GameObject o = Instantiate(pointerPrefab, pointerContainer);
                Color colorToUse = (i < opponentColors.Count) ? opponentColors[i] : Color.gray;
                colorToUse.a = 1f;
                o.GetComponent<Image>().color = colorToUse;
                RectTransform rect = o.GetComponent<RectTransform>();
                opponentPointers.Add(rect);
            }
        }
        else
        {
            Debug.LogWarning("HUDVelocity: RaceManager.Instance ainda não está disponível no Start.");
        }
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Atualiza texto de velocidade
        int speed = Mathf.RoundToInt(RaceManager.Instance.currentSpeed);
        velocityText.text = speed.ToString();
        // Atualiza texto de item coletado
        if (!string.IsNullOrEmpty(RaceManager.Instance.collectedItem))
        {
            itemText.text = "Item: " + RaceManager.Instance.collectedItem;
        }
        else
        {
            itemText.text = "Item: Nenhum";
        }

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
