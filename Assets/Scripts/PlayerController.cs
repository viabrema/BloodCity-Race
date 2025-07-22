using UnityEngine;
using System.Collections;

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
        HandleInput();
        RaceManager.Instance.distanceTraveled += RaceManager.Instance.currentSpeed * Time.deltaTime;

        // Ajuste de volume e pitch do som de aceleração
        float speedPercent = RaceManager.Instance.currentSpeed / RaceManager.Instance.maxSpeed;
        speedPercent = Mathf.Clamp01(speedPercent);
        // accelerationSound.volume = 0.5f + (speedPercent * 0.5f);
        accelerationSound.pitch = 0.5f + speedPercent * 5f;

        // Inclinação visual do carro ao subir ou descer
        float tiltAngle = verticalVelocity * 5f; // valor reduzido para suavidade
        Quaternion targetRotation = Quaternion.Euler(0, 0, tiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // Efeito de nitro
        if (RaceManager.Instance.currentSpeed > RaceManager.Instance.maxSpeed)
        {
            nitroEffect.Play();
        }
        else
        {
            nitroEffect.Stop();
        }

        // --- Se corrida ainda não começou, trava a velocidade ---
        if (!RaceManager.Instance.startedRace)
        {
            RaceManager.Instance.currentSpeed = 0f;
        }

        // ========== CONTROLE DE INÍCIO E REINÍCIO ==========
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RaceManager.Instance.startedRace = true;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RaceManager.Instance.ResetRace();
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
            else if (RaceManager.Instance.collectedItem == "shot")
            {
                // Implementar lógica de tiro
                Debug.Log("Disparando tiro!");
                RaceManager.Instance.collectedItem = ""; // Limpa o item após uso
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

    IEnumerator ActivateNitro()
    {
        GameObject Turbo = GameObject.Find("Turbo");
        Turbo.GetComponent<AudioSource>().Play();
        isNitroActive = true;
        RaceManager.Instance.currentSpeed += RaceManager.Instance.nitroBoost;

        yield return new WaitForSeconds(RaceManager.Instance.nitroDuration);

        isNitroActive = false;
    }
}
