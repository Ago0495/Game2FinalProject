using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class Interactable : NetworkComponent
{
    //sync vals
    public int user;

    //non-sync vals
    [SerializeField] int interactablePrefab; // //index in NetworkCore SpawnPrefab array
    GameObject canvasObj;
    Canvas canvas;
    bool hovered;
    Vector3 observerPosition;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "USER")
        {
            if (IsClient)
            {
                user = int.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {
            user = -1;
            SendUpdate("USER", user.ToString());
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                SendUpdate("USER", user.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //should be canvas
        canvasObj = transform.GetChild(0).gameObject;
        canvas = canvasObj.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered && user < 0)
        {
            canvas.enabled = true;
            canvasObj.transform.LookAt(observerPosition);
        }
        else
        {
            canvas.enabled = false;
        }
        hovered = false;
    }

    public void BeingHovered(Vector3 obsesrverPos)
    {
        observerPosition = obsesrverPos;
        hovered = true;
    }

    public void SetUser(int player)
    {
        user = player;
        SendUpdate("USER", user.ToString());
    }

    public virtual void UseFunctionality()
    {

    }
}
