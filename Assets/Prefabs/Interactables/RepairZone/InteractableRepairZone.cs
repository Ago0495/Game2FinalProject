using UnityEngine;
using NETWORK_ENGINE;
using System.ComponentModel;

public class InteractableRepairZone : Interactable
{
    ShipStats playerShip;
    float damage = 2;
    float damageCooldownTimer = 0f;
    float damageCooldownTime = 2f;
    public bool complete = false;

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "COMPLETE")
        {
            if (IsServer)
            {
                complete = true;
                SendUpdate("COMPLETE", complete.ToString());
            }
            if (IsClient)
            {
                complete = bool.Parse(value);
            }
        }
    }



    private void Start()
    {
        base.Start();
        if (transform.parent != null)
        {
            playerShip = transform.parent.GetComponent<ShipStats>();
        }
    }

    private void Update()
    {

        if (IsClient && !complete)
        {
            base.Update();
            transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            transform.GetChild(1).GetComponent<ParticleSystem>().Pause();
            GetComponent<BoxCollider>().enabled = false;
        }

        if (IsServer && playerShip != null && !complete)
        {
            damageCooldownTimer += Time.deltaTime;
            if (damageCooldownTimer > damageCooldownTime)
            {
                playerShip.takeDamage(damage);
                damageCooldownTimer -= damageCooldownTime;
            }
        }
        else if (playerShip == null && transform.parent != null)
        {
            playerShip = transform.parent.GetComponent<ShipStats>();
        }

        if (IsLocalPlayer && complete && user >= 0)
        {
            MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().SendCommand("USE", NetId + "," + user + "," + true);
        }
    }

    public override void SetValues(int oldInteractable)
    {
        complete = MyCore.NetObjs[oldInteractable].GetComponent<InteractableRepairZone>().complete;
    }
}
