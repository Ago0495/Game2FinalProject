using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class ShipMovement : NetworkComponent
{
    [SerializeField] Rigidbody rb;
    GameMaster gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.useGravity= false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        rb.useGravity = false;
        while (!IsConnected)
        {
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
        rb.useGravity = true;
        if (IsServer)
        {

            while (IsServer)
            {
                if (gm.GetGameStarted())
                {
                    //rb.AddForce(transform.forward * 5000);
                }
                yield return new WaitForSeconds(MyCore.MasterTimer);
            }
        }
    }

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        gm = FindAnyObjectByType<GameMaster>();
    }
}
