using UnityEngine;

public class StarBackground : MonoBehaviour
{
    public int   starCount   = 100;
    public float scrollSpeed = 0.3f;
    public float xRange      = 10f;
    public float yRange      = 7f;

    private Transform[] stars;
    private float[] speeds, xs, ys;

    void Start()
    {
        stars = new Transform[starCount];
        speeds = new float[starCount];
        xs = new float[starCount];
        ys = new float[starCount];

        for (int i = 0; i < starCount; i++)
        {
            var go = new GameObject("Star_" + i);
            go.transform.parent = transform;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = MakeCircle(8);
            sr.sortingOrder = -10;

            float spd = Random.Range(0.2f, 1.2f);
            speeds[i] = spd;
            go.transform.localScale = Vector3.one * Mathf.Lerp(0.02f, 0.08f, spd / 1.2f);
            sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.2f, 0.9f, spd / 1.2f));

            xs[i] = Random.Range(-xRange, xRange);
            ys[i] = Random.Range(-yRange, yRange);
            go.transform.position = new Vector3(xs[i], ys[i], 0f);
            stars[i] = go.transform;
        }
    }

    void Update()
    {
        for (int i = 0; i < starCount; i++)
        {
            ys[i] -= speeds[i] * scrollSpeed * Time.deltaTime;
            if (ys[i] < -yRange) { ys[i] = yRange; xs[i] = Random.Range(-xRange, xRange); }
            stars[i].position = new Vector3(xs[i], ys[i], 0f);
        }
    }

    Sprite MakeCircle(int res)
    {
        var tex = new Texture2D(res, res);
        var c   = new Vector2(res / 2f, res / 2f);
        for (int x = 0; x < res; x++)
            for (int y = 0; y < res; y++)
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), c) / (res / 2f))));
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), Vector2.one * .5f);
    }
}
