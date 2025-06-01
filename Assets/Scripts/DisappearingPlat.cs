using System.Collections;
using UnityEngine;

public class DisappearingPlat : MonoBehaviour
{
    public string playerTag = "Player";
    public float timeToStartFading = 0.3f;
    public float fadeDuration = 1f;
    public float respawnDelay = 3f;

    private bool playerOnTop = false;
    private bool isFadingOrHidden = false;
    private float timer = 0f;
    private Renderer platformRenderer;
    private Collider platformCollider;
    private Color originalColor;
    private Material materialInstance;

    void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();

        if (platformRenderer != null && platformCollider != null)
        {
            materialInstance = new Material(platformRenderer.material); 
            platformRenderer.material = materialInstance; 
            originalColor = materialInstance.color;
        }
        else
        {
            Debug.LogError("DisappearingPlat script requires a Renderer and a Collider component on the GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        if (playerOnTop && !isFadingOrHidden)
        {
            timer += Time.deltaTime;
            if (timer >= timeToStartFading)
            {
                StartCoroutine(FadeOutAndReappear());
            }
        }
        else if (!playerOnTop && !isFadingOrHidden)
        {
            timer = 0f; // reset timer if player steps off
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag) && !isFadingOrHidden)
        {
            playerOnTop = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(playerTag))
        {
            playerOnTop = false;
        }
    }

    IEnumerator FadeOutAndReappear()
    {
        isFadingOrHidden = true;
        playerOnTop = false;
        timer = 0f;

        float elapsedTime = 0f;
        Color currentColor = materialInstance.color;

        materialInstance.SetOverrideTag("RenderType", "Transparent");
        materialInstance.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        materialInstance.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        materialInstance.SetInt("_ZWrite", 0);
        materialInstance.DisableKeyword("_ALPHATEST_ON");
        materialInstance.EnableKeyword("_ALPHABLEND_ON");
        materialInstance.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        materialInstance.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            currentColor.a = alpha;
            materialInstance.color = currentColor;
            yield return null;
        }

        currentColor.a = 0f;
        materialInstance.color = currentColor;
        platformRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);


        materialInstance.SetOverrideTag("RenderType", "Opaque");
        materialInstance.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        materialInstance.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        materialInstance.SetInt("_ZWrite", 1);
        materialInstance.DisableKeyword("_ALPHABLEND_ON");
        materialInstance.renderQueue = -1; 

        materialInstance.color = originalColor;
        platformRenderer.enabled = true;
        platformCollider.enabled = true;

        isFadingOrHidden = false;
    }
}