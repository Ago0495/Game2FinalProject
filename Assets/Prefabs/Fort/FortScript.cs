using UnityEngine;
using System.Collections;

public class FortScript : Enemy
{
    [SerializeField] private int cannonPrefab;
    [SerializeField] private GameObject[] cannonLocations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void NetworkedStart()
    {
        base.Start();
    }

    public void HandleMessage(string flag, string value)
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }
}
