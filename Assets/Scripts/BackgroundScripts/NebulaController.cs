using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaController : MonoBehaviour
{
    /// Controls nebula movement and animated color transitions using a particle system.

    public ParticleSystem nebulaParticleSystem;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float movementRange = 7f;
    private Vector3 targetPosition;
    [SerializeField] float fixedYPosition = 4f;
    [SerializeField] float fixedZPosition = -1f;
    [SerializeField] float colorChangingSpeed = 0.2f;
    private ParticleSystem.ColorOverLifetimeModule colorOverLifetime;
    private Gradient grad;
    private Color[] colors = { Color.red, Color.cyan, Color.green, Color.blue};
    private float[] colorPositions = { 0.0f, 0.33f, 0.66f, 1.0f };

    void Start()
    {
        if (nebulaParticleSystem == null)
        {
            nebulaParticleSystem = GetComponent<ParticleSystem>();
        }

        colorOverLifetime = nebulaParticleSystem.colorOverLifetime; //cached

        // Initialize the gradient
        grad = new Gradient();
        colorOverLifetime.enabled = true;

        // Initialize the target position to the current position
        targetPosition = transform.position;
        SetNewTargetPosition();
    }

    void Update()
    {
        // Move towards the target position along the Xaxis
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = new Vector3(
            Mathf.MoveTowards(currentPosition.x, targetPosition.x, movementSpeed * Time.deltaTime),
            fixedYPosition,
            fixedZPosition);

        transform.position = newPosition;

        // If the nebula is close to the target position, set a new target position
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.1f)
        {
            SetNewTargetPosition();
        }

        SetGradientColors(Time.time);
    }

    private void SetGradientColors(float time)
    {
        // Calculate the current time factor for the gradient
        float t = Mathf.Repeat(Time.time * colorChangingSpeed, 1.0f);

        // Define the gradient color keys
        GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            float position = Mathf.Repeat(colorPositions[i] + t, 1.0f);
            colorKeys[i] = new GradientColorKey(colors[i], position);
        }

        // Define the constant alpha keys
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 1.0f)
        };

        // Set the gradient with animated colors and constant alpha
        grad.SetKeys(colorKeys, alphaKeys);
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);
    }
    
    void SetNewTargetPosition()
    {
        // Set a new random target position within the movement range on the x-axis
        targetPosition = new Vector3(
            Random.Range(-movementRange, movementRange),
            fixedYPosition,
            fixedZPosition);

    }
}
