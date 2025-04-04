using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkComponent
{
    [SerializeField] private Rigidbody MyRig;
    private GameObject camera;
    private Transform cameraHolderPos;

    [SerializeField] private PlayerInput MyInput;
    [SerializeField] private InputActionAsset MyMap;

    [SerializeField] private float speed;
    [SerializeField] private bool canJump = true;
    public float lookSpeed = 2f;
    private Vector2 lastInput;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    private Vector3 movingPlatform;
    public Interactable currentInteractable;
    private bool usingInteractable;
    private bool disableMovement;


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
                MyRig.linearVelocity += new Vector3(0, 10, 0);
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
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {

        }
        if (IsLocalPlayer)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
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
            }
            if (context.canceled)
            {
                SendCommand("MOVE", Vector2.zero.ToString());
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SendCommand("JUMP", context.ReadValue<float>().ToString());
        }
    }

    public void OnLook(InputAction.CallbackContext lk)
    {
        if (!disableMovement)
        {
            lookInput = lk.ReadValue<Vector2>();
        }
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
    void Update()
    {
        if (IsServer)
        {
            MyRig.linearVelocity = transform.forward * speed * lastInput.y + transform.right * speed * lastInput.x + new Vector3(0, MyRig.linearVelocity.y, 0)/* + movingPlatform*/;
        }

        if (IsLocalPlayer && cameraHolderPos != null && camera != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            camera.transform.position = cameraHolderPos.transform.position;
            RotateView();
            LookForInteractable();
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
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void LookForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity))
        {
            currentInteractable = hit.collider.GetComponent<Interactable>();

            if (currentInteractable != null)
            {
                currentInteractable.BeingHovered(camera.transform.position);
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
