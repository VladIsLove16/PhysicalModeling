[System.Serializable]
public class GearVisualLink
{
    public GearVisual Driver;
    public GearVisual Driven;
    public float GearRatio; // u = Z2 / Z1

    public GearVisualLink(GearVisual driver, GearVisual driven, float gearRatio)
    {
        Driver = driver;
        Driven = driven;
        GearRatio = gearRatio;
    }

    public void SyncRotation()
    {
        Driven.AngularVelocity = Driver.AngularVelocity / GearRatio;
        Driven.IsClockwise = !Driver.IsClockwise; // Противоположное направление
    }
}
