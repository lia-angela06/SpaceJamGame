using UnityEngine;
using System.Collections;

public class EnemyCharacter : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public float targetY   = -4.5f;

    public bool isZapped = false;

    private SpriteRenderer sr;
    private bool reachedCourt = false;
    private float wobbleTimer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = new Color(0.6f, 0.2f, 0.9f);
    }

    void Update()
    {
        if (!GameManager.gameActive || isZapped || reachedCourt) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(transform.position.x, targetY, 0f),
            moveSpeed * Time.deltaTime
        );

        wobbleTimer += Time.deltaTime * 5f;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(wobbleTimer) * 8f);

        if (Mathf.Abs(transform.position.y - targetY) < 0.1f)
        {
            reachedCourt = true;
            FindFirstObjectByType<GameManager>().EnemyReachedCourt(this);
            Destroy(gameObject, 0.15f);
        }
    }

    void OnMouseDown()
    {
        if (!GameManager.gameActive || isZapped) return;
        Zap();
    }

    public void Zap()
    {
        isZapped = true;
        if (sr) sr.color = new Color(1f, 0.9f, 0.2f);
        transform.rotation = Quaternion.identity;
        FindFirstObjectByType<ScoreManager>().EnemyZapped();
        StartCoroutine(ZapAnim());
    }

    IEnumerator ZapAnim()
    {
        Vector3 baseScale = transform.localScale;
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            transform.localScale = baseScale * Mathf.Lerp(1.3f, 0f, t / 0.35f);
            yield return null;
        }
        Destroy(gameObject);
    }
}
