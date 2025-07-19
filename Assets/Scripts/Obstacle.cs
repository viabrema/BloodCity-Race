using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float baseSpeed = 1f; // multiplicador da velocidade do cenário
    public float hitPenalty = 10f; // quanto o carro perde de velocidade ao colidir

    public GameObject sparkEffectPrefab;

    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Verificar se o obstáculo está a mais de 10 unidades à esquerda
        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
            return;
        }

        float movement = ((RaceManager.Instance.currentSpeed * 0.1f) + baseSpeed) * Time.deltaTime;
        transform.Translate(Vector2.left * movement);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 🔥 Efeito visual
            if (sparkEffectPrefab != null)
            {
                Vector3 sparkPosition = new Vector3(transform.position.x, transform.position.y, 1f);
                Instantiate(sparkEffectPrefab, sparkPosition, Quaternion.identity);
            }

            // 💥 Tocar som de impacto
            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null && audio.clip != null)
            {
                audio.Play();
            }

            // 🔥 TREME A TELA AQUI
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.2f, 0.1f); // duração, magnitude
            }

            // 🐢 Reduz a velocidade
            RaceManager.Instance.currentSpeed -= hitPenalty;
            RaceManager.Instance.currentSpeed = 10f;

            Debug.Log("Player bateu em obstáculo!");
        }
    }
}
