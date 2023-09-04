using GPHive.Core;
using UnityEngine;


public class PlayerUpgrade : Singleton<PlayerUpgrade>
{
    public float Length
    {
        get => length;
        set => length = value;
    }

    public float Power
    {
        get => power;
        set => power = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    [SerializeField] private float power;
    [SerializeField] private float speed;
    [SerializeField] private float length;

    [SerializeField] private float defaultPower;
    [SerializeField] private float defaultSpeed;
    [SerializeField] private float defaultLength;


    [Header("INC Upgrade")] [SerializeField]
    private float defaultParentParticleRange;

    [SerializeField] private float doorValueGateDividerForUpgrade;

    [SerializeField] private SwerveControllerConfig playerSwerveControllerConfig;

    [SerializeField] private ParticleSystem flameParentParticle, flameGlowParticle;
    [SerializeField] private float defaultMeltAmount;
    [SerializeField] private float defaultMeltIncreaseAmount;
    [SerializeField] private float incLimit;
    [SerializeField] private float incLimitDecrease;

    private void OnEnable()
    {
        EventManager.NextLevelCreated += _Reset;
    }

    private void OnDisable()
    {
        EventManager.NextLevelCreated -= _Reset;
    }

    public void _Reset()
    {
        power = PlayerPrefs.GetFloat("PlayerPower", defaultPower);
        speed = PlayerPrefs.GetFloat("PlayerSpeed", defaultSpeed);
        length = PlayerPrefs.GetFloat("PlayerLength", defaultLength);

        ParticleUpdate(length + defaultParentParticleRange);
    }

    public void SetPlayerUpgrade(string _name)
    {
        var _defaultInc = defaultMeltIncreaseAmount;
        if (PlayerPrefs.GetInt("Power", 0) + PlayerPrefs.GetInt("Speed", 0) +
            PlayerPrefs.GetInt("Length", 0) >= incLimit)
            _defaultInc /= incLimitDecrease;

        var _incMeltAmount = PlayerPrefs.GetFloat("IncMeltAmount", defaultMeltAmount);
        _incMeltAmount += _defaultInc;

        switch (_name)
        {
            case "Power":

                power = _incMeltAmount / (speed * (length / playerSwerveControllerConfig.ForwardSpeed));
                if (power > 10) power = 10;
                PlayerPrefs.SetFloat("PlayerPower", power);

                break;
            case "Speed":

                speed = _incMeltAmount / (power * (length / playerSwerveControllerConfig.ForwardSpeed));
                if (speed > 20) speed = 20;
                PlayerPrefs.SetFloat("PlayerSpeed", speed);


                break;
            case "Length":

                length = playerSwerveControllerConfig.ForwardSpeed * (_incMeltAmount / (power * speed));
                if (length < 16) length = 16;
                if (length > 30) length = 30;
                PlayerPrefs.SetFloat("PlayerLength", length);

                ParticleUpdate(length + defaultParentParticleRange);

                break;
        }

        PlayerPrefs.SetFloat("IncMeltAmount", _incMeltAmount);
    }

    private float DoorValueCalculator(float doorValue)
    {
        var _newValue = doorValue / doorValueGateDividerForUpgrade;

        return _newValue;
    }

    public void Upgrade(string _name, float doorValue)
    {
        switch (_name)
        {
            case "Length":

                length += DoorValueCalculator(doorValue);

                if (length < defaultLength) length = defaultLength;

                ParticleUpdate(length + defaultParentParticleRange);

                break;
            case "Power":

                power += DoorValueCalculator(doorValue);

                if (power < defaultPower) power = defaultPower;

                break;

            case "Speed":

                speed += DoorValueCalculator(doorValue);

                if (speed < defaultSpeed) speed = defaultSpeed;

                break;
        }
    }

    private void ParticleUpdate(float parentParticleRange)
    {
        flameParentParticle.startLifetime = parentParticleRange - 15;
        flameGlowParticle.startLifetime = parentParticleRange - 15;
    }
}