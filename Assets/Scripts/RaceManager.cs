using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Necessário para List<>
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
    public bool gameInitialized = false;
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

    [Header("Screens")]
    public GameObject upgradeScreen;
    private bool openedUpgradeScreen = false;

    [Header("Sons")]

    public GameObject[] songs;

    public int selectedSongIndex = 0;

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
        createSongs();
        createOponents(); // Garante que os oponentes sejam criados após o carregamento da cena
        // selectedSongIndex = (selectedSongIndex + 1) % songs.Length;
        PlayCurrentSong();

    }

    public void UpgradeScreenStarted()
    {
        upgradeScreen = GameObject.Find("UpgradeScreen");
        if (!gameInitialized)
        {
            HideUpgradeScreen();
            gameInitialized = true;

        }
        else
        {
            ShowUpgradeScreen();
        }
    }

    void PlayCurrentSong()
    {
        if (songs.Length == 0) return;

        // Para todas as músicas
        foreach (var song in songs)
        {
            if (song != null)
            {
                AudioSource audioSource = song.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }

        // Toca a música selecionada
        if (selectedSongIndex < songs.Length && songs[selectedSongIndex] != null)
        {
            AudioSource selectedAudio = songs[selectedSongIndex].GetComponent<AudioSource>();
            if (selectedAudio != null)
            {
                selectedAudio.Play();
            }
        }
    }

    void Start()
    {

    }

    public void ShowUpgradeScreen()
    {
        if (upgradeScreen != null)
        {
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
        for (int i = 0; i < oponents.Length; i++)
        {
            oponents[i].indexInRaceManager = i;
        }
        OnOponentsReady?.Invoke();
    }

    void createSongs()
    {
        //busca pela tag "Music"
        songs = GameObject.FindGameObjectsWithTag("Music");
        if (songs.Length == 0)
        {
            Debug.LogWarning("Nenhum AudioSource encontrado na cena!");
        }
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

    public void CalculeOpoentPositionByDistance()
    {
        if (oponents == null || oponents.Length == 0) return;

        // Cria uma lista com as distâncias percorridas por todos (player e oponentes)
        List<(float distance, int index, bool isPlayer)> allDistances = new List<(float, int, bool)>();

        // Adiciona a distância do player
        allDistances.Add((distanceTraveled, -1, true)); // -1 para identificar o player

        // Adiciona as distâncias dos oponentes
        for (int i = 0; i < oponents.Length; i++)
        {
            allDistances.Add((oponents[i].distanceTraveled, i, false));
        }

        // Ordena pela distância em ordem decrescente
        allDistances.Sort((a, b) => b.distance.CompareTo(a.distance));

        // Atualiza a posição dos oponentes
        for (int i = 0; i < allDistances.Count; i++)
        {
            if (!allDistances[i].isPlayer)
            {
                int opponentIndex = allDistances[i].index;
                Position position = oponents[opponentIndex].GetComponentInChildren<Position>();
                position.SetPosition(i); // Define a posição do oponente
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Position position = player.GetComponentInChildren<Position>();
                position.SetPosition(i); // Define a posição do jogador
            }

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

        // "q" e "e" trocam a de música
        if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedSongIndex = (selectedSongIndex - 1 + songs.Length) % songs.Length;
            PlayCurrentSong();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            selectedSongIndex = (selectedSongIndex + 1) % songs.Length;
            PlayCurrentSong();
        }

        // ========== CONTROLE DE INÍCIO E REINÍCIO ==========
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (openedUpgradeScreen) return;
            startedRace = true;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetRace();
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

        CalculeOpoentPositionByDistance();
    }

    public void ResetRace()
    {
        distanceTraveled = 0f;
        startedRace = false;
        currentSpeed = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
