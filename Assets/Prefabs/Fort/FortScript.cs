using UnityEngine;

public class FortScript : Enemy
{
    [SerializeField] private int cannonPrefab;
    [SerializeField] private GameObject[] cannonLocations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        foreach(GameObject canLoc in cannonLocations)
        {
            //MyCore.NetCreateObject(cannonPrefab, -1, canLoc.transform.position, Quaternion.identity);
        }
    }
}
