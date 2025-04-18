using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractableHelm : Interactable
{
    [SerializeField] int speed;
    Vector2 lastInput;
    Rigidbody rb;
    [SerializeField] Rigidbody shipRB;
    private GameObject cameraObj;
    [SerializeField] private Transform cameraHolderPos;

    //helm camera
    public float lookSpeed = 0.5f;
    private Vector2 lookInput;
    private float yaw;
    private float pitch;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "MOVE")
        {
            if (IsServer)
            {
                lastInput = NetworkCore.Vector2FromString(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();

        //shipRB = GameObject.FindGameObjectWithTag("SHIP").GetComponent<Rigidbody>();
        if (user >= 0 && IsLocalPlayer && MyCore.NetObjs[user].GetComponent<PlayerStats>().skill == 1)
        {
            MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().overrideCamera = true;
            cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    public void Start()
    {
        base.Start();

    }

    public void Update()
    {
        base.Update();

        if (IsServer)
        {
            if (shipRB != null)
            {
                shipRB.AddForce(transform.forward * lastInput.y * speed);
                shipRB.AddTorque(transform.up * lastInput.x * speed * 10);
            }
            else
            {
                shipRB = GameObject.FindGameObjectWithTag("SHIP").GetComponent<Rigidbody>();
            }
        }

        if (IsLocalPlayer && cameraHolderPos != null && cameraObj != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraObj.transform.position = cameraHolderPos.transform.position + cameraHolderPos.transform.forward * -50;
            cameraObj.transform.LookAt(GameObject.FindGameObjectWithTag("SHIP").transform.position);
            RotateView();
        }
    }

    public void OnDestroy()
    {
        //if (cameraObj != null && IsLocalPlayer)
        //{
        //    cameraObj.transform.SetParent(MyCore.NetObjs[user].transform.GetChild(0).transform);
        //}
        if (cameraObj != null && user >= 0)
        {
            cameraObj.transform.SetParent(MyCore.NetObjs[user].transform.GetChild(0).transform);
        }
        else if (cameraObj != null)
        {
            cameraObj.transform.SetParent(null);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
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

    public void OnLook(InputAction.CallbackContext lk)
    {
        //if (!disableMovement)
        //{
        if (IsLocalPlayer)
        {
            lookInput = lk.ReadValue<Vector2>();
        }
        //}
    }

    public void RotateView()
    {
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraHolderPos.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
