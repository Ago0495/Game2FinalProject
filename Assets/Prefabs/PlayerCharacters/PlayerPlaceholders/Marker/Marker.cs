using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class Marker : NetworkComponent
{
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }

    private void OnDestroy()
    {
        MyCore.NetDestroyObject(NetId);
    }
}
