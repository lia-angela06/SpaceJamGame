using UnityEngine;

public enum DebrisType { Rock, Metal, Crystal, Junk }

public class DebrisObject : MonoBehaviour
{
    [Header("Type")]
    public DebrisType debrisType;

    [Header("Fall Settings")]
    public float fallSpeed   = 2.5f;
    public float rotateSpeed = 45f;

    [Header("Drag State")]
    public bool isDragging = false;
    private Vector3 dragOffset;
    private Camera mainCam;

    private SpriteRenderer sr;
    private float swayOffset;
    private Vector3 originalScale;

    void Start()
    {
        mainCam       = Camera.main;
        sr            = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        SetColorByType();
    }

    void Update()
    {
        if (!GameManager.gameActive) return;

        if (isDragging)
        {
            Vector3 mp = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mp.z = 0f;
            transform.position = mp + dragOffset;
        }
        else
        {
            // Fall straight down
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Gentle side sway instead of spinning
            float sway = Mathf.Sin(Time.time * 1.2f + swayOffset) * 0.3f;
            transform.position = new Vector3(
                transform.position.x + sway * Time.deltaTime,
                transform.position.y,
                0f
            );

            // Slight tilt that follows the sway direction
            float tilt = Mathf.Sin(Time.time * 1.2f + swayOffset) * 12f;
            transform.rotation = Quaternion.Euler(0f, 0f, tilt);

            if (transform.position.y < GameManager.courtFloorY)
            {
                FindFirstObjectByType<GameManager>()?.DebrisHitCourt(this);
                Destroy(gameObject);
            }
        }
    }

    void OnMouseDown()
    {
        if (!GameManager.gameActive) return;
        isDragging = true;
        Vector3 mp = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mp.z = 0f;
        dragOffset = transform.position - mp;
        transform.localScale = originalScale * 1.2f;
        if (sr) sr.sortingOrder = 10;
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.localScale = originalScale;
        if (sr) sr.sortingOrder = 1;

        foreach (SortingBin bin in FindObjectsByType<SortingBin>(FindObjectsSortMode.None))
        {
            if (bin.IsOverBin(transform.position))
            {
                bin.ReceiveDebris(this);
                return;
            }
        }
    }

    public DebrisType GetDebrisType() => debrisType;

    void SetColorByType()
    {
        if (!sr) return;
        switch (debrisType)
        {
            case DebrisType.Rock:    sr.color = new Color(0.6f, 0.55f, 0.5f);  break;
            case DebrisType.Metal:   sr.color = new Color(0.5f, 0.75f, 0.9f);  break;
            case DebrisType.Crystal: sr.color = new Color(0.8f, 0.4f,  0.9f);  break;
            case DebrisType.Junk:    sr.color = new Color(0.9f, 0.75f, 0.2f);  break;
        }
    }
}
