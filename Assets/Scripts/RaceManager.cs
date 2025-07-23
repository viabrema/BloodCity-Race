using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    // public Transform playerTransform;

    [Header("Configurações de Corrida")]
    public float distanceTraveled = 0f;
    public float currentSpeed = 0f;
    public float deceleration = 10f;
    public float totalRaceDistance = 5000f;
    public string collectedItem = ""; // Guarda o tipo de item coletado (ex: "Nitro", "Shot", etc.)
    public bool gameStopped = false;
    public float pulseTime = 0f;

    public bool startedRace = false;

    [Header("Upgrades")]
    public float nitroDuration = 1f;
    public float acceleration = 10f;
    public float maxSpeed = 50f;
    public float nitroBoost = 20f;
    public float verticalSpeed = 5f;
    public int maxPulseTime = 1; // Tempo máximo de efeito do pulso
    public int durability = 100;
    public float nitroFrequency = 0.05f; // Frequência que a maleta de nitro aparece
    public float pulseFrequency = 0.01f; // Frequência que a maleta de pulso aparece

    [Header("Oponentes")]
    public Oponent[] oponents;

    public static event System.Action OnOponentsReady;

    public float[] distancesTraveledOponents;

    [Header("Screens")]
    public GameObject upgradeScreen;
    private bool openedUpgradeScreen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas
            SceneManager.sceneLoaded += OnSceneLoaded; // Registra o evento
        }
        else
        {
            Destroy(gameObject); // garante que só um exista
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Desregistra o evento
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        createOponents(); // Garante que os oponentes sejam criados após o carregamento da cena
    }

    void Start()
    {
        HideUpgradeScreen();
    }

    public void ShowUpgradeScreen()
    {
        if (upgradeScreen != null)
        {
            PauseGame(true);
            upgradeScreen.SetActive(true);
            upgradeScreen.GetComponent<UpgradeScreen>().RandomizeUpgrades();
        }
        else
        {
            Debug.LogWarning("Upgrade Screen not found!");
        }
    }

    public void HideUpgradeScreen()
    {
        if (upgradeScreen != null)
        {
            PauseGame(false);
            upgradeScreen.SetActive(false);

        }
        else
        {
            Debug.LogWarning("Upgrade Screen not found!");
        }
    }

    void createOponents()
    {
        oponents = FindObjectsOfType<Oponent>();
        distancesTraveledOponents = new float[oponents.Length];
        for (int i = 0; i < oponents.Length; i++)
        {
            oponents[i].indexInRaceManager = i;
        }
        OnOponentsReady?.Invoke();
    }

    void PauseGame(bool pause)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        if (pause)
        {
            foreach (var audioSource in audioSources)
            {
                audioSource.Pause();
            }
            Time.timeScale = 0f;
            gameStopped = true;
        }
        else
        {
            foreach (var audioSource in audioSources)
            {
                audioSource.UnPause();
            }
            Time.timeScale = 1f;
            gameStopped = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameStopped = !gameStopped;
            PauseGame(gameStopped);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            openedUpgradeScreen = !openedUpgradeScreen;
            if (openedUpgradeScreen)
            {
                ShowUpgradeScreen();
            }
            else
            {
                HideUpgradeScreen();
            }
        }

        if (gameStopped) return; // Não atualiza enquanto o jogo está pausado

        if (distanceTraveled >= totalRaceDistance)
        {
            Debug.Log("Corrida finalizada!");
            ResetRace();
            return;
        }

        if (pulseTime > 0f)
        {
            pulseTime -= Time.deltaTime;
            if (pulseTime <= 0f)
            {
                pulseTime = 0f;
            }
        }
    }

    public void ResetRace()
    {
        distanceTraveled = 0f;
        startedRace = false;
        currentSpeed = 0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
