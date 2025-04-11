using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

public class MegScript : Enemy
{
    [SerializeField] private GameObject sharkArea;
    [SerializeField] private float radius;
    [SerializeField] private float circleTime;

    private float angle = 0f;
    public bool charging;

    public override void HandleMessage(string flag, string value)
    {

    }

    public void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        sharkArea = GameObject.FindGameObjectWithTag("SArea");
    }

    public IEnumerator wait()
    {
        yield return new WaitForSeconds(3);
        charging = false;
        transition = true;
        moveSpeed -= 30;
    }

    void RotateTowards(Vector3 direction)
    {
        Vector3 desiredForward = direction.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(desiredForward);

        MyRig.angularVelocity = Vector3.up * Mathf.Sign(Vector3.SignedAngle(transform.forward, desiredForward, Vector3.up)) * 2;
    }
    //TO-Do
    //Add delay before charge
    //add rotation
    public void Charge()
    {
        if (IsServer)
        {
            MyRig.linearVelocity = Vector3.zero;
            MyRig.angularVelocity = Vector3.zero;
            transform.rotation = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
            //wait
            moveSpeed += 30; 
            MyRig.linearVelocity = (target.transform.position - transform.position).normalized * moveSpeed;
            StartCoroutine(wait());
        }
        
    }

    private void MoveToClosest(Vector3 direction)
    {
        Vector3 centerPoint = direction;
        Vector3 toShark = transform.position - centerPoint;
        Vector3 closestOrbitPoint = centerPoint + toShark.normalized * radius;

        Vector3 moveDirection = (closestOrbitPoint - transform.position).normalized * moveSpeed;
        MyRig.linearVelocity = moveDirection;
        RotateTowards(moveDirection);
        if ((Vector3.Distance(transform.position, closestOrbitPoint) < 10.0f))
        {
            transition = false;
        }
    }

    private void Circle(Vector3 direction)
    {
        angle += (moveSpeed / radius) * Time.deltaTime;

        Vector3 centerPoint = direction;

        Vector3 orbitPosition = centerPoint + new Vector3(
            Mathf.Cos(angle) * radius,
            0,
            Mathf.Sin(angle) * radius
        );

        Vector3 targetVelocity = (orbitPosition - transform.position).normalized * moveSpeed;

        MyRig.linearVelocity = targetVelocity;

        RotateTowards(targetVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (attacking && !charging && transition)
            {
                MoveToClosest(target.transform.position);
            }
            else if (attacking && !charging && !transition)
            {
                Circle(target.transform.position);

                if (Random.Range(0, 700) == 0)
                {
                    charging = true;
                    Charge();
                }
            }
            else if (transition && !charging)
            {
                MoveToClosest(sharkArea.transform.position);
            }
            else if (!charging)
            {
                Circle(sharkArea.transform.position);

                //Vector3 lookPosition = centerPoint - transform.position;
                //lookPosition.y = 0;
                //transform.rotation = Quaternion.LookRotation(lookPosition);

            }
        }
    }
}
