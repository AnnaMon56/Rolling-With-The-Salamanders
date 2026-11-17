using UnityEngine;
using System.Collections;

public class PlayerFlash : MonoBehaviour
{
    public float flashDuration = 1f;
    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        // set all renderers red
        foreach (Renderer r in renderers)
            r.material.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        // restore original colours
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}
