using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.EventSystems;

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

    Transform armature;

    public int DefUpgradePrefab;


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
        if(flag == "Battle")
        {
            musicMaster.sharkAttack();
            musicMaster.shark = true;
        }
        if (flag == "ENDBATTLE")
        {
            musicMaster.shark = false;
            musicMaster.background();
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
        armature = transform.GetChild(0);
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, -90f, 0);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //if (MyRig.linearVelocity != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(MyRig.linearVelocity.normalized);
        //}

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
        yield return new WaitForSeconds(2f);
        //MyRig.linearVelocity = (target.transform.position - transform.position).normalized * moveSpeed * 2;
        MyRig.AddForce((target.transform.position - transform.position).normalized * moveSpeed * 5, ForceMode.VelocityChange);
        yield return new WaitForSeconds(3);
        charging = false;
        transition = true;
        //moveSpeed -= 30;
    }

    private void MoveToClosest(Vector3 direction)
    {
        Vector3 centerPoint = direction;
        Vector3 toShark = transform.position - centerPoint;
        Vector3 closestOrbitPoint = centerPoint + toShark.normalized * radius;

        Vector3 moveDirection = (closestOrbitPoint - transform.position).normalized;
        RotateTowards(moveDirection);
        moveDirection *= moveSpeed;
        //MyRig.linearVelocity = moveDirection;
        MyRig.AddForce(moveDirection * Time.deltaTime, ForceMode.VelocityChange);

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
        //MyRig.linearVelocity = targetVelocity * moveSpeed;
        MyRig.AddForce(targetVelocity * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);


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
            //else if (faceShip)
            //{
            //    Vector3 direction = (transform.position - target.transform.position).normalized;
            //    Quaternion targetRotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(0, 90f, 0);
            //    //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, (rotationSpeed + 20) * Time.deltaTime);
            //    //if (MyRig.linearVelocity != Vector3.zero)
            //    //{
            //    //    transform.rotation = Quaternion.LookRotation(MyRig.linearVelocity.normalized);
            //    //}

            //    if (targetRotation.y < 0 && !Right)
            //    {
            //        //right animation
            //        MyAnime.SetInteger("Turning", 5);
            //        SendUpdate("Right", "404");
            //        Right = true;
            //        Left = false;
            //        Forward = false;
            //    }
            //    else if (targetRotation.y > 0 && !Left)
            //    {
            //        //Left animation
            //        MyAnime.SetInteger("Turning", -5);
            //        SendUpdate("Left", "404");
            //        Right = false;
            //        Left = true;
            //        Forward = false;
            //    }
            //    else if (targetRotation.y == 0 && !Forward)
            //    {
            //        //forward animation
            //        MyAnime.SetInteger("Turning", 0);
            //        SendUpdate("Forward", "404");
            //        Right = false;
            //        Left = false;
            //        Forward = true;
            //    }
            //    if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
            //    {
            //        faceShip = false;
            //    }
            //}
            else if (!charging/* && !faceShip*/)
            {
                Circle(sharkArea.transform.position);
            }
        }

        if (MyRig.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(MyRig.linearVelocity.normalized);
        }

        //rotates armature to face forward
        Vector3 tempb = new Vector3(transform.forward.z, 0, -transform.forward.x);
        Vector3 tempa = armature.rotation.eulerAngles;
        armature.forward = Vector3.Lerp(tempa, tempb, Time.deltaTime * (tempb = tempa).magnitude);
        armature.rotation = Quaternion.Euler(armature.rotation.eulerAngles + Vector3.left * 90);
    }

    public override void OnDeath()
    {
        if (IsServer)
        {
            MyCore.NetCreateObject(DefUpgradePrefab, -1, transform.position, Quaternion.identity);
        }
    }
}
