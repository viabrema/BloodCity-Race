using UnityEngine;

public class Oponent : MonoBehaviour
{
    public int indexInRaceManager = 0;

    public float maxSpeed = 52f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float slowDownFactor = 0.8f;

    public float avoidSpeed = 3f;
    public float avoidDuration = 0.5f;
    public float verticalInertia = 6f;
    public float minY = -3.5f;
    public float maxY = 3.5f;
    public float idleReturnDelay = 1.5f;
    public float returnSpeed = 2f;
    public float centerY = 0f;

    private float currentMaxSpeed;
    private bool avoidingObstacle = false;
    private float avoidTimer = 0f;
    private float verticalDirection = 0f;
    private float currentVerticalVelocity = 0f;
    private float timeSinceLastObstacle = 0f;

    public float currentSpeed = 0f;
    private bool isSlowingDown = false;

    void Start()
    {
        currentMaxSpeed = maxSpeed;
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        // Ajustar currentMaxSpeed baseado na posição em relação à tela
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x > 1f)
        {
            currentMaxSpeed = maxSpeed * 0.9f; // na frente e fora da tela → reduz
        }
        else if (viewportPos.x < 0f)
        {
            currentMaxSpeed = maxSpeed * 1.1f; // atrás e fora da tela → acelera
        }
        else
        {
            currentMaxSpeed = maxSpeed;
        }

        // Atualizar velocidade com aceleração/desaceleração
        float targetSpeed = currentMaxSpeed;

        if (isSlowingDown)
        {
            targetSpeed *= slowDownFactor;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, currentMaxSpeed, acceleration * Time.deltaTime);
        }

        // Atualizar distância percorrida
        if (indexInRaceManager < RaceManager.Instance.distancesTraveledOponents.Length)
        {
            RaceManager.Instance.distancesTraveledOponents[indexInRaceManager] += currentSpeed * Time.deltaTime;
        }

        // Movimento horizontal relativo à diferença de velocidade
        float relativeSpeed = RaceManager.Instance.currentSpeed - currentSpeed;
        transform.Translate(Vector2.left * relativeSpeed * Time.deltaTime);

        // ===== Movimento Vertical =====
        float targetVertical = 0f;

        if (avoidingObstacle)
        {
            targetVertical = verticalDirection * avoidSpeed;
            avoidTimer -= Time.deltaTime;
            if (avoidTimer <= 0f)
            {
                avoidingObstacle = false;
                timeSinceLastObstacle = 0f;
                isSlowingDown = false;
            }
        }
        else
        {
            timeSinceLastObstacle += Time.deltaTime;

            if (timeSinceLastObstacle >= idleReturnDelay)
            {
                float directionToCenter = Mathf.Sign(centerY - transform.position.y);
                targetVertical = directionToCenter * returnSpeed;
            }
        }

        currentVerticalVelocity = Mathf.Lerp(currentVerticalVelocity, targetVertical, Time.deltaTime * verticalInertia);
        float newY = transform.position.y + currentVerticalVelocity * Time.deltaTime;
        newY = Mathf.Clamp(newY, minY, maxY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void OnSensorTrigger(float direction)
    {
        if (!avoidingObstacle)
        {
            verticalDirection = direction;
            avoidingObstacle = true;
            avoidTimer = avoidDuration;
            isSlowingDown = true;
        }

        timeSinceLastObstacle = 0f;
    }
}
