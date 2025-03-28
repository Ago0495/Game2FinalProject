using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;

public class PlayerPanelOptions : NetworkComponent
{
    //sync vals
    bool isReady;

    //non-sync vals
    [SerializeField] private Toggle readyToggle;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "READY")
        {
            Debug.Log("flag == READY");
            if (IsServer)
            {
                isReady = bool.Parse(value);
                SendUpdate("READY", isReady.ToString());
            }
            if (IsClient)
            {
                isReady = bool.Parse(value);
                readyToggle.isOn = isReady;
            }
        }
    }

    public override void NetworkedStart()
    {
        GameObject lobbyPlayerCanvas = GameObject.Find("LobbyPlayerCanvas");
        if (lobbyPlayerCanvas == null)
        {
            throw new System.Exception("ERROR: Could not find LobbyPlayerCanvas on the scene.");
        }
        else
        {
            //GetChild(0) should be grid panel
            transform.SetParent(lobbyPlayerCanvas.transform.GetChild(0));
        }

        if (!IsLocalPlayer)
        {
            if (readyToggle != null)
            {
                readyToggle.interactable = false;
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                Debug.Log("PlayerPannelOptions Is Server Slow Update");
                SendUpdate("READY", isReady.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnReadyToggleChanged(bool t)
    {
        if (IsLocalPlayer)
        {
            SendCommand("READY", t.ToString());
            
        }
    }

    public bool GetIsReady()
    {
        return isReady;
    }
}
