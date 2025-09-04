using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ChobiAssets.PTM;
public class RPCsHandler : MonoBehaviourPun
{
    Cannon_Fire_CS FireScript;
    Damage_Control_Center_CS DamageScript;

    private void Start()
    {
        FireScript = GetComponentInChildren<Cannon_Fire_CS>();
        DamageScript = GetComponent<Damage_Control_Center_CS>();
    }
    [PunRPC]
    public void SyncFireEvent()
    {
        if (FireScript)
            FireScript.Fire();
    }
    [PunRPC]
    public void SyncDamageEvent(float damage, int type, int index)
    {
        if (DamageScript)
            DamageScript.Receive_Damage(damage,type,index);
    }
}
