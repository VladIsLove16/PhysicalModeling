using UnityEngine;
using System.Collections.Generic;
using UniRx;

[CreateAssetMenu(fileName = "RampBlockMotionModel", menuName = "MotionModelsDropdown/RampBlockMotionModel")]
public class RampBlockMotionModel : MotionModel
{
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 30f },
        { ParamName.additionalMass, false },
        { ParamName.friction, 1f },
    }; ;
        }
    }
    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 60f },
        { ParamName.friction, 1f },
    }; ;
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
        { ParamName.angleDeg, 0f },
        { ParamName.friction, 0f },
    }; ;
        }
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 pos = (Vector3)GetParam(ParamName.position);
        float force = (float)RampPhysics.GetForceForMass2(GetParam(ParamName.mass2));
        (Vector3 moveVector, Vector3 newVelocity) = RampPhysics.CheckInclined(
            (float)GetParam(ParamName.mass),
            (float)GetParam(ParamName.friction),
            (float)GetParam(ParamName.angleDeg),
            (Vector3)GetParam(ParamName.velocity),
            -force,
            deltaTime);

        float newMass = (float)GetParam(ParamName.mass2) + (float)GetParam(ParamName.mass2Acceleration) * deltaTime;
        TrySetParam(ParamName.mass2, newMass);
        Vector3 newPos = pos + moveVector;
        TrySetParam(ParamName.position, newPos);
        TrySetParam(ParamName.velocity, newVelocity);
        TrySetParam(ParamName.time, (float)GetParam(ParamName.time) + deltaTime);
        TrySetParam(ParamName.isMoving, (bool)!Mathf.Approximately(moveVector.magnitude,0));
        return pos + moveVector;
    }
    //public override bool TrySetParam(ParamName ParamName, object value)
    //{
    //    Debug.Log("ParamName " + ParamName + " changed ");
    //    if (ParamName == ParamName.additionalMass)
    //    {
    //        Debug.Log("ParamName");

    //    }
    //    return base.TrySetParam(ParamName, value);
    //}
    public override Vector3 CalculatePosition(float Time)
    {
        return Vector3.zero;
    }

    public override List<TopicField> GetRequiredParams()
    {
        List<TopicField> RequiredParams = new List<TopicField>();
      
            RequiredParams = new List<TopicField>()
            {
               new TopicField(ParamName.position, true),
               new TopicField(ParamName.position2, true),
               new TopicField(ParamName.isMoving, true),
               new TopicField(ParamName.velocity, true),
               new TopicField(ParamName.angleDeg, false),
               new TopicField(ParamName.mass, false),
               new TopicField(ParamName.mass2, false),
               new TopicField(ParamName.mass2Acceleration, false),
               new TopicField(ParamName.friction, false),
               new TopicField(ParamName.time, true)
            };
        return RequiredParams;
    }
    private void OnadditionalMassParamChange()
    {

    }
    public override void ResetParam(ParamName paramName)
    {
        if (paramName == ParamName.angleDeg)
            return;
        if (paramName == ParamName.mass)
            return;
        if (paramName == ParamName.mass2)
            return;
        if (paramName == ParamName.friction)
            return;
        if (paramName == ParamName.force)
            return;
        if (paramName == ParamName.forceAcceleration)
            return;
        base.ResetParam(paramName);
    }
}
