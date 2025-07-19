using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool isNitroActive = false;
    private float verticalVelocity = 0f;


    void Update()
    {
        HandleInput();
        RaceManager.Instance.distanceTraveled += RaceManager.Instance.currentSpeed * Time.deltaTime;
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
        transform.Translate(Vector2.up * verticalVelocity * Time.deltaTime);

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

        // ========== NITRO ==========
        if (Input.GetKeyDown(KeyCode.Space) && !isNitroActive)
        {
            StartCoroutine(ActivateNitro());
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
        isNitroActive = true;
        RaceManager.Instance.currentSpeed += RaceManager.Instance.nitroBoost;

        yield return new WaitForSeconds(RaceManager.Instance.nitroDuration);

        isNitroActive = false;
    }
}
