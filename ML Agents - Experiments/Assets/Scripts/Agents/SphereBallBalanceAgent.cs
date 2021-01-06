using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SphereBallBalanceAgent : Agent
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
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var zpos = Mathf.Clamp(vectorAction[0], -1f, 1f);
        var xpos = Mathf.Clamp(vectorAction[1], -1f, 1f);

        // Changing the z position
        if ((gameObject.transform.localPosition.z < 2.5f && zpos > 0f) ||
            (gameObject.transform.localPosition.z > -2.5f && zpos < 0f))
        {
            gameObject.transform.localPosition += new Vector3(0, 0, zpos);            
        }

        if ((gameObject.transform.localPosition.x < 2.5f && xpos > 0f) ||
            (gameObject.transform.localPosition.x > -2.5f && xpos < 0f))
        {
            gameObject.transform.localPosition += new Vector3(zpos, 0, 0);
        }
                
        if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
            Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
            Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
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
        gameObject.GetComponent<Renderer>().material.color = new Color(1, 1 - (numberOfTimesLost * 1.0f / 5), 1 - (numberOfTimesLost * 1.0f / 5));

        gameObject.transform.localPosition = new Vector3(Random.Range(-1.25f, 1.25f), 0, Random.Range(-1.25f, 1.25f));
        ballRigidbody.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.localPosition = new Vector3(Random.Range(-1.25f, 1.25f), 5f, Random.Range(-1.25f, 1.25f)) + gameObject.transform.localPosition;
        ResetScene();
    }

    void ResetScene()
    {
        ballRigidbody.mass = defaultParameters.GetWithDefault("mass", 1.0f);
        var scale = defaultParameters.GetWithDefault("scale", 1.0f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }
}
