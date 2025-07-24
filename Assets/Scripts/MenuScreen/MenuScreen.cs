using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Importa o SceneManager para troca de cenas

public class MenuScreen : MonoBehaviour
{
    public GameObject background;
    public GameObject player;

    public Button startButton;

    private Vector3 initialPosition;
    private Vector3 playerInitialPosition;

    [Header("Parallax Config")]
    public float maxOffsetX = 50f;
    public float maxOffsetY = 30f;
    public float playerOffsetX = 20;
    public float playerOffsetY = 10;
    public float lerpSpeed = 5f;

    public Slider musicVolumeSlider; // Adiciona um slider para controlar o volume da música
    public AudioSource backgroundMusic; // Adiciona uma referência para a música de fundo

    void Start()
    {
        initialPosition = background.transform.localPosition;
        playerInitialPosition = player.transform.localPosition;

        backgroundMusic = GetComponent<AudioSource>();

        if (backgroundMusic != null)
        {
            backgroundMusic.volume = RaceManager.Instance.musicVolume; // Define o volume da música de fundo
            backgroundMusic.Play(); // Inicia a reprodução da música de fundo
        }

        // Adiciona o listener ao botão de start
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => LoadGameScene());
        }

        // Adiciona o listener ao slider de volume
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener((value) => RaceManager.Instance.ChangeVolume(value));
            musicVolumeSlider.value = RaceManager.Instance.musicVolume; // Sincroniza o slider com o volume inicial
        }
    }

    void Update()
    {
        AnimateBackground();
        AnimatePlayer();

        if (backgroundMusic != null)
        {
            backgroundMusic.volume = RaceManager.Instance.musicVolume; // Atualiza o volume da música de fundo
        }
    }

    void AnimateBackground()
    {
        Vector2 normalizedMouse = GetNormalizedMouse();

        float targetX = initialPosition.x - normalizedMouse.x * maxOffsetX;
        float targetY = initialPosition.y - normalizedMouse.y * maxOffsetY;

        Vector3 currentPos = background.transform.localPosition;
        currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * lerpSpeed);
        currentPos.y = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * lerpSpeed);
        background.transform.localPosition = currentPos;
    }

    void AnimatePlayer()
    {
        Vector2 normalizedMouse = GetNormalizedMouse();

        // Movendo o player MENOS e no MESMO sentido (parecendo mais perto da câmera)
        float targetX = playerInitialPosition.x - normalizedMouse.x * playerOffsetX;
        float targetY = playerInitialPosition.y - normalizedMouse.y * playerOffsetY;

        Vector3 currentPos = player.transform.localPosition;
        currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * lerpSpeed);
        currentPos.y = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * lerpSpeed);
        player.transform.localPosition = currentPos;
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Race01"); // Substitua "GameScene" pelo nome da sua cena de jogo
    }

    Vector2 GetNormalizedMouse()
    {
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float normalizedX = (mouseX / screenWidth - 0.5f) * 2f;
        float normalizedY = (mouseY / screenHeight - 0.5f) * 2f;

        return new Vector2(normalizedX, normalizedY);
    }
}
