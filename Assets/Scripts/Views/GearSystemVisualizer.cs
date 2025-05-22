using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class GearSystemVisualizer : MonoBehaviour
{
    public float gearSpacing = 0.1f;
    public float gearVisualScale = 0.1f;
    [SerializeField] private float toothWidth;
    [SerializeField] private float toothHeight;
    [SerializeField] private float toothDepth;
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GearVisualLink> links = new List<GearVisualLink>();
    [SerializeField] private List<GearVisual> generated = new List<GearVisual>();
    Gearbox gearbox = null;
    public void GenerateSystem(Gearbox gearbox)
    {
        if (gearbox == null)
            return;
        this.gearbox = gearbox;
        UpdateAllVisual(gearbox);
    }
    private void UpdateAllVisual(bool forceRegeneration = false)
    {
        UpdateAllVisual(gearbox, forceRegeneration);
    }
    private void UpdateAllVisual(Gearbox gearbox, bool forceRegeneration = false)
    {
        if (gearbox == null)
            return;
        gearbox.changed += UpdateAllVisual2;
        UpdateAllVisual2(gearbox, forceRegeneration);
    }

    private void UpdateAllVisual2(Gearbox gearbox, bool forceRegeneration)
    {
        DestoryAll();
        List<GearVisual> generatedNow = GetVisuals(gearbox, forceRegeneration);
        UpdatePositions();
    }

    private void ClearLinks()
    {
        links.Clear();
    }
    private List<GearVisual> GetVisuals(Gearbox gearbox, bool forceRegeneration = false)
    {
        if (gearbox == null)
            return null;
        ClearLinks();
        List<GearPair> stages = gearbox.GetType().GetField("stages", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(gearbox) as List<GearPair>;
        List<GearVisual> generatedNow = new();
        float phaseOffset = 0f;
        for (int i = 0; i < stages.Count; i++)
        {
            GearPair pair = stages[i];
            GearVisual driverVisual = GetGearVisual(pair.Driver);
            GearVisual drivenVisual = GetGearVisual(pair.Driven);


            //xOffset += pair.Driven.PitchDiameter * drivenVisual.VisualScale + gearSpacing;

            generatedNow.Add(driverVisual);
            generatedNow.Add(drivenVisual);

            GearVisualLink link = new GearVisualLink(driverVisual, drivenVisual, pair.GearRatio);
            float circuit = drivenVisual.GearData.PitchDiameter*gearVisualScale*Mathf.PI; // длина окржуности
            float radius = drivenVisual.GearData.PitchDiameter * gearVisualScale / 2;    // радиус окружности
            float arcLength = toothHeight/2f; // ширина зуба вдоль окружности(примерно)
            float teethDeg = (arcLength / radius); // угол соответсвубщий ширине зуба
            float angleDeg = teethDeg * 180 / Mathf.PI;
            phaseOffset =Mathf.Abs( phaseOffset) + angleDeg;
            phaseOffset*= i%2==0 ? 1 : -1f;
            drivenVisual.SetPhaseOffsetDeg(phaseOffset);
                links.Add(link);
        }
        SyncRotation();
        return generatedNow;
    }

    private void SyncRotation()
    {
        foreach (var link in links)
        {
            link.SyncRotation();
        }
    }

    public void SetAngularVelocity(float velocity)
    {
        if (generated.Count > 0)
        {
            generated[0].AngularVelocity = velocity;
            generated[0].IsClockwise = true;
        }
        SyncRotation();
    }
    private void DestoryAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
        generated.Clear();
    }
    private void DestoryExceptions(List<GearVisual> generatedNow)
    {
        var Exceptlist = generated.Except(generatedNow).ToList();
        foreach (var gen in Exceptlist)
        {
            generated.Remove(gen);
            Destroy(gen.gameObject);
        }
    }

    private void UpdatePositions()
    {
       float xOffset = 0f;
       for(int i = 0; i < generated.Count; i++)
       {
            var visual = generated[i];
            visual.transform.position = Vector3.zero + Vector3.right * ( visual.GearData.PitchDiameter * gearVisualScale + visual.ToothWidth) + Vector3.right * xOffset;
            xOffset += visual.GearData.PitchDiameter*gearVisualScale*2 +  gearSpacing + visual.ToothWidth*0.75f;
       }
    }

    private GearVisual GetGearVisual(Gear gear, bool forceRegeneration = false)
    {
        GearVisual GearVisual;
        if (forceRegeneration)
        {
            GearVisual = CreateGear(gear);
            return GearVisual;
        }
         GearVisual = generated.FirstOrDefault(x => x.GearData == gear);
        if(GearVisual == null)
            GearVisual = CreateGear(gear);
        return GearVisual;
    }
    private GearVisual CreateGear(Gear gear)
    {
        GameObject gearGO;
        GearVisual visual;
        gearGO = new GameObject($"Gear_{gear.TeethCount}");
        gearGO.transform.parent = transform;
        visual = gearGO.AddComponent<GearVisual>();
        visual.SetVisualScale(gearVisualScale);
        visual.SetToothWidth(toothWidth);
        visual.SetToothHeight(toothHeight);
        visual.SetToothDepth(toothDepth);
        visual.SetPrefab(prefab);
        visual.Generate(gear);
        generated.Add(visual);
        return visual;
    }
    private void OnValidate()
    {
        UpdateAllVisual2(gearbox, true);
    }
}
