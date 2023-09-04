using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;


namespace GPHive.Game
{
    public class PlayerEconomy : Singleton<PlayerEconomy>
    {
        [SerializeField] private TextMeshProUGUI currencyText;

        public float GetMoney()
        {
            return PlayerPrefs.GetFloat("Player Currency");
        }

        private void SetMoney(float amount)
        {
            PlayerPrefs.SetFloat("Player Currency", amount);
        }

        [SerializeField] private bool moneyTextAnimationEnabled;

        public GameEvent OnMoneyChange;
        [SerializeField] private CoefficientUpgrade incomeUpgrade;
        public float levelByIncome;

        [Button]
        private void Add10Coin()
        {
            AddMoney(100);
        }

        [Button]
        private void Add100Coin()
        {
            AddMoney(1000);
        }

        [Button]
        private void Add1000Coin()
        {
            AddMoney(10000);
        }

        private void Start()
        {
            currencyText.text = GetMoney().ToString();
        }


        /// <summary>
        /// Returns true if player have enough currency.
        /// </summary>
        /// <param name="spendAmount">Currency amount to spend</param>
        /// <returns></returns>
        public bool SpendMoney(float spendAmount)
        {
            if (GetMoney() < spendAmount) return false;

            var _oldMoney = GetMoney();
            SetMoney(GetMoney() - spendAmount);
            if (moneyTextAnimationEnabled)
                MoneyTextAnimation(_oldMoney);
            else
                SetMoneyText();

            OnMoneyChange.Raise();
            return true;
        }


        public void AddMoney(float amount)
        {
            amount = amount * (1 + levelByIncome * incomeUpgrade.Level);
            var _oldMoney = GetMoney();
            SetMoney(GetMoney() + amount);
            if (moneyTextAnimationEnabled)
                MoneyTextAnimation(_oldMoney);
            else
                SetMoneyText();

            OnMoneyChange.Raise();
        }

        public bool CheckEnoughMoney(float amount)
        {
            return GetMoney() >= amount;
        }

        private void MoneyTextAnimation(float _oldMoney)
        {
            DOTween.To(() => _oldMoney, x => _oldMoney = x, GetMoney(), 1)
                .OnUpdate(() => currencyText.text = _oldMoney.ToString());
        }

        private void SetMoneyText()
        {
            currencyText.text = GetMoney().ToString();
        }
    }
}