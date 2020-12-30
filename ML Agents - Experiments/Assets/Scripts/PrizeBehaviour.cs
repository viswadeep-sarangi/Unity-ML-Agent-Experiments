using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class PrizeBehaviour : MonoBehaviour
{
    public MinionMoverAgent _agentScript;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.name.Equals("Plane"))
        {
            _agentScript.HitPrize();
        }
    }
}
