using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Importa o DOTween
using UnityEngine.EventSystems; // Necessário para EventTrigger e EventTriggerType

public class UpgradeScreen : MonoBehaviour
{

    public List<MotorUpgrade> motorUpgrades;
    public List<nitroUpgrade> nitroUpgrades;
    public List<pulseUpgrade> pulseUpgrades;

    public int selectedMotorUpgradeIndex = 0;
    public int selectedNitroUpgradeIndex = 0;
    public int selectedPulseUpgradeIndex = 0;

    private TextMeshProUGUI motorName;
    private TextMeshProUGUI motorDescription;
    private TextMeshProUGUI nitroName;
    private TextMeshProUGUI nitroDescription;
    private TextMeshProUGUI pulseName;
    private TextMeshProUGUI pulseDescription;

    private GameObject Card1;
    private GameObject Card2;
    private GameObject Card3;

    void Start()
    {

        // Obtém referências aos componentes de texto
        motorName = GameObject.Find("MotorName").GetComponent<TextMeshProUGUI>();
        motorDescription = GameObject.Find("MotorDescription").GetComponent<TextMeshProUGUI>();
        nitroName = GameObject.Find("NitroName").GetComponent<TextMeshProUGUI>();
        nitroDescription = GameObject.Find("NitroDescription").GetComponent<TextMeshProUGUI>();
        pulseName = GameObject.Find("PulseName").GetComponent<TextMeshProUGUI>();
        pulseDescription = GameObject.Find("PulseDescription").GetComponent<TextMeshProUGUI>();

        // Obtém referências aos cards
        Card1 = GameObject.Find("Card1");
        Card2 = GameObject.Find("Card2");
        Card3 = GameObject.Find("Card3");

        // Inicializa as listas de upgrades
        motorUpgrades = new List<MotorUpgrade>();
        nitroUpgrades = new List<nitroUpgrade>();
        pulseUpgrades = new List<pulseUpgrade>();

        // Exemplo de adição de upgrades
        motorUpgrades.Add(new MotorUpgrade { name = "Turbo", description = "Aumenta a velocidade máxima", type = "maxSpeed" });
        motorUpgrades.Add(new MotorUpgrade { name = "Aceleração", description = "Aumenta a aceleração", type = "acceleration" });
        motorUpgrades.Add(new MotorUpgrade { name = "Drift", description = "Melhora o controle durante curvas", type = "verticalSpeed" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Carga do Nitro", description = "Aumenta a duração do nitro", type = "nitroDuration" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Boost", description = "Aumenta o impulso do nitro", type = "nitroBoost" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Frequência do Nitro", description = "Aumenta a chance da nitro aparecer", type = "nitroFrequency" });
        pulseUpgrades.Add(new pulseUpgrade { name = "Duração do Pulso", description = "Aumenta a duração da pulso elétrica", type = "pulseDurability" });
        pulseUpgrades.Add(new pulseUpgrade { name = "Frequência da Pulso", description = "Aumenta a chance da pulso elétrica aparecer", type = "pulseFrequency" });

        // Exibe os upgrades iniciais
        RandomizeUpgrades();
        ApplyCardsEvents();
        RaceManager.Instance.UpgradeScreenStarted();
    }

    public void ApplyCardsEvents()
    {
        // Adiciona eventos aos cards
        Card1.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("motor"));
        Card2.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("nitro"));
        Card3.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("pulse"));

        // Adiciona animações ao passar o mouse
        AddHoverAnimation(Card1);
        AddHoverAnimation(Card2);
        AddHoverAnimation(Card3);
    }

    void AddHoverAnimation(GameObject card)
    {
        EventTrigger trigger = card.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = card.AddComponent<EventTrigger>();
        }

        // Evento para quando o mouse entra
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (!card.transform) return;
            card.transform.DOKill(); // Cancela animações anteriores
            card.transform.DOScale(8f, 0.2f).SetUpdate(true); // Zoom
            card.transform.DOShakeRotation(0.5f, new Vector3(0, 0, 10), vibrato: 10, randomness: 90).SetUpdate(true); // Efeito de shake
        });
        trigger.triggers.Add(pointerEnter);

        // Evento para quando o mouse sai
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            if (!card.transform) return;
            card.transform.DOKill(); // Cancela animações anteriores
            card.transform.DOScale(7f, 0.2f).SetUpdate(true); // Volta ao tamanho original
            card.transform.DORotate(Vector3.zero, 0.5f).SetUpdate(true); // Volta à rotação original
        });
        trigger.triggers.Add(pointerExit);
    }

    public void executeUpgrade(string type = "motor")
    {
        if (type == "motor")
        {
            if (selectedMotorUpgradeIndex < 0 || selectedMotorUpgradeIndex >= motorUpgrades.Count) return;
            MotorUpgrade upgrade = motorUpgrades[selectedMotorUpgradeIndex];
            upgrade.execute();
            Debug.Log("Motor Upgrade Executado: " + upgrade.name);
        }
        else if (type == "nitro")
        {
            if (selectedNitroUpgradeIndex < 0 || selectedNitroUpgradeIndex >= nitroUpgrades.Count) return;
            nitroUpgrade upgrade = nitroUpgrades[selectedNitroUpgradeIndex];
            upgrade.execute();
            Debug.Log("Nitro Upgrade Executado: " + upgrade.name);
        }
        else if (type == "pulse")
        {
            if (selectedPulseUpgradeIndex < 0 || selectedPulseUpgradeIndex >= pulseUpgrades.Count) return;
            pulseUpgrade upgrade = pulseUpgrades[selectedPulseUpgradeIndex];
            upgrade.execute();
            Debug.Log("Pulse Upgrade Executado: " + upgrade.name);
        }
        else
        {
            Debug.LogError("Tipo de upgrade desconhecido: " + type);
        }


        RaceManager.Instance.HideUpgradeScreen();
        RaceManager.Instance.StartCountdown();

    }

    public void ShowMotorUpgrade(int index)
    {
        if (index < 0 || index >= motorUpgrades.Count) return;

        MotorUpgrade upgrade = motorUpgrades[index];
        motorName.text = upgrade.name;
        motorDescription.text = upgrade.description;
    }

    public void ShowNitroUpgrade(int index)
    {
        if (index < 0 || index >= nitroUpgrades.Count) return;

        nitroUpgrade upgrade = nitroUpgrades[index];
        nitroName.text = upgrade.name;
        nitroDescription.text = upgrade.description;
    }

    public void ShowPulseUpgrade(int index)
    {
        if (index < 0 || index >= pulseUpgrades.Count) return;

        pulseUpgrade upgrade = pulseUpgrades[index];
        pulseName.text = upgrade.name;
        pulseDescription.text = upgrade.description;
    }

    public void RandomizeUpgrades()
    {
        // Exemplo de como randomizar upgrades
        selectedMotorUpgradeIndex = Random.Range(0, motorUpgrades.Count);
        ShowMotorUpgrade(selectedMotorUpgradeIndex);

        selectedNitroUpgradeIndex = Random.Range(0, nitroUpgrades.Count);
        ShowNitroUpgrade(selectedNitroUpgradeIndex);

        selectedPulseUpgradeIndex = Random.Range(0, pulseUpgrades.Count);
        ShowPulseUpgrade(selectedPulseUpgradeIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
