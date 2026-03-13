using UnityEngine;

public class DebugClick : MonoBehaviour
{
    void OnMouseDown() => Debug.Log("CLICKED: " + gameObject.name);
}
