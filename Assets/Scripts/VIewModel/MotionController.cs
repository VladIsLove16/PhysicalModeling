using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class MotionController : MonoBehaviour
{
    [SerializeField] private List<MotionModel> MotionModels; // Список всех моделей
    [SerializeField] private TMP_Dropdown MotionModelsDropdown;
    [SerializeField] private MotionView View;
    private MotionViewModel ViewModel;
    public MotionModel CurrentMotionModel { get; private set; }

    private void Start()
    {
        MotionModelsDropdown.ClearOptions();
        MotionModelsDropdown.AddOptions(MotionModels.Select(m => m.Title).ToList());

        CurrentMotionModel = MotionModels[0];
        CurrentMotionModel.InitializeParameters();

        ViewModel = new MotionViewModel(CurrentMotionModel);
        View.Init(ViewModel);

        MotionModelsDropdown.onValueChanged.AddListener(OnMotionModelChanged);
    }

    private void OnMotionModelChanged(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= MotionModels.Count)
            return;

        CurrentMotionModel = MotionModels[selectedIndex];
        CurrentMotionModel.InitializeParameters();

        ViewModel.Init(CurrentMotionModel);
    }
}