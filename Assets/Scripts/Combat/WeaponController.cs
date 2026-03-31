using System.Collections;
using System.Collections.Generic;
using FirepowerFullBlast.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirepowerFullBlast.Combat
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private List<WeaponDefinition> loadout = new();
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private float impactForce = 20f;

        private int _currentWeaponIndex;
        private int _currentMagazineAmmo;
        private int _currentReserveAmmo;
        private float _nextShotTime;
        private bool _isReloading;
        private bool _triggerReleasedSinceLastShot = true;

        private WeaponDefinition CurrentWeapon => loadout.Count == 0 ? null : loadout[_currentWeaponIndex];

        private void Start()
        {
            if (loadout.Count == 0)
            {
                enabled = false;
                Debug.LogWarning("WeaponController requires at least one WeaponDefinition.");
                return;
            }

            EquipWeapon(0);
        }

        private void Update()
        {
            if (CurrentWeapon == null)
            {
                return;
            }

            HandleWeaponSwitch();

            if (_isReloading)
            {
                return;
            }

            HandleReload();
            HandleFire();
        }

        private void HandleWeaponSwitch()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                EquipWeapon(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame && loadout.Count > 1)
            {
                EquipWeapon(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame && loadout.Count > 2)
            {
                EquipWeapon(2);
            }
        }

        private void HandleReload()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                TryStartReload();
            }
        }

        private void HandleFire()
        {
            if (Mouse.current == null)
            {
                return;
            }

            bool fireHeld = Mouse.current.leftButton.isPressed;

            if (!fireHeld)
            {
                _triggerReleasedSinceLastShot = true;
                return;
            }

            if (!_triggerReleasedSinceLastShot && !CurrentWeapon.automatic)
            {
                return;
            }

            if (Time.time < _nextShotTime)
            {
                return;
            }

            if (_currentMagazineAmmo <= 0)
            {
                GameEventBus.RaiseStatusChanged("Reload Required");
                TryStartReload();
                return;
            }

            FireShot();
            _triggerReleasedSinceLastShot = false;
        }

        private void FireShot()
        {
            _nextShotTime = Time.time + 1f / Mathf.Max(CurrentWeapon.fireRate, 0.01f);
            _currentMagazineAmmo--;
            GameEventBus.RaiseAmmoChanged(_currentMagazineAmmo, _currentReserveAmmo);
            GameEventBus.RaiseStatusChanged("Firing");

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            Vector3 origin = aimCamera != null ? aimCamera.transform.position : transform.position;
            Vector3 direction = aimCamera != null ? aimCamera.transform.forward : transform.forward;
            direction += Random.insideUnitSphere * CurrentWeapon.hipSpread;

            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, CurrentWeapon.range, CurrentWeapon.hitMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(direction.normalized * impactForce, ForceMode.Impulse);
                }

                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(CurrentWeapon.damage, gameObject);
                }
                else
                {
                    Health targetHealth = hit.collider.GetComponentInParent<Health>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(CurrentWeapon.damage, gameObject);
                    }
                }
            }
        }

        private void TryStartReload()
        {
            if (_isReloading || _currentMagazineAmmo >= CurrentWeapon.magazineSize || _currentReserveAmmo <= 0)
            {
                return;
            }

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            _isReloading = true;
            GameEventBus.RaiseStatusChanged("Reloading");
            yield return new WaitForSeconds(CurrentWeapon.reloadTime);

            int ammoNeeded = CurrentWeapon.magazineSize - _currentMagazineAmmo;
            int ammoToLoad = Mathf.Min(ammoNeeded, _currentReserveAmmo);
            _currentMagazineAmmo += ammoToLoad;
            _currentReserveAmmo -= ammoToLoad;

            _isReloading = false;
            GameEventBus.RaiseAmmoChanged(_currentMagazineAmmo, _currentReserveAmmo);
            GameEventBus.RaiseStatusChanged("Combat Ready");
        }

        private void EquipWeapon(int index)
        {
            if (index < 0 || index >= loadout.Count)
            {
                return;
            }

            _currentWeaponIndex = index;
            _currentMagazineAmmo = CurrentWeapon.magazineSize;
            _currentReserveAmmo = CurrentWeapon.reserveAmmo;
            _isReloading = false;

            GameEventBus.RaiseWeaponChanged(CurrentWeapon.weaponName);
            GameEventBus.RaiseAmmoChanged(_currentMagazineAmmo, _currentReserveAmmo);
            GameEventBus.RaiseStatusChanged("Combat Ready");
        }

        private void OnDrawGizmosSelected()
        {
            if (muzzlePoint == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(muzzlePoint.position, 0.025f);
        }
    }
}
