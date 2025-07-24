using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configurações")]
    public string type = "default"; // Tipo do item, ex: "turbo", "shield", etc.
    public GameObject collectEffect; // Prefab de partículas
    public GameObject Bag;

    [Header("Estado")]
    public bool destroyed = false;

    private AudioSource collectSound;


    void Start()
    {
        collectSound = GetComponent<AudioSource>();

        if (collectSound == null)
        {
            Debug.LogError("CollectibleItem: AudioSource not found on the item!");
        }
    }
    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Move-se para a esquerda com a mesma velocidade do player
        float movement = RaceManager.Instance.currentSpeed * Time.deltaTime;
        movement *= 0.1f; // Ajuste para suavizar o movimento do item
        transform.Translate(Vector2.left * movement);

        // Destroi caso saia muito da tela
        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D itemCollider = GetComponent<Collider2D>();
        if ((other.CompareTag("Player") || other.CompareTag("Oponent")) && !destroyed)
        {
            TriggerCollectEffect();
            if (collectSound != null)
            {
                collectSound.volume = RaceManager.Instance.musicVolume; // Ajusta o volume do som
                collectSound.Play();
            }
            itemCollider.enabled = false; // Desativa o collider para evitar múltiplas coletas
            destroyed = true;
            Debug.Log($"Item {type} coletado por {other.tag}!");
            Destroy(Bag);
        }
    }

    private void TriggerCollectEffect()
    {
        if (collectEffect != null)
        {

            ParticleSystem ps = collectEffect.GetComponent<ParticleSystem>();
            Debug.Log("Efeito de coleta ativado!", ps);
            if (ps != null)
            {
                ps.Play();
            }

        }
    }
}
