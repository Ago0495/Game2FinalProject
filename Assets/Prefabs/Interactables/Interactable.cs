using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class Interactable : NetworkComponent
{
    //sync vals
    public int user = -1;

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
                if (user >= 0)
                {
                    MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().currentInteractable = this;
                }
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {
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
                if (user >= 0)
                {
                    MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().currentInteractable = this;
                }
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        //should be canvas
        canvasObj = transform.GetChild(0).gameObject;
        canvas = canvasObj.GetComponent<Canvas>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (hovered && Owner < 0)
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
        if (player > 0)
        {
            PlayerControl(player);
        }
        else
        {
            GameObject newInteractable = MyCore.NetCreateObject(interactablePrefab, -1, transform.position, Quaternion.identity);
            newInteractable.transform.SetParent(transform.parent, true);
            newInteractable.GetComponent<Interactable>().user = -1;
            newInteractable.GetComponent<Interactable>().SendUpdate("USER", "-1");
            MyCore.NetDestroyObject(NetId);
        }
    }

    public void PlayerControl(int player)
    {
        if (IsServer)
        {
            GameObject newInteractable = MyCore.NetCreateObject(interactablePrefab, MyCore.NetObjs[player].Owner, transform.position, Quaternion.identity);
            newInteractable.transform.SetParent(transform.parent, true);
            newInteractable.GetComponent<Interactable>().user = player;
            newInteractable.GetComponent<Interactable>().SendUpdate("USER", player.ToString());
            MyCore.NetDestroyObject(NetId);
        }
    }



    public virtual void UseFunctionality()
    {

    }
}
