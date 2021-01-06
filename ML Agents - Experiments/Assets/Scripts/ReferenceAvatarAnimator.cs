using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ReferenceAvatarAnimator : MonoBehaviour
{
    public ReadJointPositionsTXT ParserScript;
    public bool CenterBody;

    Dictionary<string, List<Keyframe[]>> JointName_PositionKeyframesArray;
    bool InitializedVariables;

    List<string> _animationClipNames;
    Animation _anim;

    // Start is called before the first frame update
    void OnEnable()
    {
        _anim = GetComponent<Animation>();
        InitializedVariables = false;
        _animationClipNames = new List<string>();
        ParserScript.ParsingComplete += ParsingCompleteActionListener;
    }

    void InitializeVariables(Dictionary<string, Vector3> data, int numKeyframes)
    {
        JointName_PositionKeyframesArray = new Dictionary<string, List<Keyframe[]>>();
        foreach(string joint in data.Keys)
        {
            JointName_PositionKeyframesArray[joint] = new List<Keyframe[]>();
            JointName_PositionKeyframesArray[joint].Add(new Keyframe[numKeyframes]); // for localPosition.x
            JointName_PositionKeyframesArray[joint].Add(new Keyframe[numKeyframes]); // for localPosition.y
            JointName_PositionKeyframesArray[joint].Add(new Keyframe[numKeyframes]); // for localPosition.z
        }
    }

    void ParsingCompleteActionListener(Dictionary<double, Dictionary<string, Vector3>> data)
    {
        Debug.Log("Received parsed data");

        List<double> _timestamps = data.Keys.ToList();
        _timestamps.Sort();

        for(int i=0; i<_timestamps.Count; i++)
        {
            Dictionary<string, Vector3> _bodyPose = data[_timestamps[i]];
            float frame_time = Convert.ToSingle(_timestamps[i] / 1000);

            if (!InitializedVariables)
            {
                InitializeVariables(_bodyPose, _timestamps.Count);
                InitializedVariables = true;
            }

            foreach(string joint in _bodyPose.Keys)
            {
                Vector3 JointPos = _bodyPose[joint];
                JointName_PositionKeyframesArray[joint][0][i] = new Keyframe(frame_time, JointPos.x);
                JointName_PositionKeyframesArray[joint][1][i] = new Keyframe(frame_time, JointPos.y);
                JointName_PositionKeyframesArray[joint][2][i] = new Keyframe(frame_time, JointPos.z);
            }
        }
        Debug.Log("Populated all the keyframes");


        foreach(string joint in JointName_PositionKeyframesArray.Keys)
        {
            AnimationClip animClip = new AnimationClip();
            animClip.legacy = true;

#if UNITY_EDITOR
            animClip.wrapMode = WrapMode.PingPong;
#else
            animClip.wrapMode = WrapMode.Once;
#endif

            AnimationCurve animCurve_x = new AnimationCurve(JointName_PositionKeyframesArray[joint][0]);
            AnimationCurve animCurve_y = new AnimationCurve(JointName_PositionKeyframesArray[joint][1]);
            AnimationCurve animCurve_z = new AnimationCurve(JointName_PositionKeyframesArray[joint][2]);

            animClip.SetCurve(joint, typeof(Transform), "localPosition.x", animCurve_x);
            animClip.SetCurve(joint, typeof(Transform), "localPosition.y", animCurve_y);
            animClip.SetCurve(joint, typeof(Transform), "localPosition.z", animCurve_z);

            _animationClipNames.Add(string.Concat("AnimationClipForJoint - ", joint));
            _anim.AddClip(animClip, _animationClipNames.Last());
        }
        Debug.Log("Set all the animation curves");

        for (int i = 0; i < _animationClipNames.Count; i++)
        {
            AnimationState animState = _anim[_animationClipNames[i]];
            animState.layer = 1;
            animState.enabled = true;
            animState.weight = Convert.ToSingle(1.0 / _animationClipNames.Count);
            animState.blendMode = AnimationBlendMode.Blend;
        }
        Debug.Log("Blended all the animations");

        _anim.Play();
        Debug.Log("Playing animation...");
    }
}
