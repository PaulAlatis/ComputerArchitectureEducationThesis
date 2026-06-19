using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f; // Duration of the shake effect
    [SerializeField] private float shakeMagnitude = 0.5f; // Magnitude of the shake effect

    Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position; // Store the initial position of the camera
    }

    public void Play()
    {
        StartCoroutine(ShakeCamera());
    }

    IEnumerator ShakeCamera()
    {
        float timeElapsed = 0.0f;

        while (timeElapsed < shakeDuration)
        {
            
            transform.position = initialPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude; // Randomly move the camera within a sphere

            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame(); // Wait for the next frame
        }

        transform.position = initialPosition; // Reset to the original position after shaking
    }
}
