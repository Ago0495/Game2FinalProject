using NETWORK_ENGINE;
using System.Collections;
using UnityEngine;

public class Collectable : NetworkComponent
{
    public int scoreValue = 1000;
    private bool collected = false;
    public GameMaster gm;
    public GameObject ship;
    public GameObject[] cannons;
    public GameObject collectAudio;
    public override void HandleMessage(string flag, string value)
    {
        if (flag == "COLLECTED")
        {
            //gameObject.SetActive(false);
            collectAudio.GetComponent<AudioSource>().Play();
        }
    }

    public override void NetworkedStart()
    {
        gm = FindObjectOfType<GameMaster>();
        ship = GameObject.FindGameObjectWithTag("SHIP");
        collectAudio = GameObject.FindGameObjectWithTag("COLLECTAUDIO");
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
            gm.AddScore(scoreValue);
            OnCollected();
            SendUpdate("COLLECTED", "1");
            //gameObject.SetActive(false);
            collected = true;
        }
    }

    public virtual void OnCollected()
    {

    }
}

