using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NetworkPlayerManager : NetworkComponent
{
    //sync vals

    //non-sync vals
    [SerializeField] private int playerPanelPrefab;  //index in NetworkCore SpawnPrefab array;
    GameObject playerPanel;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {
            playerPanel = MyCore.NetCreateObject(playerPanelPrefab, Owner, Vector3.zero, Quaternion.identity);
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
