using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class NetworkRigidBody : NetworkComponent
{
    //sync vars
    public Vector3 lastPosition;
    public Vector3 lastRotation;
    public Vector3 lastVelocity;
    public Vector3 lastAngularVelocity;

    //non-sync vars
    public float threshold;
    public float eThreshold;
    public bool useAdjustVel;
    public Vector3 adjustVelocity;
    public Rigidbody myRig;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient && flag == "POS")
        {
            lastPosition = NetworkCore.Vector3FromString(value);
            if (useAdjustVel)
            {
                adjustVelocity = lastPosition - myRig.position;
            }
            if ((lastPosition - myRig.position).magnitude > eThreshold)
            {
                myRig.position = lastPosition;
                adjustVelocity = Vector3.zero;
            }
        }
        if (IsClient && flag == "VEL")
        {
            lastVelocity = NetworkCore.Vector3FromString(value);
            if (lastVelocity.magnitude < 0.01f)
            {
                adjustVelocity = Vector3.zero;
            }

        }
        if (IsClient && flag == "ROT")
        {
            lastRotation = NetworkCore.Vector3FromString(value);
            if ((lastRotation - myRig.rotation.eulerAngles).magnitude > eThreshold)
            {
                myRig.rotation = Quaternion.Euler(lastRotation);
            }
        }
        if (IsClient && flag == "ANG")
        {
            lastAngularVelocity = NetworkCore.Vector3FromString(value);
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {
                if ((myRig.position - lastPosition).magnitude > threshold)
                {
                    SendUpdate("POS", myRig.position.ToString());
                    lastPosition = myRig.position;
                }

                if ((myRig.rotation.eulerAngles - lastRotation).magnitude > threshold)
                {
                    SendUpdate("ROT", myRig.rotation.eulerAngles.ToString());
                    lastRotation = myRig.position;
                }

                if ((lastVelocity - myRig.linearVelocity).magnitude > threshold)
                {
                    SendUpdate("VEL", myRig.linearVelocity.ToString());
                    lastVelocity = myRig.linearVelocity;
                }

                if ((myRig.angularVelocity - lastAngularVelocity).magnitude > threshold)
                {
                    SendUpdate("ANG", myRig.angularVelocity.ToString());
                    lastAngularVelocity = myRig.angularVelocity;
                }

                if (IsDirty)
                {
                    SendUpdate("POS", myRig.position.ToString());
                    SendUpdate("VEL", myRig.linearVelocity.ToString());
                    SendUpdate("ROT", myRig.rotation.ToString());
                    SendUpdate("ANG", myRig.angularVelocity.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            myRig.linearVelocity = lastVelocity;
            if (useAdjustVel)
            {
                myRig.linearVelocity += adjustVelocity;
                myRig.angularVelocity = lastAngularVelocity;
            }
        }
    }
}
