using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class EnemyCannon : NetworkComponent
{
    public GameObject target;
    public float detectionRange = 20f;
    public int cannonBallSpeed;
    public float gravity;

    public float reloadTime;
    public bool reloading;
    public Vector3 aimOffset;

    public Rigidbody MyRig;

    public int cannonballPrefab;

    public Vector3 lastPosition;
    public Vector3 lastRotation;

    public float Threashhold;
    public float Ethreashhold;



    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer)
            {
                float distance = (this.transform.position - lastPosition).magnitude;
                if (distance > Threashhold)
                {
                    SendUpdate("POS", this.transform.position.ToString());
                    lastPosition = this.transform.position;
                }
                if ((this.transform.rotation.eulerAngles - lastRotation).magnitude > Threashhold)
                {
                    lastRotation = this.transform.rotation.eulerAngles;
                    SendUpdate("ROT", lastRotation.ToString());
                }

                if (IsDirty)
                {
                    SendUpdate("POS", lastPosition.ToString());
                    SendUpdate("ROT", lastRotation.ToString());
                    //animation

                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "POS" && IsClient)
        {
            lastPosition = NetworkCore.Vector3FromString(value);
        }
        if (flag == "ROT" && IsClient)
        {
            lastRotation = NetworkCore.Vector3FromString(value);
        }
    }

    public override void NetworkedStart()
    {
        target = GameObject.FindGameObjectWithTag("SHIP");
    }

    public IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("SHIP");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            Vector3 toTarget = target.transform.position - transform.position + aimOffset;

            Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

            float grav = Mathf.Abs(Physics.gravity.y);
            float v2 = cannonBallSpeed * cannonBallSpeed;
            float v4 = v2 * v2;
            float gx = grav * toTargetXZ.magnitude;

            float discriminant = v4 - grav * (grav * toTargetXZ.magnitude * toTargetXZ.magnitude + 2 * toTarget.y * v2);

            Quaternion targetRotation = Quaternion.identity;

            float sqrtDisc = Mathf.Sqrt(discriminant);
            float angle = Mathf.Atan((v2 - sqrtDisc) / gx);

            Vector3 flatDir = toTargetXZ.normalized;
            Quaternion yawRotation = Quaternion.LookRotation(flatDir);
            if(Mathf.Abs(angle) > 0)
            {
                targetRotation = yawRotation * Quaternion.Euler(-Mathf.Rad2Deg * angle, 0, 0);

                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                20 * Time.deltaTime);

                if (!reloading)
                {
                    FireAtPlayer();
                    StartCoroutine(reload());
                }
            }
        }
        if (IsClient)
        {
            float distance = (this.transform.position - this.lastPosition).magnitude;
            if (distance > Ethreashhold)
            {
                this.transform.position = this.lastPosition;
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, lastPosition, Time.deltaTime * 20);
            }

            this.transform.rotation = Quaternion.Euler(lastRotation);
        }

    }

    void FireAtPlayer()
    {
        GameObject tempBall = MyCore.NetCreateObject(cannonballPrefab, -1, transform.position + transform.forward * 5, Quaternion.identity);
        Rigidbody tempRB = tempBall.GetComponent<Rigidbody>();
        tempRB.gameObject.layer = gameObject.layer;
        if (tempRB != null)
        {
            tempRB.linearVelocity = transform.forward * cannonBallSpeed;
        }
        reloading = true;
    }
}
