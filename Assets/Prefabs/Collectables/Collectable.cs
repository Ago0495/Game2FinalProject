using NETWORK_ENGINE;
using System.Collections;
using UnityEngine;

public class Collectable : NetworkComponent
{
    public int scoreValue = 1000;
    private bool collected = false;
    public GameMaster gm;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "COLLECTED")
        {
            //gameObject.SetActive(false);
        }
    }

    public override void NetworkedStart()
    {
        gm = FindObjectOfType<GameMaster>();
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (collected)
            {
                MyCore.NetDestroyObject(this.NetId);
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer || collected) return;

        if (other.CompareTag("SHIP"))
        {
            collected = true;
            gm.AddScore(scoreValue);
            OnCollected();
            SendUpdate("COLLECTED", "1");
            //gameObject.SetActive(false);
        }
    }

    public virtual void OnCollected()
    {

    }
}

