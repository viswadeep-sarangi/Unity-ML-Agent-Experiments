using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BouncyBallBalanceAgent : Agent
{
    public GameObject ball;
    Rigidbody ballRigidbody;
    EnvironmentParameters defaultParameters;

    [Header("Private Fields")]
    [SerializeField] int numberOfTimesLost;


    public override void Initialize()
    {
        numberOfTimesLost = 0;
        ballRigidbody = ball.GetComponent<Rigidbody>();
        defaultParameters = Academy.Instance.EnvironmentParameters;
        ResetScene();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //base.CollectObservations(sensor);
        sensor.AddObservation(ballRigidbody.velocity);
        sensor.AddObservation(ball.transform.localPosition);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.rotation.z);
        sensor.AddObservation(transform.rotation.x);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var zpos = 2f*Mathf.Clamp(vectorAction[0], -1f, 1f);
        var xpos = 2f*Mathf.Clamp(vectorAction[1], -1f, 1f);
        var zangle = 2f * Mathf.Clamp(vectorAction[2], -1f, 1f);
        var xangle = 2f * Mathf.Clamp(vectorAction[3], -1f, 1f);

        // Changing the z position
        if ((gameObject.transform.localPosition.z < 4.0f && zpos > 0f) ||
            (gameObject.transform.localPosition.z > -4.0f && zpos < 0f))
        {
            gameObject.transform.localPosition += new Vector3(0, 0, zpos);
        }
        
        // Changing the x position
        if ((gameObject.transform.localPosition.x < 4.0f && xpos > 0f) ||
            (gameObject.transform.localPosition.x > -4.0f && xpos < 0f))
        {
            gameObject.transform.localPosition += new Vector3(xpos, 0, 0);
        }

        // Changing the z angle
        if ((gameObject.transform.rotation.z < 0.25f && zangle > 0f) ||
            (gameObject.transform.rotation.z > -0.25f && zangle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), zangle);
        }

        // Changing the x angle
        if ((gameObject.transform.rotation.x < 0.25f && xangle > 0f) ||
            (gameObject.transform.rotation.x > -0.25f && xangle < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), xangle);
        }

        // Reward/Punishment Conditions
        if (ball.transform.localPosition.y  < 0 ||
            Mathf.Abs(ball.transform.localPosition.x) > 5f ||
            Mathf.Abs(ball.transform.localPosition.z) > 5f)
        {
            SetReward(-1f);
            numberOfTimesLost++;
            EndEpisode();
        }
        else
        {
            SetReward(0.1f);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {

    }

    public override void OnEpisodeBegin()
    {
        // the redder the sphere looks, the more number of times its dropped the ball. after 5 drops, it should be bright red
        Color _currColor = gameObject.GetComponent<Renderer>().material.color;
        Color _newColor = new Color(_currColor.r, _currColor.g * (1 - (numberOfTimesLost * 1.0f / 5)), _currColor.b * (1 - (numberOfTimesLost * 1.0f / 5)));
        gameObject.GetComponent<Renderer>().material.color = _newColor;

        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));

        gameObject.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f));

        ballRigidbody.velocity = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));

        ball.transform.localPosition = new Vector3(Random.Range(-1.25f, 1.25f), 4f, Random.Range(-1.25f, 1.25f));
        ResetScene();
    }

    void ResetScene()
    {
        ballRigidbody.mass = defaultParameters.GetWithDefault("mass", 1.0f);
        var scale = defaultParameters.GetWithDefault("scale", 0.5f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }
}
