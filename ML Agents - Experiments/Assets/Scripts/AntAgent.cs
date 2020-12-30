using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces
public class AntAgent : Agent
{
    [Header("Target To Walk Towards")]
    [Space(10)]
    public Transform target;

    public Transform ground;
    public bool detectTargets;
    public bool targetIsStatic;
    public bool respawnTargetWhenTouched;
    public float targetSpawnRadius;

    [Header("Body Parts")]
    [Space(10)]
    public Transform body;
    public Transform leg0Upper;
    public Transform leg0Lower;
    public Transform leg1Upper;
    public Transform leg1Lower;
    public Transform leg2Upper;
    public Transform leg2Lower;
    public Transform leg3Upper;
    public Transform leg3Lower;
    public Transform leg4Upper;
    public Transform leg4Lower;
    public Transform leg5Upper;
    public Transform leg5Lower;


    [Header("Joint Settings")]
    [Space(10)]
    JointDriveController m_JdController;
    Vector3 m_DirToTarget;
    float m_MovingTowardsDot;
    float m_FacingDot;

    [Header("Reward Functions To Use")]
    [Space(10)]
    public bool rewardMovingTowardsTarget; // Agent should move towards target

    public bool rewardFacingTarget; // Agent should face the target
    public bool rewardUseTimePenalty; // Hurry up

    [Header("Foot Grounded Visualization")]
    [Space(10)]
    public bool useFootGroundedVisualization;

    public MeshRenderer foot0;
    public MeshRenderer foot1;
    public MeshRenderer foot2;
    public MeshRenderer foot3;
    public MeshRenderer foot4;
    public MeshRenderer foot5;
    public Material groundedMaterial;
    public Material unGroundedMaterial;

    Quaternion m_LookRotation;
    Matrix4x4 m_TargetDirMatrix;

    public override void Initialize()
    {
        m_JdController = GetComponent<JointDriveController>();
        m_DirToTarget = target.position - body.position;


        //Setup each body part
        // There's 1 body
        // There's 12 leg parts
        m_JdController.SetupBodyPart(body);
        m_JdController.SetupBodyPart(leg0Upper);
        m_JdController.SetupBodyPart(leg0Lower);
        m_JdController.SetupBodyPart(leg1Upper);
        m_JdController.SetupBodyPart(leg1Lower);
        m_JdController.SetupBodyPart(leg2Upper);
        m_JdController.SetupBodyPart(leg2Lower);
        m_JdController.SetupBodyPart(leg3Upper);
        m_JdController.SetupBodyPart(leg3Lower);
        m_JdController.SetupBodyPart(leg4Upper);
        m_JdController.SetupBodyPart(leg4Lower);
        m_JdController.SetupBodyPart(leg5Upper);
        m_JdController.SetupBodyPart(leg5Lower);
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        var rb = bp.rb;
        sensor.AddObservation(bp.groundContact.touchingGround ? 1 : 0); // Whether the bp touching the ground // +1 obs

        var velocityRelativeToLookRotationToTarget = m_TargetDirMatrix.inverse.MultiplyVector(rb.velocity);
        sensor.AddObservation(velocityRelativeToLookRotationToTarget); // +3 obs

        var angularVelocityRelativeToLookRotationToTarget = m_TargetDirMatrix.inverse.MultiplyVector(rb.angularVelocity);
        sensor.AddObservation(angularVelocityRelativeToLookRotationToTarget); // +3 obs

        if (bp.rb.transform != body)
        {
            var localPosRelToBody = body.InverseTransformPoint(rb.position);
            sensor.AddObservation(localPosRelToBody); // +3 obs
            sensor.AddObservation(bp.currentXNormalizedRot); // Current x rot // +1 obs
            sensor.AddObservation(bp.currentYNormalizedRot); // Current y rot // +1 obs
            sensor.AddObservation(bp.currentZNormalizedRot); // Current z rot // +1 obs
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit); // +1 obs
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        m_JdController.GetCurrentJointForces();

        // Update pos to target
        m_DirToTarget = target.position - body.position;
        m_LookRotation = Quaternion.LookRotation(m_DirToTarget);
        m_TargetDirMatrix = Matrix4x4.TRS(Vector3.zero, m_LookRotation, Vector3.one);

        RaycastHit hit;
        if (Physics.Raycast(body.position, Vector3.down, out hit, 10.0f))
        {
            sensor.AddObservation(hit.distance);
        }
        else
            sensor.AddObservation(10.0f); // +1 obs

        // Forward & up to help with orientation
        var bodyForwardRelativeToLookRotationToTarget = m_TargetDirMatrix.inverse.MultiplyVector(body.forward);
        sensor.AddObservation(bodyForwardRelativeToLookRotationToTarget); // +3 obs

        var bodyUpRelativeToLookRotationToTarget = m_TargetDirMatrix.inverse.MultiplyVector(body.up);
        sensor.AddObservation(bodyUpRelativeToLookRotationToTarget); // +3 obs

        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        } 
        /**
         * Previously there were 8 leg parts. i.e. 14 obs per leg part i.e. 112 for just leg obs. then we have 7 for initial obs. and then 7 more for just the head
         * Now that we have 12 leg parts. we'll have 168 for leg obs. 7 initial and then 7 for head. thus 182 input vector size
         */
    }

    /// <summary>
    /// Agent touched the target
    /// </summary>
    public void TouchedTarget()
    {
        AddReward(1f);
        Debug.Log(string.Format("Yes! {0} of parent {1} touched the Target!", gameObject.name, gameObject.transform.parent.name));
        if (respawnTargetWhenTouched)
        {
            GetRandomTargetPos();
        }
    }

    /// <summary>
    /// Moves target to a random position within specified radius.
    /// </summary>
    public void GetRandomTargetPos()
    {
        var newTargetPos = Random.insideUnitSphere * targetSpawnRadius;
        newTargetPos.y = 5;
        target.position = newTargetPos + ground.position;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // The dictionary with all the body parts in it are in the jdController
        var bpDict = m_JdController.bodyPartsDict;

        var i = -1;
        // Pick a new target joint rotation
        bpDict[leg0Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[leg1Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[leg2Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[leg3Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
        bpDict[leg4Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0); 
        bpDict[leg5Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0); // +4 floats
        bpDict[leg0Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[leg1Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[leg2Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[leg3Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[leg4Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
        bpDict[leg5Lower].SetJointTargetRotation(vectorAction[++i], 0, 0); // +2 floats

        // Update joint strength
        bpDict[leg0Upper].SetJointStrength(vectorAction[++i]);
        bpDict[leg1Upper].SetJointStrength(vectorAction[++i]);
        bpDict[leg2Upper].SetJointStrength(vectorAction[++i]);
        bpDict[leg3Upper].SetJointStrength(vectorAction[++i]);
        bpDict[leg4Upper].SetJointStrength(vectorAction[++i]);
        bpDict[leg5Upper].SetJointStrength(vectorAction[++i]); // +2 floats
        bpDict[leg0Lower].SetJointStrength(vectorAction[++i]);
        bpDict[leg1Lower].SetJointStrength(vectorAction[++i]);
        bpDict[leg2Lower].SetJointStrength(vectorAction[++i]);
        bpDict[leg3Lower].SetJointStrength(vectorAction[++i]);
        bpDict[leg4Lower].SetJointStrength(vectorAction[++i]);
        bpDict[leg5Lower].SetJointStrength(vectorAction[++i]); // +2 floats

        /**
         * Initially we had 20 outputs from the NN. Now we have 30 outputs
         */
    }

    void FixedUpdate()
    {
        if (detectTargets)
        {
            foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
            {
                if (bodyPart.targetContact && bodyPart.targetContact.touchingTarget)
                {
                    TouchedTarget();
                }
            }
        }

        // If enabled the feet will light up green when the foot is grounded.
        // This is just a visualization and isn't necessary for function
        if (useFootGroundedVisualization)
        {
            foot0.material = m_JdController.bodyPartsDict[leg0Lower].groundContact.touchingGround
                ? groundedMaterial
                : unGroundedMaterial;
            foot1.material = m_JdController.bodyPartsDict[leg1Lower].groundContact.touchingGround
                ? groundedMaterial
                : unGroundedMaterial;
            foot2.material = m_JdController.bodyPartsDict[leg2Lower].groundContact.touchingGround
                ? groundedMaterial
                : unGroundedMaterial;
            foot3.material = m_JdController.bodyPartsDict[leg3Lower].groundContact.touchingGround
                ? groundedMaterial
                : unGroundedMaterial;
        }

        // Set reward for this step according to mixture of the following elements.
        if (rewardMovingTowardsTarget)
        {
            RewardFunctionMovingTowards();
        }

        if (rewardFacingTarget)
        {
            RewardFunctionFacingTarget();
        }

        if (rewardUseTimePenalty)
        {
            RewardFunctionTimePenalty();
        }
    }

    /// <summary>
    /// Reward moving towards target & Penalize moving away from target.
    /// </summary>
    void RewardFunctionMovingTowards()
    {
        m_MovingTowardsDot = Vector3.Dot(m_JdController.bodyPartsDict[body].rb.velocity, m_DirToTarget.normalized);
        AddReward(0.03f * m_MovingTowardsDot);
    }

    /// <summary>
    /// Reward facing target & Penalize facing away from target
    /// </summary>
    void RewardFunctionFacingTarget()
    {
        m_FacingDot = Vector3.Dot(m_DirToTarget.normalized, body.forward);
        AddReward(0.01f * m_FacingDot);
    }

    /// <summary>
    /// Existential penalty for time-contrained tasks.
    /// </summary>
    void RewardFunctionTimePenalty()
    {
        AddReward(-0.001f);
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        Debug.Log(string.Format("New Episode started for {0} of parent {1} ", gameObject.name, gameObject.transform.parent.name));
        if (m_DirToTarget != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(m_DirToTarget);
        }
        transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));

        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
        if (!targetIsStatic)
        {
            GetRandomTargetPos();
        }
    }
}
