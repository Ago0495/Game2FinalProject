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
    private bool faceShip;

    //animation
    private bool Forward;
    private bool Left;
    private bool Right;
    private bool Tight;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "Left" && IsClient)
        {
            MyAnime.SetInteger("Turning", -5);
        }
        if (flag == "Right" && IsClient)
        {
            MyAnime.SetInteger("Turning", 5);
        }
        if (flag == "Forward" && IsClient)
        {
            MyAnime.SetInteger("Turning", 0);
        }
    }

    public void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if (IsServer)
            {
                if (IsDirty)
                {
                    if (Right)
                    {
                        //right animation
                        SendUpdate("Right", "404");
                    }
                    else if (Left)
                    {
                        //Left animation
                        SendUpdate("Left", "404");
                    }
                    else if (Forward)
                    {
                        //forward animation
                        SendUpdate("Forward", "404");
                    }
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
           
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        sharkArea = GameObject.FindGameObjectWithTag("SArea");
        MyAnime = GetComponent<Animator>();
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, -90f, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 localDirection = transform.InverseTransformDirection(-direction);
        if (localDirection.x < 0 && !Right)
        {
            //right animation
            MyAnime.SetInteger("Turning", 5);
            SendUpdate("Right", "404");
            Right = true;
            Left = false;
            Forward = false;
        }
        else if (localDirection.x > 0 && !Left)
        {
            //Left animation
            MyAnime.SetInteger("Turning", -5);
            SendUpdate("Left", "404");
            Right = false;
            Left = true;
            Forward = false;
        }
        else if (localDirection.x == 0 && !Forward)
        {
            //forward animation
            MyAnime.SetInteger("Turning", 0);
            SendUpdate("Forward", "404");
            Right = false;
            Left = false;
            Forward = true;
        }
    }

    //TO-Do
    //Add delay before charge
    //add rotation
    public IEnumerator Charge()
    {
        MyRig.linearVelocity = Vector3.zero;
        while (faceShip)
        {
            yield return new WaitForSeconds(0.5f);
        }
        MyRig.linearVelocity = (target.transform.position - transform.position).normalized * moveSpeed;
        yield return new WaitForSeconds(3);
        charging = false;
        transition = true;
        moveSpeed -= 30;
    }

    private void MoveToClosest(Vector3 direction)
    {
        Vector3 centerPoint = direction;
        Vector3 toShark = transform.position - centerPoint;
        Vector3 closestOrbitPoint = centerPoint + toShark.normalized * radius;

        Vector3 moveDirection = (closestOrbitPoint - transform.position).normalized;
        RotateTowards(moveDirection);
        moveDirection *= moveSpeed;
        MyRig.linearVelocity = moveDirection;
        
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

        Vector3 targetVelocity = (orbitPosition - transform.position).normalized;
        RotateTowards(targetVelocity);
        MyRig.linearVelocity = targetVelocity * moveSpeed;

        
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

                if (Random.Range(0, 650) == 0)
                {
                    charging = true;
                    faceShip = true;

                    StartCoroutine(Charge());
                }
            }
            else if (transition && !charging)
            {
                MoveToClosest(sharkArea.transform.position);
            }
            else if (faceShip)
            {
                Vector3 direction = (transform.position - target.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, 90f, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (rotationSpeed + 20) * Time.deltaTime);
                if (targetRotation.y < 0 && !Right)
                {
                    //right animation
                    MyAnime.SetInteger("Turning", 5);
                    SendUpdate("Right", "404");
                    Right = true;
                    Left = false;
                    Forward = false;
                }
                else if (targetRotation.y > 0 && !Left)
                {
                    //Left animation
                    MyAnime.SetInteger("Turning", -5);
                    SendUpdate("Left", "404");
                    Right = false;
                    Left = true;
                    Forward = false;
                }
                else if (targetRotation.y == 0 && !Forward)
                {
                    //forward animation
                    MyAnime.SetInteger("Turning", 0);
                    SendUpdate("Forward", "404");
                    Right = false;
                    Left = false;
                    Forward = true;
                }
                if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
                {
                    faceShip = false;
                }
            }
            else if (!charging && !faceShip)
            {
                Circle(sharkArea.transform.position);
            }
        }
    }
}
