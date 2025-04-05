using NETWORK_ENGINE;
using UnityEngine;

public class CoinChest : Collectable
{
    void Start()
    {
    }

    void Update()
    {
    }

    public override void OnCollected()
    {
        if (IsServer)
        {
            gm.numcoinchestscollected++;
            Debug.Log(gm.numcoinchestscollected);

        }

    }
}
