using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MinionMoverAgent : Agent
{
    public GameObject groundPlane;
    public GameObject prize;
    public GameObject Head;
    public GameObject Leg1;
    public GameObject Leg2;
    public GameObject Leg3;

    [Header("--- Private Variables ---")]
    //Rigidbody prizeRigidbody;
    Rigidbody minionRigidbody;
    EnvironmentParameters defaultParameters;

    float distanceToPrize, prevDistanceToPrize;
    int numberOfWins;
    int winLimit;

    public override void Initialize()
    {
        winLimit = 10;
        distanceToPrize = -1;
        prevDistanceToPrize = -1;

        //prizeRigidbody = prize.GetComponent<Rigidbody>();
        minionRigidbody = GetComponent<Rigidbody>();
        defaultParameters = Academy.Instance.EnvironmentParameters;
        ResetScene();
    }

    void ResetScene()
    {
        //prizeRigidbody.mass = defaultParameters.GetWithDefault("mass", 1.0f);
        var scale = defaultParameters.GetWithDefault("scale", 1.0f);
        prize.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void HitPrize()
    {
        Debug.Log(string.Format("Agent:{0}. Hit the Prize", transform.parent.name));
        AddReward(1.0f);
        numberOfWins++;
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {   
        sensor.AddObservation(prize.transform.localPosition);
        sensor.AddObservation(Head.transform.position);
        sensor.AddObservation(groundPlane.transform.position.y);
        sensor.AddObservation(minionRigidbody.velocity);
        sensor.AddObservation(Leg1.transform.localRotation.eulerAngles);
        sensor.AddObservation(Leg2.transform.localRotation.eulerAngles);
        sensor.AddObservation(Leg3.transform.localRotation.eulerAngles);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var leg1RotX = Mathf.Clamp(vectorAction[0], -1f, 1f);
        var leg1RotY = Mathf.Clamp(vectorAction[1], -1f, 1f);
        var leg1RotZ = Mathf.Clamp(vectorAction[2], -1f, 1f);

        var leg2RotX = Mathf.Clamp(vectorAction[3], -1f, 1f);
        var leg2RotY = Mathf.Clamp(vectorAction[4], -1f, 1f);
        var leg2RotZ = Mathf.Clamp(vectorAction[5], -1f, 1f);

        var leg3RotX = Mathf.Clamp(vectorAction[6], -1f, 1f);
        var leg3RotY = Mathf.Clamp(vectorAction[7], -1f, 1f);
        var leg3RotZ = Mathf.Clamp(vectorAction[8], -1f, 1f);

        Leg1.transform.Rotate(new Vector3(leg1RotX, leg1RotY, leg1RotZ));
        Leg2.transform.Rotate(new Vector3(leg2RotX, leg2RotY, leg2RotZ));
        Leg3.transform.Rotate(new Vector3(leg3RotX, leg3RotY, leg3RotZ));

        // Punish if it falls off
        /**if (Head.transform.localPosition.y < 0 ||
            Mathf.Abs(Head.transform.localPosition.x) > 10f ||
            Mathf.Abs(Head.transform.localPosition.z) > 10f)*/
        if (Head.transform.position.y < groundPlane.transform.position.y)
        {
            Debug.Log(string.Format("Agent:{0}. Fell Off", transform.parent.name));
            SetReward(-1f);
            numberOfWins = 0;
            EndEpisode();
        }
        else
        {
            // Rewarding coming closer to the prize
            distanceToPrize = Vector3.Distance(Head.transform.position, prize.transform.position);
            if (prevDistanceToPrize > -1)
            {
                float diffDistance = prevDistanceToPrize - distanceToPrize;
                if (diffDistance > 0)
                {
                    AddReward(diffDistance / prevDistanceToPrize);
                }
                else
                {
                    float scale = 2f;
                    AddReward(scale * (diffDistance / prevDistanceToPrize)); // penalize it more for going away from the goal
                }
            }
            prevDistanceToPrize = distanceToPrize;

            // Punishing being upside down
            float Leg1Y = Leg1.transform.Find("Arm1/Toe1").position.y;
            float Leg2Y = Leg2.transform.Find("Arm2/Toe2").position.y;
            float Leg3Y = Leg3.transform.Find("Arm3/Toe3").position.y;
            float HeadY = Head.transform.position.y;

            if((HeadY< Leg1Y) && (HeadY< Leg2Y) && (HeadY< Leg3Y))
            {
                AddReward(-0.15f);
            }
        }
    }

    public override void OnEpisodeBegin()
    {
        // the redder the sphere looks, the more number of times its dropped the ball. after 5 drops, it should be bright red
        //Color _currColor = Head.GetComponent<Renderer>().material.color;
        Color _newColor = new Color((winLimit - numberOfWins)*1.0f / winLimit, 0, numberOfWins*1.0f / winLimit);
        Head.GetComponent<Renderer>().material.color = _newColor;

        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        transform.localPosition = new Vector3(Random.Range(-9f, 9f), 4, Random.Range(-9f, 9f));
        Leg1.transform.rotation = Leg2.transform.rotation = Leg3.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        Leg1.transform.Rotate(new Vector3(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)));
        Leg2.transform.Rotate(new Vector3(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)));
        Leg3.transform.Rotate(new Vector3(Random.Range(-90f, 90f), Random.Range(-90f, 90f), Random.Range(-90f, 90f)));

        prize.transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0.5f, Random.Range(-9f, 9f));
        prevDistanceToPrize = -1;
        distanceToPrize = -1;
        ResetScene();
    }

    public override void Heuristic(float[] actionsOut)
    {

    }
}
