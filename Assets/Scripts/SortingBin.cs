using UnityEngine;
using TMPro;

public class SortingBin : MonoBehaviour
{
    [Header("Setup")]
    public DebrisType acceptedType;
    public float      binRadius = 1.2f;

    [Header("Label")]
    public TextMeshPro binLabel;

    private SpriteRenderer sr;
    private Color idleColor    = new Color(0.3f, 0.5f, 0.9f, 0.7f);
    private Color hoverCorrect = new Color(0.4f, 0.9f, 0.5f, 0.9f);
    private Color hoverWrong   = new Color(0.95f, 0.3f, 0.3f, 0.9f);
    private Color flashCorrect = new Color(0.3f, 0.95f, 0.5f, 1f);
    private Color flashWrong   = new Color(1f, 0.2f, 0.2f, 1f);
    private bool  isHighlighted = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = idleColor;
        if (binLabel) binLabel.text = acceptedType.ToString();
    }

    void Update()
    {
        DebrisObject dragging = null;
        foreach (DebrisObject d in FindObjectsByType<DebrisObject>(FindObjectsSortMode.None))
            if (d.isDragging) { dragging = d; break; }

        if (dragging != null && IsOverBin(dragging.transform.position))
        {
            if (!isHighlighted)
            {
                isHighlighted = true;
                if (sr) sr.color = dragging.GetDebrisType() == acceptedType ? hoverCorrect : hoverWrong;
            }
        }
        else if (isHighlighted)
        {
            isHighlighted = false;
            if (sr) sr.color = idleColor;
        }
    }

    public void ReceiveDebris(DebrisObject debris)
    {
        bool correct = debris.GetDebrisType() == acceptedType;
        FindFirstObjectByType<ScoreManager>()?.DebrisSorted(correct);

        if (correct)
        {
            StartCoroutine(Flash(flashCorrect));
            Destroy(debris.gameObject);
        }
        else
        {
            StartCoroutine(Flash(flashWrong));
            FindFirstObjectByType<GameManager>()?.WrongBin();
        }
    }

    public bool IsOverBin(Vector3 pos) =>
        Vector2.Distance(transform.position, pos) <= binRadius;

    System.Collections.IEnumerator Flash(Color c)
    {
        if (sr) sr.color = c;
        yield return new WaitForSeconds(0.25f);
        if (sr) sr.color = idleColor;
        isHighlighted = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, binRadius);
        Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, binRadius);
    }
}
