using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PlayerPanelOptions : NetworkComponent
{
    //sync vals
    string playerName = "";
    bool isReady;
    int skill;    

    //non-sync vals
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Toggle readyToggle;
    [SerializeField] private TMP_Dropdown skillSelect;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "NAME")
        {
            if (IsServer)
            {
                playerName = value;
                SendUpdate("NAME", playerName);
            }
            if (IsClient)
            {
                playerName = value;
                nameField.text = playerName;
            }
        }
        if (flag == "READY")
        {
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
        if (flag == "SKILL")
        {
            if (IsServer)
            {
                skill = int.Parse(value);
                SendUpdate("SKILL", skill.ToString());
            }
            if (IsClient)
            {
                skill = int.Parse(value);
                skillSelect.SetValueWithoutNotify(skill);
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
            if (nameField != null)
            {
                nameField.interactable = false;
            }
            if (readyToggle != null)
            {
                readyToggle.interactable = false;
            }
            if (skillSelect != null)
            {
                skillSelect.interactable = false;
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                SendUpdate("NAME", playerName);
                SendUpdate("READY", isReady.ToString());
                SendUpdate("SKILL", skill.ToString());
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

    public void OnSkillSelectChanged(int i)
    {
        if (IsLocalPlayer)
        {
            SendCommand("SKILL", i.ToString());
        }
    }

    public void OnNameFieldChanged(string s)
    {
        if (IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }
    }

    public bool GetIsReady()
    {
        return isReady;
    }
}
