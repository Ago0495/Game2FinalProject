using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using TMPro;

public class NetworkPlayerController : NetworkComponent
{
    [SerializeField] private Rigidbody MyRig;
    [SerializeField] private Animator MyAnime;
    protected GameObject cameraObj;
    private Transform cameraHolderPos;

    [SerializeField] private PlayerInput MyInput;
    [SerializeField] private InputActionAsset MyMap;

    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private bool canJump = true;
    [SerializeField] float jumpForce;
    public float lookSpeed = 0.5f;
    private Vector2 lastInput;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    private Vector3 movingPlatform;
    public Interactable currentInteractable;
    [SerializeField] private bool usingInteractable;
    [SerializeField] private bool disableMovement;


    public override void HandleMessage(string flag, string value)
    {
        if (flag == "MOVE")
        {
            if (IsServer && !disableMovement)
            {
                lastInput = NetworkCore.Vector2FromString(value);
            }
        }
        if (flag == "JUMP")
        {
            if (IsServer && canJump && !disableMovement)
            {
                canJump = false;
                //MyRig.linearVelocity += new Vector3(0, 10, 0);
                MyRig.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            }
        }
        if (flag == "ROTATE")
        {
            if (IsServer && !disableMovement)
            {
                yaw = float.Parse(value);
                transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }
        if (flag == "USE")
        {
            string[] args = value.Split(",");
            if (IsServer)
            {
                GameObject tempInteract = MyCore.NetObjs[int.Parse(args[0])].gameObject;
                if (tempInteract != null)
                {
                    Interactable interactable = tempInteract.GetComponent<Interactable>();
                    if (!usingInteractable)
                    {
                        if (interactable.Owner < 0)
                        {
                            interactable.SetUser(int.Parse(args[1]));
                            usingInteractable = true;
                            disableMovement = true;
                            SendUpdate("USE", args[0] + "," + args[1] + "," + usingInteractable);
                        }
                    }
                    else
                    {
                        interactable.SetUser(-1);
                        usingInteractable = false;
                        disableMovement = false;
                        SendUpdate("USE", args[0] + "," + args[1] + "," + usingInteractable);
                    }
                }
                else
                {
                    Debug.LogError("ERROR: " + args[0] + " is not in scene or was removed");
                }
            }
            if (IsLocalPlayer)
            {
                usingInteractable = bool.Parse(args[2]);
                disableMovement = usingInteractable;
            }
        }
        if (flag == "TITLE")
        {
            string[] args = value.Split(",");
            if (IsClient)
            {
                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = args[0];
                transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = args[1];
            }
        }
        if (flag == "IDLE")
        {
            if (IsServer)
            {
                MyAnime.SetBool("Idle", true);
                MyAnime.SetBool("Move", false);
                SendUpdate(flag, value);
            }
            if (IsClient)
            {
                MyAnime.SetBool("Idle", true);
                MyAnime.SetBool("Move", false);
            }
        }
        if (flag == "ANJUMP")
        {
            if (IsServer)
            {
                MyAnime.SetBool("Jump", true);
                MyAnime.SetBool("Idle", false);
                SendUpdate(flag, value);
            }
            if (IsClient)
            {
                MyAnime.SetBool("Jump", true);
                MyAnime.SetBool("Idle", false);
            }
        }
        if (flag == "OnGround")
        {
            if (IsServer)
            {
                MyAnime.SetBool("Jump", false);
                SendUpdate("OnGround", value);
            }
            if (IsClient)
            {
                MyAnime.SetBool("Jump", false);
            }
        }
        if (flag == "MOVING")
        {
            if (IsServer)
            {
                MyAnime.SetBool("Move", true);
                MyAnime.SetBool("Idle", false);
                SendUpdate(flag, value);
            }
            if (IsClient)
            {
                MyAnime.SetBool("Move", true);
                MyAnime.SetBool("Idle", false);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {

        }
        if (IsLocalPlayer)
        {
            cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
            cameraHolderPos = transform.GetChild(0).transform;
        }
    }

    public void OnDirectionChanged(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if (context.started || context.performed)
            {
                SendCommand("MOVE", context.ReadValue<Vector2>().ToString());
                SendCommand("MOVING", "404");
            }
            if (context.canceled)
            {
                SendCommand("MOVE", Vector2.zero.ToString());
                SendCommand("IDLE", "404");
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SendCommand("JUMP", context.ReadValue<float>().ToString());
            SendCommand("ANJUMP", "404");
        }
    }

    public void OnLook(InputAction.CallbackContext lk)
    {
        //if (!disableMovement)
        //{
            lookInput = lk.ReadValue<Vector2>();
        //}
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (IsServer)
        {
            MyRig.AddForce((transform.forward * lastInput.y + transform.right * lastInput.x) * speed * Time.deltaTime, ForceMode.VelocityChange);
            //MyRig.linearVelocity = transform.forward * speed * lastInput.y + transform.right * speed * lastInput.x + new Vector3(0, MyRig.linearVelocity.y, 0)/* + movingPlatform*/;
            if (MyRig.linearVelocity.magnitude > maxSpeed)
            {

            }
        }

        if (IsLocalPlayer && cameraHolderPos != null && cameraObj != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraObj.transform.position = cameraHolderPos.transform.position;
            RotateView();
            if (!usingInteractable)
            {
                LookForInteractable();
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (IsServer)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].point.y < transform.position.y)
                {
                    canJump = true;
                    SendUpdate("OnGround", "404");
                    MyAnime.SetBool("Jump", false);
                    if (collision.contacts[i].otherCollider.GetComponent<Rigidbody>() != null)
                    {
                        Rigidbody TempRB = collision.contacts[i].otherCollider.GetComponent<Rigidbody>();
                        movingPlatform = TempRB.linearVelocity;
                    }
                }
            }
        }
    }
    private void RotateView()
    {
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        SendCommand("ROTATE", yaw.ToString());
        cameraObj.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void LookForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out hit, 3f))
        {
            currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                currentInteractable.BeingHovered(cameraObj.transform.position);
            }
        }
    }

    public void UseInteractable(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if(context.started && currentInteractable != null)
            {
                SendCommand("USE", currentInteractable.NetId + "," + this.NetId + "," + usingInteractable);
            }
        }
    }
}
