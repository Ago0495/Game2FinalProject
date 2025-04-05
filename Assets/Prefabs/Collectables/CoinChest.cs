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
        Debug.Log("Override called");
        base.OnCollected();
        scoreValue += 1000;
    }
}
