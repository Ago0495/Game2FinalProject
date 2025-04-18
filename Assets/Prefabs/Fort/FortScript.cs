using UnityEngine;
using System.Collections;

public class FortScript : Enemy
{
    [SerializeField] private int cannonPrefab;
    [SerializeField] private GameObject[] cannons;
    public int coinChestPrefab;

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

    public override void OnDamageTaken(float damageTaken)
    {
        Debug.Log("For took " + damageTaken + " Damage");
    }

    public override void OnDeath()
    {
        if (IsServer)
        {
            for (int i = 0; i < cannons.Length; i++)
            {
                MyCore.NetDestroyObject(cannons[i].GetComponent<EnemyCannon>().NetId);
            }
            MyCore.NetCreateObject(coinChestPrefab, -1, transform.position + Vector3.up * 5, Quaternion.identity);
        }
    }
}
