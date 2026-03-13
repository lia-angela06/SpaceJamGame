using UnityEngine;

public class CourtBackground : MonoBehaviour
{
    // Create 5 crack sprite GameObjects as children of the court.
    // Drag them into this array. They activate one-by-one as damage rises.
    public GameObject[] crackObjects;
    private int lastDamage = 0;

    void Update()
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm == null || gm.courtDamage == lastDamage) return;
        lastDamage = gm.courtDamage;
        for (int i = 0; i < crackObjects.Length; i++)
            if (crackObjects[i]) crackObjects[i].SetActive(i < lastDamage);
    }
}
