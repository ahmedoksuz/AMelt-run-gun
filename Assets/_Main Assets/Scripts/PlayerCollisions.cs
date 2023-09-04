using MoreMountains.NiceVibrations;
using GPHive.Core;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] private GameObject fireParticele;
    [SerializeField] private GameEvent miniGameStartEvent;
    private SwerveController swerveController;

    private void Start()
    {
        swerveController = GetComponent<SwerveController>();
    }


    private void OnEnable()
    {
        EventManager.NextLevelCreated += GameStarted;
    }

    private void OnDisable()
    {
        EventManager.NextLevelCreated -= GameStarted;
    }

    private void GameStarted()
    {
        fireParticele.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MiniGameStart"))
        {
            fireParticele.SetActive(false);
            GameManager.Instance.finish = true;
            miniGameStartEvent.Raise();
            swerveController.enabled = false;
            other.enabled = false;
        }

        if (other.CompareTag("Door"))
        {
            other.GetComponent<Door>().Die();
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }
    }
}