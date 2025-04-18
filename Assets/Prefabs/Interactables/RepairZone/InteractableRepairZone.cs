using UnityEngine;
using NETWORK_ENGINE;
using System.ComponentModel;
using System.Collections;

public class InteractableRepairZone : Interactable
{
    ShipStats playerShip;
    float damage = 1;
    float damageCooldownTimer = 0f;
    float damageCooldownTime = 4f;
    public bool complete = false;

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);

        if (flag == "COMPLETE")
        {
            if (IsServer)
            {
                if (bool.Parse(value) && MyCore.NetObjs[user].GetComponent<PlayerStats>().skill == 3)
                {
                    Debug.Log("Ship healed: " + (playerShip.getMaxHealth() * 0.05f));
                    playerShip.setHealth(playerShip.getHealth() + (playerShip.getMaxHealth() * 0.05f));
                }

                complete = bool.Parse(value);


                SendUpdate("COMPLETE", complete.ToString());


            }
            if (IsClient)
            {
                complete = bool.Parse(value);
            }
            //Debug.Log("HandleMessage: " + complete);

            if (!complete)
            {
                transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                transform.GetChild(1).GetComponent<ParticleSystem>().Pause();
                GetComponent<BoxCollider>().enabled = false;
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
        base.Update();
        //if (!complete)
        //{
        //    transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        //    GetComponent<BoxCollider>().enabled = true;
        //}
        //else
        //{
        //    transform.GetChild(1).GetComponent<ParticleSystem>().Pause();
        //    GetComponent<BoxCollider>().enabled = false;
        //}

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

        //Debug.Log(NetId + ": A: " + complete);

        //if (IsLocalPlayer && complete && user >= 0)
        //{
        //    Debug.Log("This should not yet be called");
        //    MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().SendCommand("USE", NetId + "," + user + "," + true);
        //}
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
        //playerShip = transform.parent.GetComponent<ShipStats>();

        if (!complete)
        {
            transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            transform.GetChild(1).GetComponent<ParticleSystem>().Pause();
            GetComponent<BoxCollider>().enabled = false;
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

        while (IsLocalPlayer)
        {
            if (IsLocalPlayer && complete && user >= 0)
            {
                //Debug.Log("This should not yet be called");
                MyCore.NetObjs[user].GetComponent<NetworkPlayerController>().SendCommand("USE", NetId + "," + user + "," + true);
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }
    public override void SetValues(int oldInteractable)
    {
        complete = MyCore.NetObjs[oldInteractable].GetComponent<InteractableRepairZone>().complete;
        SendUpdate("COMPLETE", complete.ToString());
    }
}
