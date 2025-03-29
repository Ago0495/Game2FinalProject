using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkComponent
{
  //  public Material[] SkeletonMatArray;
  //  public Renderer SkeletonRenderer;
    public Rigidbody MyRig;
  //  public Animator MyAnime;

    public PlayerInput MyInput;
    public InputActionAsset MyMap;

    public Vector3 LastInput;
   // public bool Attack;
    //public bool Attacking;

    public float Speed;
   // public float SwingRate;

    public override void HandleMessage(string flag, string value)
    {

        if (flag == "MOVE" && IsServer)
        {
            LastInput = Vector2FromString(value);
        }
       /* if (flag == "WALK" && IsClient)
        {
            MyAnime.SetFloat("speedh", Vector2FromString(value).magnitude);
        }
        if (flag == "SWING" && IsServer && Attack)
        {
            Attacking = true;
            Attack = false;
            SendUpdate("SWING", "1");
            MyAnime.SetBool("Attack1h1", true);
            StartCoroutine(Swing());
        }
        if (flag == "SWING" && IsClient)
        {
            Attacking = true;
        }
        if (flag == "CANSWING" && IsClient)
        {
            Attack = true;
            Attacking = false;
            MyAnime.SetBool("Attack1h1", false);
        }*/
    }
    public Vector2 Vector2FromString(string s)
    {
        string[] args = s.Trim().Trim('(').Trim(')').Split(',');
        return new Vector2(float.Parse(args[0]), float.Parse(args[1]));
    }

    public override void NetworkedStart()
    {
        //SkeletonRenderer.materials[0] = SkeletonMatArray[this.Owner % 3];
       //SkeletonRenderer.material = SkeletonMatArray[this.Owner % 3];
        if (IsServer)
        {
            int tstart = (this.Owner % 3) + 1;
            GameObject temp = GameObject.Find("SpawnPoint" + tstart);
            MyRig.position = temp.transform.position;
            MyRig.useGravity = true;
        }

    }

    public void OnDirectionChanged(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started || context.action.phase == InputActionPhase.Performed)
        {
            //This means the direction has changed!
            SendCommand("MOVE", context.ReadValue<Vector2>().ToString());
        }

        if (context.action.phase == InputActionPhase.Canceled)
        {
            //Hmmm.. why would we need this check    
            SendCommand("MOVE", Vector2.zero.ToString());
        }

    }

    /*public void OnFire(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            //Fire Button was pushed!
            if (IsLocalPlayer && Attack)
            {
                SendCommand("SWING", "1");
                Attack = false;
                StartCoroutine(Swing());
            }
        }
    }
    */
   /* public IEnumerator Swing()
    {
        yield return new WaitForSeconds(SwingRate);
        if (IsServer)
        {
            Attack = true;
            MyAnime.SetBool("Attack1h1", false);
            SendUpdate("CANSWING", Attack.ToString());
        }
        Attacking = false;
    }*/

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        MyRig = GetComponent<Rigidbody>();
        //Attack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            Vector3 temp = new Vector3(LastInput.x, 0, LastInput.y) * Speed + new Vector3(0, MyRig.linearVelocity.y, 0);
            MyRig.linearVelocity = temp;
            if (temp.magnitude > 0.1)
            {
                transform.forward = new Vector3(temp.x, 0, temp.z).normalized;
            }
           // MyAnime.SetFloat("speedh", temp.magnitude);
            SendUpdate("WALK", new Vector2(temp.x, temp.z).ToString());
        }

        if (IsClient)
        {
            /*if (Attacking)
            {
                MyAnime.SetBool("Attack1h1", true);
                Attacking = false;
            }*/
        }
        if (IsLocalPlayer)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z - 10);
        }
    }
}
