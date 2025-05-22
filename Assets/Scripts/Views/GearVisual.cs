using UnityEngine;

public class GearVisual : MonoBehaviour
{
    public Gear GearData;
    public float VisualScale => m_visualScale;
    public float AngularVelocity;
    public float ToothWidth => m_toothWidth;
    public float ToothHeight => m_toothHeight;
    public bool IsClockwise = true;

    private Transform visual;
    private float m_visualScale = 1f;
    private float m_toothWidth = 0.1f;
    private float m_toothHeight = 0.1f;
    private float m_toothDepth = 0.05f;
    [SerializeField] private float rotationOffsetDeg = 0f;
    private GameObject prefab;

    public void SetVisualScale(float scale) => m_visualScale = scale;
    public void SetToothHeight(float height) => m_toothHeight = height;
    public void SetToothWidth(float width) => m_toothWidth = width;
    public void SetToothDepth(float depth) => m_toothDepth = depth;
    public void SetPrefab(GameObject gameObject) => prefab = gameObject;

    public void SetPhaseOffsetDeg(float deltaDeg)
    {
        rotationOffsetDeg = deltaDeg;
    }

    public void Regenerate()
    {
        Generate(GearData);
    }

    public void Generate(Gear gear)
    {
        GearData = gear;
        int teethCount = gear.TeethCount;

        // Очистка старой визуализации
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float bodyDiameter = VisualScale*GearData.PitchDiameter;
        visual = Instantiate(prefab, transform.position, Quaternion.identity, this.transform).transform;
        visual.localRotation = Quaternion.Euler(90f, 0f, 0f);
        visual.localScale = new Vector3(bodyDiameter*2, 0.05f, bodyDiameter*2);
        visual.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();

        // Добавление зубьев
        for (int i = 0; i < teethCount; i++)
        {
            float angle = 2 * Mathf.PI * i / teethCount + rotationOffsetDeg;
            float x = Mathf.Cos(angle) * bodyDiameter;
            float y = Mathf.Sin(angle) * bodyDiameter;

            GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tooth.transform.SetParent(transform, false);
            tooth.transform.localScale = new Vector3(m_toothWidth , m_toothHeight, m_toothDepth);
            tooth.transform.localPosition = new Vector3(x, y, 0f);

            // Направляем зуб от центра наружу
            float angleDeg = angle * Mathf.Rad2Deg;
            tooth.transform.localRotation = Quaternion.Euler(0f, 0f, angleDeg);
            tooth.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    void Update()
    {
        if (GearData == null) return;
        float direction = IsClockwise ? -1f : 1f;
        float angularSpeed = direction * AngularVelocity;
        float angle = angularSpeed * Time.time + rotationOffsetDeg;
        transform.localRotation = Quaternion.Euler(0f, 0f, GetAngle(angle));
    }
    private float GetAngle(float angle)
    {
        return -angle;
    }
}
