using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using TMPro;

namespace GPHive.Core
{
    public enum GameState
    {
        Idle,
        Playing,
        End
    }

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI levelText;

        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject loseScreen, unlockWeponScrean;
        [SerializeField] private GameObject startIncremental;
        [SerializeField] private GameObject endIncremental;
        [SerializeField] private List<GameObject> unlockWeapons = new();
        [SerializeField] private float OpenFinalUIAfterTime;
        [SerializeField] private Transform player;
        [SerializeField] private SwerveController swerveController;
        [SerializeField] private GameObject OutOfAmmo;

        private Vector3 playerPos;
        public bool unlockWeaponBool = false, finish = false;
        private GameState gameState;


        public GameState GameState => gameState;

        private void Awake()
        {
            playerPos = player.position;
        }

        private void Start()
        {
            SetLevelText();
            Application.targetFrameRate = 60;
        }

        private void SetLevelText()
        {
            levelText.SetText($"Level {SaveLoadManager.GetLevel()}");
            startIncremental.SetActive(true);
            endIncremental.SetActive(false);
            OutOfAmmo.SetActive(false);
        }

        public void OutOfAmmoMeth()
        {
            OutOfAmmo.SetActive(true);
        }

        public void StartLevel()
        {
            EventManager.StartLevel(SaveLoadManager.GetLevel());

            gameState = GameState.Playing;
            startIncremental.SetActive(false);
        }

        private void ResetAll()
        {
            swerveController.enabled = true;
            player.position = playerPos;
            finish = false;
            unlockWeaponBool = false;
            CameraManager.Instance.SwitchCamera("Default");
        }

        public void NextLevel()
        {
            EventManager.NextLevel(SaveLoadManager.GetLevel());
            StartCoroutine(CO_NextLevel());
        }

        private IEnumerator CO_NextLevel()
        {
            Destroy(LevelManager.Instance.ActiveLevel);
            yield return new WaitForEndOfFrame();
            LevelManager.Instance.ActiveLevel = LevelManager.Instance.CreateLevel();

            gameState = GameState.Idle;
            ResetAll();
            SetLevelText();
        }


        private void UnlockWeapon()
        {
            unlockWeponScrean.SetActive(true);
            PlayerPrefs.SetInt("ActiveStartWeapon", PlayerPrefs.GetInt("ActiveStartWeapon", 0) + 1);

            foreach (var unlock in unlockWeapons) unlock.SetActive(false);
            unlockWeapons[PlayerPrefs.GetInt("ActiveStartWeapon", 0) + 4].SetActive(true);
        }

        /// <summary>
        /// Call when level is successfully finished.
        /// </summary>
        public void WinLevel()
        {
            EventManager.SuccessLevel(SaveLoadManager.GetLevel());
            SaveLoadManager.IncreaseLevel();

            gameState = GameState.End;

            /*   if (unlockWeaponBool)
                   UnlockWeapon();
               else
                   StartCoroutine(CO_OpenUIDelayed(winScreen));*/
            StartCoroutine(CO_OpenUIDelayed(winScreen));
        }

        /// <summary>
        /// Call when level is failed.
        /// </summary>
        public void LoseLevel()
        {
            endIncremental.SetActive(true);
            EventManager.FailLevel(SaveLoadManager.GetLevel());

            gameState = GameState.End;

            StartCoroutine(CO_OpenUIDelayed(loseScreen));
        }

        private IEnumerator CO_OpenUIDelayed(GameObject UI)
        {
            yield return BetterWaitForSeconds.Wait(OpenFinalUIAfterTime);
            endIncremental.SetActive(true);
            UI.SetActive(true);
        }
    }
}