using UnityEngine;
using UnityEngine.SceneManagement;
public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public Transform playerTransform;

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

    [Header("Oponentes")]
    public Oponent[] oponents;

    public static event System.Action OnOponentsReady;


    // [HideInInspector] // impede o Unity de tentar desenhar isso no Inspector (evita StackOverflow)
    public float[] distancesTraveledOponents;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas
        }
        else
        {
            Destroy(gameObject); // garante que só um exista
        }
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;

        // Pega todos os oponentes existentes na cena
        oponents = FindObjectsOfType<Oponent>();
        // distancesTraveledOponents = new float[oponents.Length];
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
    }

    public void ResetRace()
    {
        distanceTraveled = 0f;

        for (int i = 0; i < oponents.Length; i++)
        {
            oponents[i].distanceTraveled = 0f;
        }

        // Reseta a velocidade e outros parâmetros
        startedRace = false;
        currentSpeed = 0f;

        // Destroi todos os obstáculos
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var obj in obstacles)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
