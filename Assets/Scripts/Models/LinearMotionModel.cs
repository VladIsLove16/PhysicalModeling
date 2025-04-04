using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[CreateAssetMenu(fileName = "LinearMotionModel",menuName = "MotionModels/Linear")]
public class LinearMotionModel : MotionModel
{
    public override Vector3 CalculatePosition(float deltaTime)
    {
        Vector3 speed = (Vector3)Parameters[ParamName.velocity].Value;
        ReactiveProperty<object> timeRx = Parameters[ParamName.time];
        float timeValue = (float)timeRx.Value;
        timeValue += deltaTime;
        timeRx.Value = timeValue;
        Vector3 newPosition = speed * timeValue;
        Parameters[ParamName.pathTraveled].Value = newPosition.magnitude;
        Parameters[ParamName.distance].Value = newPosition.magnitude;
        return newPosition;
    }
}


