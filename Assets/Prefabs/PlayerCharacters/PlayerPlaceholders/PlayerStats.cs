using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;

public class PlayerStats : Entity
{
    //sync vals
    public int skill;

    //non-sync vals
    public static string[] skills = {
        "Cannoneer",
        "Helmsman",
        "Marksman",
        "Repairman"
    };

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "SETSKILL")
        {

        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(MyCore.MasterTimer);
    }
}
