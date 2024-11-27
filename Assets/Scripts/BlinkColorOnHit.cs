using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f; // Duration to show the damage color

    [Header("Dynamic")]
    private Renderer[] renderers; // All the renderers of this & its children
    private Color[] originalColors; // Original colors of the materials

    // Blink colors for normal and shielded damage
    private static Color blinkColorRed = Color.red; // Color to indicate normal damage
    private static Color blinkColorBlue = Color.blue; // Color to indicate shield damage

    void Awake()
    {
        // Get all Renderer components from this GameObject and its children
        renderers = GetComponentsInChildren<Renderer>(true);
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Store the original color of each renderer's material
            originalColors[i] = renderers[i].material.color;
        }
    }

    // Method to trigger the blink effect with a specific color
    public void Blink(bool hasShield)
    {
        StartCoroutine(BlinkCoroutine(hasShield ? blinkColorBlue : blinkColorRed));
    }

    private IEnumerator BlinkCoroutine(Color blinkColor)
    {
        // Set all materials to the blink color
        foreach (var renderer in renderers)
        {
            renderer.material.color = blinkColor;
        }

        // Wait for the blink duration
        yield return new WaitForSeconds(blinkDuration);

        // Revert back to the original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}
