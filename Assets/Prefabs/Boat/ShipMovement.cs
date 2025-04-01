using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class ShipMovement : NetworkComponent
{
    [SerializeField] Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        rb.isKinematic = true;
        while (!IsConnected)
        {
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
        rb.isKinematic = false;
    }

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }
}
