using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System;
[Serializable]
public struct LabConfig
{
    public int index;
    public MotionModel[] motionModels;
}
public class MotionController : MonoBehaviour
{
    [SerializeField] private List<MotionModel> MotionModels; // Список всех моделей
    [SerializeField] private TMP_Dropdown MotionModelsDropdown;
    [SerializeField] private RigidbodyView Viewrb;
    [SerializeField] private TransformView Viewtr;
    [SerializeField] private ObstaclesView Viewobs;
    [SerializeField] private RampView Viewramp;
    [SerializeField] private RampBlockView ViewRampBlock;
    [SerializeField] private RefractionLensView RefractionView;
    [SerializeField] private RefractionMaterialsView RefractionMaterialsView;
    [SerializeField] private GearView GearView;
    [SerializeField] private WaterView WaterView;
    [SerializeField] private PistonView PistonView;
    [SerializeField] private SubmarineView SubmarineView;
    [SerializeField] private PointA pointA;
    [SerializeField] private MotionView CurrentView;
    [SerializeField] private LabConfig[] configs;
    
    private MotionViewModel ViewModel;
    public MotionModel CurrentMotionModel { get; private set; }
   public int LabConfig;

    private void Start()
    {
        MotionModelsDropdown.ClearOptions();
        MotionModelsDropdown.AddOptions(MotionModels.Select(m => m.Title).ToList());

        CurrentMotionModel = configs.First(x=> x.index == LabConfig).motionModels[0];
        CurrentMotionModel.InitializeParameters();

        ViewModel = new MotionViewModel(CurrentMotionModel);
        InitView();

        MotionModelsDropdown.onValueChanged.AddListener(OnMotionModelChanged);
    }
    
    private void OnMotionModelChanged(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= MotionModels.Count)
            return;

        SetModel(MotionModels[selectedIndex]);
    }
    public void SetModel(MotionModel motionModel)
    {
        CurrentMotionModel = motionModel;
        CurrentMotionModel.OnDisabled();
        CurrentMotionModel.InitializeParameters();
        if (CurrentMotionModel is HitMotionModel hitMotionModel)
            hitMotionModel.Init(Viewrb.MovingObjectrb, Viewrb.HittedObjectrb);
        else if (CurrentMotionModel is ObstaclesMotionModel obstaclesMotionModel)
            obstaclesMotionModel.Init(Viewobs.MovingObject, pointA);
        //else if (CurrentMotionModel is RefractionLensMotionModel refractionLensMotionModel)
        //    refractionLensMotionModel
        CurrentMotionModel.OnEnabled();
        ViewModel.Init(CurrentMotionModel);
        InitView();
    }
    private void InitView()
    {
        if (CurrentView != null)
            CurrentView.OnDisabled();
        if (CurrentMotionModel is HitMotionModel hitMotionModel)
            CurrentView = Viewrb;
        else if (CurrentMotionModel is ObstaclesMotionModel obstaclesMotionModel)
            CurrentView = Viewobs;
        else if (CurrentMotionModel is RampMotionModel rampMotionModel)
            CurrentView = Viewramp;
        else if (CurrentMotionModel is RampBlockMotionModel ramblockMotionModel)
            CurrentView = ViewRampBlock;
        else if (CurrentMotionModel is RefractionLensMotionModel refractionLensMotionModel)
            CurrentView = RefractionView;
        else if (CurrentMotionModel is RefractionMaterialsMotionModel refractionMaterialsMotionModel)
            CurrentView = RefractionMaterialsView;
        else if (CurrentMotionModel is GearMotionModel gearMotionModel)
            CurrentView = GearView;
        else if (CurrentMotionModel is WaterMotionModel waterMotionModel)
            CurrentView = WaterView;
        else if (CurrentMotionModel is PistionMotionModel pistionMotionModel)
            CurrentView = PistonView;
        else if (CurrentMotionModel is SubmarineMotionModel submarineMotionModel)
            CurrentView = SubmarineView;
        else
            CurrentView = Viewtr;
        Debug.Log("Current view setted to " + CurrentView.name);
        CurrentView.OnEnabled();
        CurrentView.Init(ViewModel);
    }
}