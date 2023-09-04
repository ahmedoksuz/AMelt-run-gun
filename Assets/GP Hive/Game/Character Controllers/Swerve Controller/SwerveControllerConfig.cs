using UnityEngine;

[CreateAssetMenu(fileName = "Swerve Config", menuName = "GP Hive Objects/Character Controllers/Swerve Config")]
public class SwerveControllerConfig : ScriptableObject
{
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float smooth;
    [SerializeField] private float swerveLimit;
    [SerializeField] private float inputResetTime;
    [SerializeField] private float threshold;

    public float ForwardSpeed => forwardSpeed;
    public float HorizontalSpeed => horizontalSpeed;
    public float Sensitivity => sensitivity;
    public float Smooth => smooth;
    public float SwerveLimit => swerveLimit;
    public float InputResetTime => inputResetTime;
    public float Threshold => threshold;
}