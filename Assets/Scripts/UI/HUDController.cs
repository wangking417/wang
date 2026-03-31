using FirepowerFullBlast.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace FirepowerFullBlast.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HUDController : MonoBehaviour
    {
        private Label _weaponLabel;
        private Label _healthLabel;
        private Label _ammoLabel;
        private Label _scoreLabel;
        private Label _statusLabel;

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;

            _weaponLabel = root.Q<Label>("weapon-name");
            _healthLabel = root.Q<Label>("health-label");
            _ammoLabel = root.Q<Label>("ammo-label");
            _scoreLabel = root.Q<Label>("score-label");
            _statusLabel = root.Q<Label>("status-label");

            GameEventBus.WeaponChanged += UpdateWeapon;
            GameEventBus.PlayerHealthChanged += UpdateHealth;
            GameEventBus.AmmoChanged += UpdateAmmo;
            GameEventBus.ScoreChanged += UpdateScore;
            GameEventBus.StatusChanged += UpdateStatus;

            UpdateScore(GameEventBus.CurrentScore);
        }

        private void OnDisable()
        {
            GameEventBus.WeaponChanged -= UpdateWeapon;
            GameEventBus.PlayerHealthChanged -= UpdateHealth;
            GameEventBus.AmmoChanged -= UpdateAmmo;
            GameEventBus.ScoreChanged -= UpdateScore;
            GameEventBus.StatusChanged -= UpdateStatus;
        }

        private void UpdateWeapon(string weaponName)
        {
            if (_weaponLabel != null)
            {
                _weaponLabel.text = $"Weapon: {weaponName}";
            }
        }

        private void UpdateHealth(float current, float max)
        {
            if (_healthLabel != null)
            {
                _healthLabel.text = $"HP: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }

        private void UpdateAmmo(int currentMagazine, int currentReserve)
        {
            if (_ammoLabel != null)
            {
                _ammoLabel.text = $"Ammo: {currentMagazine} / {currentReserve}";
            }
        }

        private void UpdateScore(int score)
        {
            if (_scoreLabel != null)
            {
                _scoreLabel.text = $"Score: {score}";
            }
        }

        private void UpdateStatus(string status)
        {
            if (_statusLabel != null)
            {
                _statusLabel.text = $"Status: {status}";
            }
        }
    }
}

