using NETWORK_ENGINE;
using System.Collections;
using UnityEngine;

public class Collectable : NetworkComponent
{
    public int scoreValue = 1000;
    private bool collected = false;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "COLLECTED")
        {
            gameObject.SetActive(false);
        }
    }

    public override void NetworkedStart()
    {
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer || collected) return;

        if (other.CompareTag("SHIP"))
        {
            collected = true;
            OnCollected();
            SendUpdate("COLLECTED", "1");
            gameObject.SetActive(false);
        }
    }

    public virtual void OnCollected()
    {
        if (IsServer)
        {
            GameMaster gm = FindObjectOfType<GameMaster>();
            if (gm != null)
            {
                gm.AddScore(scoreValue);
            }

            MyCore.NetDestroyObject(NetId);
        }
    }
}

