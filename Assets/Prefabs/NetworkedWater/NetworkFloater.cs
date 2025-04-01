using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class NetworkFloater : NetworkComponent
{
    //sync vals

    //non-sync vals
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float depthBeforeSubmerged = 1f;
    [SerializeField] private float displacementAmount = 3f;
    [SerializeField] private float waterDrag = 0.99f;
    [SerializeField] private float waterAngularDrag = 0.5f;
    public int floaterCount = 0;

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            rb.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
            float waveHeight = NetworkWaveMaster.instance.GetWaveHeight(transform.position);
            if (transform.position.y < waveHeight)
            {
                float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
                rb.AddForce(displacementMultiplier * -rb.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rb.AddTorque(displacementMultiplier * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);

            }
        }
    }
}
