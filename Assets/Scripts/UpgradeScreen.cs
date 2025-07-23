using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        GameObject upgradeScreen = GameObject.Find("UpgradeScreen");
        RaceManager.Instance.upgradeScreen = upgradeScreen;

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
        motorUpgrades.Add(new MotorUpgrade { name = "Turbo", description = "Aumenta a velocidade máxima em 10%.", type = "maxSpeed" });
        motorUpgrades.Add(new MotorUpgrade { name = "Aceleração", description = "Aumenta a aceleração em +1.", type = "acceleration" });
        motorUpgrades.Add(new MotorUpgrade { name = "Drift", description = "Aumenta o controle de direção.", type = "verticalSpeed" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Carga do Nitro", description = "Aumenta a duração do nitro em 1 segundo.", type = "nitroDuration" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Boost", description = "Aumenta o impulso do nitro em 10%.", type = "nitroBoost" });
        nitroUpgrades.Add(new nitroUpgrade { name = "Frequência do Nitro", description = "Aumenta a frequência de aparecimento do nitro.", type = "nitroFrequency" });
        pulseUpgrades.Add(new pulseUpgrade { name = "Duração do Pulso", description = "Aumenta a duração do pulso em 1 segundo.", type = "pulseDurability" });
        pulseUpgrades.Add(new pulseUpgrade { name = "Frequência do Pulso", description = "Aumenta a frequência de aparecimento do pulso.", type = "pulseFrequency" });

        // Exibe os upgrades iniciais
        RandomizeUpgrades();
        ApplyCardsEvents();
    }

    public void ApplyCardsEvents()
    {
        // Adiciona eventos aos cards
        Card1.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("motor"));
        Card2.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("nitro"));
        Card3.GetComponent<Button>().onClick.AddListener(() => executeUpgrade("pulse"));
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
