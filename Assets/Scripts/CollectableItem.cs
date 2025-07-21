using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configurações")]
    public string type = "default"; // Tipo do item, ex: "turbo", "shield", etc.
    public GameObject collectEffectPrefab; // Prefab de partículas
    public GameObject Bag;

    private bool destroyed = false;

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
        if ((other.CompareTag("Player") || other.CompareTag("Oponent")) && !destroyed)
        {
            TriggerCollectEffect();
            if (collectSound != null)
            {
                collectSound.Play();
            }
            destroyed = true;
            Debug.Log($"Item {type} coletado por {other.tag}!");
            Destroy(Bag);
        }
    }

    private void TriggerCollectEffect()
    {
        if (collectEffectPrefab != null)
        {
            Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y, 1f);
            Instantiate(collectEffectPrefab, effectPosition, Quaternion.identity);
        }
    }
}
