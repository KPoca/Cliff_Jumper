using UnityEngine;
using System.Collections;

public class FlashColor : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color originalColor;

    [SerializeField] Color color = Color.red;
    [SerializeField] float flashDuration = 0.2f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void FlashRed()
    {
        StartCoroutine(Flash(color, flashDuration));
    }

    IEnumerator Flash(Color flashColor, float duration)
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }
}
