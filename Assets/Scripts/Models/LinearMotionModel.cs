using UnityEngine;

[CreateAssetMenu(fileName = "LinearMotionModel", menuName = "MotionModels/Linear")]
public class LinearMotionModel : MotionModel
{
    public override Vector3 UpdatePosition(float deltaTime)
    {
        Vector3 velocity = GetParameter<Vector3>(ParamName.velocity);
        Vector3 position = GetParameter<Vector3>(ParamName.position);
        float time = GetParameter<float>(ParamName.time);
        float path = GetParameter<float>(ParamName.pathTraveled);

        Vector3 delta = velocity * deltaTime;
        Vector3 newPosition = position + delta;
        float newTime = time + deltaTime;
        float newPath = path + velocity.magnitude * deltaTime;

        SetParameter(ParamName.position, newPosition.ToString());
        SetParameter(ParamName.time, newTime.ToString());
        SetParameter(ParamName.pathTraveled, newPath.ToString());
        SetParameter(ParamName.distance, newPosition.magnitude.ToString());

        Debug.Log("UpdatePosition: " + newPosition);
        return newPosition;
    }

    public override Vector3 CalculatePosition(float time)
    {
        Vector3 velocity = GetParameter<Vector3>(ParamName.velocity);
        Vector3 startPosition = GetParameter<Vector3>(ParamName.position);

        Vector3 delta = velocity * time;
        Vector3 newPosition = startPosition + delta;

        SetParameter(ParamName.time, time.ToString());
        SetParameter(ParamName.position, newPosition.ToString());
        SetParameter(ParamName.pathTraveled, delta.magnitude.ToString());
        SetParameter(ParamName.distance, newPosition.magnitude.ToString());

        Debug.Log("CalculatePosition: " + newPosition);
        return newPosition;
    }
}
