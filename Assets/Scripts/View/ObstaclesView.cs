using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;
public class ObstaclesView : MotionView
{
    [SerializeField] ObstacleSpawner obstacleSpawner;
    [SerializeField] Slider SeedSlider;
    [SerializeField] Slider MovingAngleSlider;
    [SerializeField] PointA PointA;
    [SerializeField] Toggle pointReachedToggle;
    [SerializeField] bool clearObstaclesOnGeneration = true;
    [SerializeField] GameObject  MovingObject;
    private int maxSeedSliderValue
    {
        get { return (int)Math.Pow(10, 6); }
    }

    protected override void Start()
    {
        SeedSlider.onValueChanged .AddListener(Slider_OnValueChanged);
        MovingAngleSlider.onValueChanged. AddListener(MovingAngleSlider_onValueChanged);
        PointA.pointReached+= PointA_OnPointReached;
        pointReachedToggle.enabled = false;
        obstacleSpawner.clearObstaclesOnGeneration = clearObstaclesOnGeneration;
    }

    private void MovingAngleSlider_onValueChanged(float arg0)
    {
        float angle = arg0 * 2f * Mathf.PI;
        float x = Mathf.Cos(angle);
        float y = Mathf.Sin(angle);
        Vector2 rotation =  new Vector2(x, y);

        MovingObject.transform.eulerAngles =new Vector3( rotation.x*90, 0, rotation.y*90);
    }

    private void PointA_OnPointReached()
    {
        pointReachedToggle.isOn = true;
    }

    private void Slider_OnValueChanged(float sliderValue)
    {
        obstacleSpawner.SetSeed((int)sliderValue * maxSeedSliderValue);
        obstacleSpawner.SpawnObstacles();
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
        obstacleSpawner.SpawnObstacles();
    }
    public override void OnDisabled()
    {
        base.OnDisabled();
        obstacleSpawner.ClearObstacles();
    }
}