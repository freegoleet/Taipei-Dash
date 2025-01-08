using UnityEngine;

public class CarEntity : Entity
{
    [Header("Car")]
    [SerializeField]
    private SpriteRenderer m_CarBody = null;
    [SerializeField]
    private SpriteRenderer m_CarWindows = null;

    public float AccelerationRate { get; private set; } = 1f;
    public float MaxSpeed { get; private set; } = 1f;
    public float LawbreakingRate { get; private set; } = 0.1f;

    public override void Initialize() {
        base.Initialize();
    }

    public void SetupCar(SO_CarData data) {
        AccelerationRate = Random.Range(0, data.AccelerationCoef);
        LawbreakingRate = Random.Range(0, data.LawbreakingCoef);
        MaxSpeed = Random.Range(0, data.SpeedCoef);
        var cartype = data.CarBodies[Random.Range(0, data.CarBodies.Count)];
        m_CarBody.sprite = cartype.CarBody;
        m_CarWindows.sprite = cartype.Windows;
        m_CarBody.color = data.AvailableColors[Random.Range(0, data.AvailableColors.Count)];
    }

    public override void Tick(float dt) {
        base.Tick(dt);
    }
}
