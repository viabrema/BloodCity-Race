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

    public TextMeshProUGUI CountdownText; // Adiciona o campo para o texto de contagem regressiva

    [Header("Customização")]
    public Color playerColor = Color.white;

    private float barWidth;
    private RectTransform playerPointer;
    private List<RectTransform> opponentPointers = new List<RectTransform>();

    void Awake()
    {
        RaceManager.OnOponentsReady += CreateOpponentPointers;
    }

    void OnDestroy()
    {
        RaceManager.OnOponentsReady -= CreateOpponentPointers;
    }

    void Start()
    {

        if (velocityBar == null || pointerContainer == null || pointerPrefab == null || velocityText == null)
        {
            Debug.LogError("HUDVelocity: Missing references in the inspector.");
            return;
        }

        barWidth = velocityBar.GetComponent<RectTransform>().sizeDelta.x;

        // Cria ponteiro do jogador
        GameObject p = Instantiate(pointerPrefab, pointerContainer);
        p.GetComponent<Image>().color = playerColor;
        playerPointer = p.GetComponent<RectTransform>();
        playerPointer.SetSiblingIndex(10);
    }

    void CreateOpponentPointers()
    {

        // Remove ponteiros antigos, se existirem
        foreach (var old in opponentPointers)
        {
            if (old != null) Destroy(old.gameObject);
        }
        opponentPointers.Clear();

        // Cria novos ponteiros

        for (int i = 0; i < RaceManager.Instance.oponents.Length; i++)
        {
            GameObject o = Instantiate(pointerPrefab, pointerContainer);
            if (RaceManager.Instance.oponents[i] == null) continue;
            Color colorToUse = RaceManager.Instance.oponents[i].pointColor;
            colorToUse.a = 1f;
            o.GetComponent<Image>().color = colorToUse;
            RectTransform rect = o.GetComponent<RectTransform>();
            opponentPointers.Add(rect);
        }
    }

    void Update()
    {
        if (RaceManager.Instance == null || RaceManager.Instance.gameStopped) return;

        // Atualiza texto de velocidade
        int speed = Mathf.RoundToInt(RaceManager.Instance.currentSpeed);
        velocityText.text = Mathf.RoundToInt(speed * 2.2f).ToString();

        if (RaceManager.Instance.countdownRunning)
        {
            // Atualiza texto de contagem regressiva
            CountdownText.text = Mathf.CeilToInt(RaceManager.Instance.countdown).ToString();
        }
        else
        {
            CountdownText.text = "";
        }

        // Atualiza item coletado
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
            if (i < RaceManager.Instance.oponents.Length)
            {
                float opponentProgress = Mathf.Clamp01(RaceManager.Instance.oponents[i].distanceTraveled / RaceManager.Instance.totalRaceDistance);
                float opponentPosition = opponentProgress * barWidth;
                opponentPointers[i].anchoredPosition = new Vector2(opponentPosition, opponentPointers[i].anchoredPosition.y);
            }
        }
    }
}
