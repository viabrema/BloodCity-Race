using UnityEngine;

public class OponentSensor : MonoBehaviour
{
    private Oponent oponent;

    void Start()
    {
        oponent = GetComponentInParent<Oponent>();
        if (oponent == null)
        {
            Debug.LogError("OponentSensor: Oponent script not found on parent!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("Obstacle") || other.CompareTag("Player")) && oponent != null)
        {
            float direction = other.transform.position.y > transform.position.y ? -1f : 1f;
            oponent.OnSensorTrigger(direction);
        }
    }
}
