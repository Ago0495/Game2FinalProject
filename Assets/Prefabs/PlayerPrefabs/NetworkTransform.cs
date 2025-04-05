using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.AI;


public class NetworkTransform : NetworkComponent
{
    //synch vars
    Vector3 lastPosition;
    Vector3 lastRotation;

    //non-sync vars
    public float Threshold;
    public float Ethreshold;
    public float Speed;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "POS" && IsClient)
        {
            lastPosition = Vector3.Lerp(lastPosition, NetworkCore.Vector3FromString(value), 5);
        }
        if (flag == "ROT" && IsClient)
        {
            lastRotation = Vector3.Lerp(lastRotation, NetworkCore.Vector3FromString(value), 5);
        }
    }

    public override void NetworkedStart()
    {
        //if (IsServer)
        //{
        //    this.transform.position = new Vector3(-10, 0, 0);
        //    lastPosition = transform.position;
        //    lastRotation = transform.rotation.eulerAngles;
        //}
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer)
            {
                float distance = (this.transform.position - lastPosition).magnitude;
                if (distance > Threshold)
                {
                    SendUpdate("POS", this.transform.position.ToString());
                    lastPosition = this.transform.position;
                }
                if ((this.transform.rotation.eulerAngles - lastRotation).magnitude > Threshold)
                {
                    lastRotation = this.transform.rotation.eulerAngles;
                    SendUpdate("ROT", lastRotation.ToString()); 
                }

                if (IsDirty)
                {
                    SendUpdate("POS", lastPosition.ToString());
                    SendUpdate("ROT", lastRotation.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(Threshold);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            float distance = (this.transform.position - this.lastPosition).magnitude;
            if (distance > Threshold)
            {
                this.transform.position = this.lastPosition;
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, lastPosition, Time.deltaTime * Speed);
            }
            this.transform.rotation = Quaternion.Euler(lastRotation);
        }
    }
}