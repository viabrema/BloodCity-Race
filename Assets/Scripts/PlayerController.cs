using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    private bool isNitroActive = false;
    private float verticalVelocity = 0f;
    private AudioSource accelerationSound;

    private ParticleSystem nitroEffect;

    void Start()
    {
        accelerationSound = GetComponent<AudioSource>();
        accelerationSound.loop = true;
        accelerationSound.Play(); // Garante que o som comece tocando
        nitroEffect = GameObject.Find("Turbo")?.GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            CollectibleItem item = other.GetComponent<CollectibleItem>();
            if (item != null)
            {
                string collectedType = item.type;
                Debug.Log("Item coletado do tipo: " + collectedType);

                RaceManager.Instance.collectedItem = collectedType;
            }
        }
    }

    void Update()
    {
        if (RaceManager.Instance == null || RaceManager.Instance.gameStopped) return;
        HandleInput();
        RaceManager.Instance.distanceTraveled += RaceManager.Instance.currentSpeed * Time.deltaTime;

        // Ajuste de volume e pitch do som de aceleração
        float speedPercent = RaceManager.Instance.currentSpeed / RaceManager.Instance.maxSpeed;
        speedPercent = Mathf.Clamp01(speedPercent);
        accelerationSound.volume = RaceManager.Instance.musicVolume * 0.5f;
        accelerationSound.pitch = 0.5f + speedPercent * 5f;

        // Inclinação visual do carro ao subir ou descer
        float tiltAngle = verticalVelocity * 5f; // valor reduzido para suavidade
        Quaternion targetRotation = Quaternion.Euler(0, 0, tiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Efeito de nitro
        if (RaceManager.Instance.currentSpeed > RaceManager.Instance.maxSpeed)
        {
            if (!nitroEffect.isPlaying)
            {
                nitroEffect.Play();
            }
        }
        else
        {
            if (nitroEffect.isPlaying)
            {
                nitroEffect.Stop();
            }
        }

        // --- Se corrida ainda não começou, trava a velocidade ---
        if (!RaceManager.Instance.startedRace)
        {
            RaceManager.Instance.currentSpeed = 0f;
        }
    }

    void HandleInput()
    {
        if (RaceManager.Instance == null) return;

        // ========== MOVIMENTO VERTICAL COM INÉRCIA ==========
        float vInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) vInput = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vInput = -1f;

        float targetVelocity = vInput * RaceManager.Instance.verticalSpeed;
        verticalVelocity = Mathf.Lerp(verticalVelocity, targetVelocity, Time.deltaTime * 5f);
        transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime, Space.World);

        // ========== ACELERAÇÃO COM LIMITE DINÂMICO ==========
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float maxAllowed = isNitroActive ? float.MaxValue : RaceManager.Instance.maxSpeed;
            if (RaceManager.Instance.currentSpeed < maxAllowed)
            {
                RaceManager.Instance.currentSpeed += RaceManager.Instance.acceleration * Time.deltaTime;
                RaceManager.Instance.currentSpeed = Mathf.Min(RaceManager.Instance.currentSpeed, maxAllowed);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            RaceManager.Instance.currentSpeed -= RaceManager.Instance.deceleration * Time.deltaTime * 10f;
        }
        else
        {
            RaceManager.Instance.currentSpeed = Mathf.MoveTowards(
                RaceManager.Instance.currentSpeed,
                0,
                RaceManager.Instance.deceleration * Time.deltaTime
            );
        }

        // Clamp inferior
        RaceManager.Instance.currentSpeed = Mathf.Max(RaceManager.Instance.currentSpeed, 0);

        // ========== EXECUTA O ITEM ==========
        if (Input.GetKeyDown(KeyCode.Space) && !isNitroActive)
        {
            // StartCoroutine(ActivateNitro());
            if (RaceManager.Instance.collectedItem == "nitro")
            {
                StartCoroutine(ActivateNitro());
                RaceManager.Instance.collectedItem = ""; // Limpa o item após uso
            }
            else if (RaceManager.Instance.collectedItem == "pulse")
            {
                GameObject pulse = GameObject.Find("PulseEffect");
                ParticleSystem pulseEffect = pulse.GetComponent<ParticleSystem>();
                pulseEffect.Clear();
                pulseEffect.Play();

                AudioSource audio = pulse.GetComponent<AudioSource>();
                audio.volume = RaceManager.Instance.musicVolume * 0.5f;
                audio.Play();

                ApplyGravityToObstacles(); // <- Aqui

                RaceManager.Instance.collectedItem = "";
                StartCoroutine(DelayedPulseTime());
            }
            else if (RaceManager.Instance.collectedItem == "shield")
            {
                // Implementar lógica de escudo
                Debug.Log("Escudo ativado!");
                RaceManager.Instance.collectedItem = ""; // Limpa o item após uso
            }
        }



        // ========== RETORNO SUAVE PÓS-NITRO ==========
        if (!isNitroActive && RaceManager.Instance.currentSpeed > RaceManager.Instance.maxSpeed)
        {
            RaceManager.Instance.currentSpeed = Mathf.MoveTowards(
                RaceManager.Instance.currentSpeed,
                RaceManager.Instance.maxSpeed,
                RaceManager.Instance.deceleration * Time.deltaTime * 2f
            );
        }
    }

    void ApplyGravityToObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obj in obstacles)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1.5f; // Ajuste conforme o quanto você quer que eles "caiam"
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle")) return;

        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
        if (obstacle == null || !obstacle.isSolid) return;

        // Ponto mais próximo do obstáculo em relação ao jogador
        Vector2 contactPoint = collision.ClosestPoint(transform.position);
        Vector2 direction = contactPoint - (Vector2)transform.position;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            // Colisão vertical
            if (direction.y > 0 && verticalVelocity > 0)
            {
                // Tentando subir contra algo acima
                verticalVelocity *= -2;
                Debug.Log("Impedido de subir por obstáculo acima");
            }
            else if (direction.y < 0 && verticalVelocity < 0)
            {
                // Tentando descer contra algo abaixo
                verticalVelocity *= -2;
                Debug.Log("Impedido de descer por obstáculo abaixo");
            }

            // Reduz só um pouco a velocidade horizontal
            RaceManager.Instance.currentSpeed *= 1f;
        }
        else
        {
            // Colisão horizontal — bloqueia completamente o avanço
            RaceManager.Instance.currentSpeed = 0f;
            Debug.Log("Colisão frontal — velocidade zerada");
        }
    }



    IEnumerator ActivateNitro()
    {
        GameObject Turbo = GameObject.Find("Turbo");
        AudioSource audio = Turbo.GetComponent<AudioSource>();
        audio.volume = RaceManager.Instance.musicVolume * 0.5f;
        audio.Play();
        isNitroActive = true;
        RaceManager.Instance.currentSpeed += RaceManager.Instance.nitroBoost;

        yield return new WaitForSeconds(RaceManager.Instance.nitroDuration);

        isNitroActive = false;
    }

    IEnumerator DelayedPulseTime()
    {
        yield return new WaitForSeconds(2f);
        RaceManager.Instance.pulseTime = RaceManager.Instance.maxPulseTime;
    }
}
