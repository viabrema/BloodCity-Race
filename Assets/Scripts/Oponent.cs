using UnityEngine;

public class Oponent : MonoBehaviour
{
    public int indexInRaceManager = 0;

    [Header("Velocidade")]
    public float maxSpeed = 52f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    public float slowDownFactor = 0.8f;
    public float distanceTraveled = 0f;

    [Header("HUD")]
    public Color pointColor = Color.red;

    [Header("Evitar Obstáculos")]
    public float avoidSpeed = 3f;
    public float avoidDuration = 0.5f;
    public float verticalInertia = 6f;
    public float minY = -3.5f;
    public float maxY = 3.5f;
    public float idleReturnDelay = 1.5f;
    public float returnSpeed = 2f;
    public float centerY = 0f;

    [HideInInspector] public float currentSpeed = 0f;

    private float currentMaxSpeed;
    private float avoidTimer = 0f;
    private float verticalDirection = 0f;
    private float currentVerticalVelocity = 0f;
    private float timeSinceLastObstacle = 0f;

    private bool avoidingObstacle = false;
    private bool isSlowingDown = false;

    // Colisão com obstáculo
    private bool isTouchingObstacle = false;
    private float obstacleAvoidDirection = 0f;
    private Collider2D currentObstacle = null;
    private float currentObstacleSlowFactor = 1f;

    void Start()
    {
        currentMaxSpeed = 0f;
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;

        if (!RaceManager.Instance.startedRace)
        {
            currentSpeed = 0f;
            return;
        }

        HandleHorizontalSpeed();
        UpdateRaceDistance();
        MoveHorizontal();
        MoveVertical();
        ApplyTilt();
    }

    private void HandleHorizontalSpeed()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        currentMaxSpeed = maxSpeed;

        // Ajusta a velocidade máxima com base na posição do oponente na tela
        if (viewportPos.x > 0.7f)
            currentMaxSpeed *= 0.9f;
        else if (viewportPos.x < 0f)
            currentMaxSpeed *= 1.1f;

        float targetSpeed = currentMaxSpeed;

        if (isTouchingObstacle)
        {
            targetSpeed *= currentObstacleSlowFactor;

            if (!avoidingObstacle)
            {
                avoidingObstacle = true;
                verticalDirection = obstacleAvoidDirection;
                avoidTimer = avoidDuration;
            }
        }
        else if (isSlowingDown)
        {
            targetSpeed *= slowDownFactor;
        }

        float speedChange = (targetSpeed < currentSpeed) ? deceleration : acceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedChange * Time.deltaTime);
    }

    private void UpdateRaceDistance()
    {
        distanceTraveled += currentSpeed * Time.deltaTime;

    }

    private void MoveHorizontal()
    {
        float relativeSpeed = RaceManager.Instance.currentSpeed - currentSpeed;
        transform.Translate(Vector2.left * relativeSpeed * Time.deltaTime);
    }



    private void MoveVertical()
    {
        float targetVertical = 0f;

        if (avoidingObstacle)
        {
            targetVertical = verticalDirection * avoidSpeed;
            avoidTimer -= Time.deltaTime;

            if (avoidTimer <= 0f)
            {
                avoidingObstacle = false;
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
        float newY = Mathf.Clamp(transform.position.y + currentVerticalVelocity * Time.deltaTime, minY, maxY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void ApplyTilt()
    {
        float tiltAmount = currentVerticalVelocity * 1.5f;
        float maxTilt = 15f;
        tiltAmount = Mathf.Clamp(tiltAmount, -maxTilt, maxTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, tiltAmount), Time.deltaTime * 5f);
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

    public void SetSlowedByObstacle(bool value, float direction, Collider2D obstacle)
    {
        isTouchingObstacle = value;
        obstacleAvoidDirection = direction;
        currentObstacle = obstacle;

        if (value && obstacle != null)
        {
            Obstacle obsScript = obstacle.GetComponent<Obstacle>();
            currentObstacleSlowFactor = obsScript != null ? obsScript.oponentSlowDownFactor : 0.5f;
        }
        else
        {
            currentObstacleSlowFactor = 1f;
        }
    }
}
