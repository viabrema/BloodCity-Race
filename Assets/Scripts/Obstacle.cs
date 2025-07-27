using UnityEngine;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public bool isStatic = false;
    public float baseSpeed = 1f;

    public bool isSolid = false;

    [Header("Penalidade ao Jogador")]
    public float hitPenalty = 10f;
    public GameObject sparkEffectPrefab;
    public AudioSource audioSource;

    [Header("Impacto no Oponente")]
    public float oponentSlowDownFactor = 0.5f;

    public int direction = 0; //1 para baixo, -1 para cima, 0 para sem direção

    private SpriteRenderer spriteRenderer;

    // Para evitar múltiplos efeitos por segundo
    private readonly HashSet<GameObject> triggeredOponents = new HashSet<GameObject>();

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (baseSpeed > 0f && spriteRenderer != null)
        {
            spriteRenderer.flipX = true;
        }
    }

    void Update()
    {
        if (RaceManager.Instance == null || RaceManager.Instance.gameStopped) return;

        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
            return;
        }

        audioSource.volume = RaceManager.Instance.musicVolume * 0.3f;

        float speedMultiplier = isStatic
            ? RaceManager.Instance.currentSpeed
            : (RaceManager.Instance.currentSpeed * 0.1f) + baseSpeed;

        float movement = speedMultiplier * Time.deltaTime;
        transform.Translate(Vector2.left * movement);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Oponent"))
        {
            Oponent oponent = other.GetComponent<Oponent>();
            if (oponent != null)
            {


                if (isSolid)
                {
                    if (oponent.maxSpeed * 0.5f > oponent.currentSpeed)
                        oponent.currentSpeed *= 0.5f;
                    else
                        oponent.currentSpeed = oponent.maxSpeed * 0.5f;
                    return;
                }

                float direction = (transform.position.y > other.transform.position.y) ? -1f : 1f;
                oponent.SetSlowedByObstacle(true, direction, this.GetComponent<Collider2D>());

                if (!triggeredOponents.Contains(oponent.gameObject))
                {
                    HandleOponentCollision();
                    triggeredOponents.Add(oponent.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Oponent"))
        {
            Oponent oponent = other.GetComponent<Oponent>();
            if (oponent != null)
            {
                oponent.SetSlowedByObstacle(false, 0f, null);
                triggeredOponents.Remove(oponent.gameObject);
            }
        }
    }

    private void HandlePlayerCollision()
    {
        PlayCollisionEffects();
        RaceManager.Instance.currentSpeed -= hitPenalty;
        RaceManager.Instance.currentSpeed = Mathf.Max(RaceManager.Instance.currentSpeed, 10f);
    }

    private void HandleOponentCollision()
    {
        PlayCollisionEffects(false);
    }

    private void PlayCollisionEffects(bool shake = true)
    {
        if (sparkEffectPrefab != null)
        {
            Vector3 sparkPosition = new Vector3(transform.position.x, transform.position.y, 1f);
            Instantiate(sparkEffectPrefab, sparkPosition, Quaternion.identity);
        }

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        if (CameraShake.Instance != null && shake)
        {
            CameraShake.Instance.Shake(0.2f, 0.1f);
        }
    }
}
