using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class EnemyCannon : NetworkComponent
{
    public float detectionRange = 20f;
    public float fireCooldown = 2f;
    public int projectilePrefabIndex = 10; //Projectile prefab
    public Transform firePoint;

    public float lastFireTime = 0f;

    public override void HandleMessage(string flag, string value)
    {
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.05f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
