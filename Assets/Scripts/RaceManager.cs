using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    // public Transform playerTransform;

    [Header("Velocidade")]
    public float distanceTraveled = 0f;
    public float currentSpeed = 0f;
    public float maxSpeed = 50f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float nitroBoost = 20f;
    public float nitroDuration = 1f;
    public float totalRaceDistance = 5000f;
    public string collectedItem = ""; // Guarda o tipo de item coletado (ex: "Nitro", "Shot", etc.)

    public bool startedRace = false;

    [Header("Upgrades")]
    public float verticalSpeed = 5f;
    public int durability = 100;
    public int nitroLevel = 1;

    public float pulseTime = 0f;

    [Header("Oponentes")]
    public Oponent[] oponents;

    public static event System.Action OnOponentsReady;


    public float[] distancesTraveledOponents;

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

        createOponents();
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

    void Update()
    {
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
