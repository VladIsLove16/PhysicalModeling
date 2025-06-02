using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PistionMotionModel", menuName = "MotionModelsDropdown/PistionMotionModel")]
internal class PistionMotionModel : MotionModel
{
    private float pistonHeightDelta = 0f;
    private bool isMoving = false;
    private float velocity = 0f;
    private Dictionary<ParamName, object> defaultValues = new Dictionary<ParamName, object>()
        {
            { ParamName.piston1Square, 1f },
            { ParamName.piston2Square, 3f},
            { ParamName.isMoving, false},
            { ParamName.weight, 1f},
            { ParamName.applyingForce, 10f}, 
    };
    protected override Dictionary<ParamName, object> DefaultValues
    {
        get
        {
            return defaultValues;
        }
    }
    protected override Dictionary<ParamName, object> MaxValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
            {
                { ParamName.applyingForce, 100f },
                { ParamName.weight, 10f },
            }; ;
        }
    }
    protected override Dictionary<ParamName, object> MinValues
    {
        get
        {
            return new Dictionary<ParamName, object>()
    {
            { ParamName.applyingForce, 0f },
            { ParamName.weight, 1f },
    }; ;
        }
    }
    public override Vector3 CalculatePosition(float Time)
    {
        return new Vector3(0, pistonHeightDelta, 0);
    }
    public override void ResetParam(TopicField field)
    {
        if (field.ParamName == ParamName.weight)
            return;
        if (field.ParamName == ParamName.applyingForce)
            return;
        base.ResetParam(field);
    }
    public override Vector3 UpdatePosition(float deltaTime)
    {
        float piston1Square = (float)GetParam(ParamName.piston1Square);
        float piston2Square = (float)GetParam(ParamName.piston2Square);
        float weight = (float)GetParam(ParamName.weight);
        float force = (float)GetParam(ParamName.applyingForce);

        float g = 9.81f;
        float gravityForce = weight * g;
        float pascalForce = force * (piston2Square / piston1Square);
        Debug.Log("pascalForce " + pascalForce + "  gravityForce: " + gravityForce);
        if (weight == 0)
        {
            velocity = 0;
            isMoving = false;
        }
        else if (pascalForce > gravityForce)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        float excessForce = pascalForce - gravityForce;
        float acceleration = excessForce / weight;
        velocity = acceleration * deltaTime;

        pistonHeightDelta = velocity;
        Debug.Log(pistonHeightDelta);
        TrySetParam(ParamName.pistonHeightDelta, pistonHeightDelta);
        TrySetParam(ParamName.isMoving, isMoving);
        TrySetParam(ParamName.velocity, new Vector3(0,velocity,0));
        return new Vector3(0, pistonHeightDelta, 0);
    }

    public override List<TopicField> GetRequiredParams()
    {
        return new List<TopicField>
        {
            new TopicField(ParamName.piston1Square, false),
            new TopicField(ParamName.piston2Square, false),
            new TopicField(ParamName.weight, false),
            new TopicField(ParamName.applyingForce, false),
            new TopicField(ParamName.pistonHeightDelta, true),
            new TopicField(ParamName.velocity, true),
            new TopicField(ParamName.isMoving, true)
        };
    }
}
