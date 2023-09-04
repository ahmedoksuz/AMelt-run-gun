using System.Collections.Generic;
using System.Linq;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace GPHive.Game.Upgrade
{
    public class IncrementalUpgrade : MonoBehaviour
    {
        public List<SerializableDictionary<Upgrade, UpgradeButton>> upgrades;


        private void OnEnable()
        {
            SetUpgrades();
        }

        private void SetUpgrades()
        {
            foreach (var _upgrade in upgrades)
            {
                _upgrade.Key.SetLevel();
                if (CheckMaxLevel(_upgrade)) continue;

                _upgrade.Value.LevelText.SetText($"LVL {_upgrade.Key.Level + 1}");
                _upgrade.Value.PriceText.SetText($"${_upgrade.Key.GetPrice()}");
            }

            CheckUpgradesBuyable();
        }

        public void CheckUpgradesBuyable()
        {
            foreach (var _upgrade in upgrades.Where(upgrade => !CheckMaxLevel(upgrade)))
            {
                _upgrade.Value.Button.interactable = _upgrade.Key.IsBuyable();
                BuyableInteractiable(_upgrade.Key.IsBuyable(), _upgrade);
            }
        }

        private static void BuyableInteractiable(bool incrabtiable,
            SerializableDictionary<Upgrade, UpgradeButton> upgrade)
        {
            if (!incrabtiable)
            {
                var levelTextColor = upgrade.Value.LevelText.color;
                levelTextColor.a = .5f;
                upgrade.Value.LevelText.color = levelTextColor;

                var PriceText = upgrade.Value.PriceText.color;
                PriceText.a = .5f;
                upgrade.Value.PriceText.color = PriceText;
            }
            else
            {
                var levelTextColor = upgrade.Value.LevelText.color;
                levelTextColor.a = 1;
                upgrade.Value.LevelText.color = levelTextColor;

                var PriceText = upgrade.Value.PriceText.color;
                PriceText.a = 1;
                upgrade.Value.PriceText.color = PriceText;
            }
        }

        private static bool CheckMaxLevel(SerializableDictionary<Upgrade, UpgradeButton> upgrade)
        {
            if (!upgrade.Key.IsMaxLevel()) return false;

            upgrade.Value.LevelText.SetText("MAX");
            upgrade.Value.PriceText.gameObject.SetActive(false);
            upgrade.Value.Button.interactable = false;

            BuyableInteractiable(false, upgrade);

            return true;
        }

        public void Upgrade(Upgrade upgrade)
        {
            if (!PlayerEconomy.Instance.SpendMoney(upgrade.GetPrice())) return;
            upgrade.BuyUpgrade();
            PlayerUpgrade.Instance.SetPlayerUpgrade(upgrade.Name);
            SetUpgrades();
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }
    }
}