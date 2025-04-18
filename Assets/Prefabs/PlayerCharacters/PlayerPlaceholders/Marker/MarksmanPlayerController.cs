using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class MarksmanPlayerController : NetworkPlayerController
{
    //sync vals
    public bool canFire = true;
    public float reloadTimer = 0;
    public float reloadTime = 2;
    //non-sync vals
    [SerializeField] int markerPrefabIndex;
    PlayerStats playerStats;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "HIT")
        {
            if (IsServer && canFire && !usingInteractable)
            {
                string[] args = value.Split(",");

                GameObject enemyHitObj = MyCore.NetObjs[int.Parse(args[0])].gameObject;
                Vector3 hitPos = NetworkCore.Vector3FromString(args[1] + "," + args[2] + "," + args[3]);

                //do damage and spawn marker
                GameObject tempMarker = MyCore.NetCreateObject(markerPrefabIndex, -1, hitPos, Quaternion.identity);
                Transform tempMarkerTransform = tempMarker.transform;
                if (tempMarkerTransform != null)
                {
                    tempMarkerTransform.SetParent(enemyHitObj.transform);
                    enemyHitObj.GetComponent<Entity>().takeDamage(5);
                }

                canFire = false;
                SendUpdate("CANFIRE", canFire.ToString());
            }
        }
        if (flag == "CANFIRE")
        {
            if (IsServer)
            {
                canFire = bool.Parse(value);
                SendUpdate("CANFIRE", canFire.ToString());
            }
            if (IsClient)
            {
                canFire = bool.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    public void Update()
    {
        base.Update();
        if (!canFire)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadTime)
            {
                canFire = true;
                reloadTimer -= reloadTime;
            }
            if (IsServer)
            {
                SendUpdate("CANFIRE", canFire.ToString());
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer)
        {
            if (context.started)
            {
                RaycastHit hit;
                if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out hit, Mathf.Infinity))
                {
                    Enemy markedEnemy = hit.collider.transform.GetComponentInParent<Enemy>();

                    if (markedEnemy != null)
                    {
                        //int enemyID = hit.transform.GetComponent<NetworkID>().NetId;
                        int enemyID = markedEnemy.GetComponent<NetworkID>().NetId;
                        Vector3 hitPos = hit.point;
                        SendCommand("HIT", enemyID.ToString() + "," + hitPos); 
                    }
                }
            }
        }
    }
}
