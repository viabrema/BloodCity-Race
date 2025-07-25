using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [Header("Configurações de Corrida")]
    public float distanceTraveled = 0f;
    public float currentSpeed = 0f;
    public float deceleration = 10f;
    public float totalRaceDistance = 5000f;
    public string collectedItem = "";
    public bool gameStopped = false;
    public float pulseTime = 0f;
    public bool gameInitialized = false;
    public bool startedRace = false;
    public float countdown = 5f;
    public bool countdownRunning = false;
    public bool win = false;

    public int attempts = 1;

    [Header("Upgrades")]
    public float nitroDuration = 1f;
    public float acceleration = 10f;
    public float maxSpeed = 50f;
    public float nitroBoost = 20f;
    public float verticalSpeed = 5f;
    public int maxPulseTime = 1;
    public int durability = 100;
    public float nitroFrequency = 0.05f;
    public float pulseFrequency = 0.01f;

    [Header("Oponentes")]
    public Oponent[] oponents;
    public static event System.Action OnOponentsReady;

    [Header("Screens")]
    public GameObject upgradeScreen;
    private bool openedUpgradeScreen = false;

    [Header("Sons")]
    public GameObject[] songs;
    public int selectedSongIndex = 0;
    public float musicVolume = 1f;

    public AudioSource countdownAudioSource;
    public AudioSource goAudioSource;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ChangeVolume(float volume)
    {
        musicVolume = volume;
        for (int i = 0; i < songs.Length; i++)
        {
            AudioSource audioSource = songs[i].GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = (i == selectedSongIndex) ? musicVolume : 0f;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {


        if (scene.name == "Race01")
        {
            countdownAudioSource = GameObject.Find("CountdownSound")?.GetComponent<AudioSource>();
            if (countdownAudioSource != null)
            {
                countdownAudioSource.volume = musicVolume;
            }
            goAudioSource = GameObject.Find("GoSound")?.GetComponent<AudioSource>();
            if (goAudioSource != null)
            {
                goAudioSource.volume = musicVolume;
            }
            createSongs();
            createOponents();
            PlayCurrentSong();
        }
        else if (scene.name == "Dialog")
        {
            if (win)
            {
                Cutscenes.Instance.SetCurrentScene("scene06");
                return;
            }

            if (attempts == 1)
            {
                attempts++;
                Cutscenes.Instance.SetCurrentScene("scene01");
            }
            else if (attempts == 2)
            {
                attempts++;
                Cutscenes.Instance.SetCurrentScene("scene05", 1);
            }
            else if (attempts > 2)
            {
                attempts++;
                Cutscenes.Instance.SetCurrentScene("scene05", 2);
            }
        }

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

        for (int i = 0; i < songs.Length; i++)
        {
            AudioSource audioSource = songs[i].GetComponent<AudioSource>();
            if (audioSource != null)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();

                audioSource.volume = (i == selectedSongIndex) ? musicVolume : 0f;
            }
        }
    }

    void Start()
    {
        StartCountdown();
    }

    public void ShowUpgradeScreen()
    {
        if (upgradeScreen != null)
        {
            upgradeScreen.SetActive(true);
            upgradeScreen.GetComponent<UpgradeScreen>().RandomizeUpgrades();
        }
    }

    public void HideUpgradeScreen()
    {
        if (upgradeScreen != null)
        {
            upgradeScreen.SetActive(false);
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
        if (SceneManager.GetActiveScene().name != "Race01") return;
        songs = GameObject.FindGameObjectsWithTag("Music");
        foreach (var song in songs)
        {
            AudioSource audioSource = song.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.loop = true;
                if (!audioSource.isPlaying)
                    audioSource.Play();

                audioSource.volume = 0f;
            }
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

        List<(float distance, int index, bool isPlayer)> allDistances = new();

        allDistances.Add((distanceTraveled, -1, true));
        for (int i = 0; i < oponents.Length; i++)
        {
            allDistances.Add((oponents[i].distanceTraveled, i, false));
        }

        allDistances.Sort((a, b) => b.distance.CompareTo(a.distance));

        for (int i = 0; i < allDistances.Count; i++)
        {
            if (!allDistances[i].isPlayer)
            {
                int opponentIndex = allDistances[i].index;
                Position position = oponents[opponentIndex].GetComponentInChildren<Position>();
                position.SetPosition(i);
            }
            else
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Position position = player.GetComponentInChildren<Position>();
                position.SetPosition(i);
            }
        }
    }

    public void StartCountdown()
    {
        countdown = 5f;
        countdownRunning = true;
    }

    int lastLoggedCountdown = -1; // Variável para armazenar o último valor logado

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Race01") return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameStopped = !gameStopped;
            PauseGame(gameStopped);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            openedUpgradeScreen = !openedUpgradeScreen;
            if (openedUpgradeScreen)
                ShowUpgradeScreen();
            else
                HideUpgradeScreen();
        }

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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetRace();
        }

        if (gameStopped) return;

        if (distanceTraveled >= totalRaceDistance)
        {
            // Recalcula posições antes de verificar
            CalculeOpoentPositionByDistance();

            // Se o jogador está na frente
            List<(float distance, int index, bool isPlayer)> allDistances = new();
            allDistances.Add((distanceTraveled, -1, true));
            for (int i = 0; i < oponents.Length; i++)
            {
                allDistances.Add((oponents[i].distanceTraveled, i, false));
            }
            allDistances.Sort((a, b) => b.distance.CompareTo(a.distance));

            if (allDistances[0].isPlayer)
            {
                win = true;
                Debug.Log("Você venceu!");
            }
            else
            {
                win = false;
                Debug.Log("Você perdeu!");
            }

            ResetRace();
            SceneManager.LoadScene("Dialog");
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

        if (countdownRunning && countdown > 0f)
        {
            countdown -= Time.deltaTime;

            int currentCountdown = Mathf.CeilToInt(countdown);
            if (currentCountdown != lastLoggedCountdown)
            {
                if (countdownAudioSource != null && currentCountdown > 0)
                {
                    countdownAudioSource.Play();

                }

                if (goAudioSource != null && currentCountdown == 0)
                {
                    goAudioSource.Play();

                }

                Debug.Log("Contagem: " + currentCountdown);
                lastLoggedCountdown = currentCountdown;
            }

            if (countdown <= 0f)
            {
                countdown = 0f;
                countdownRunning = false;
                startedRace = true;
                Debug.Log("Corrida iniciada!");
                // Aqui você pode liberar o movimento do player ou ativar algum flag
            }
        }
    }

    public void ResetRace()
    {
        countdownRunning = false;
        distanceTraveled = 0f;
        startedRace = false;
        currentSpeed = 0f;
        countdown = 5f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
