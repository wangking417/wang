using System.Collections;
using UnityEngine;

namespace FirepowerFullBlast.Interaction
{
    public class RotatingDoorInteractable : Interactable
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Vector3 openEulerAngles = new(0f, 90f, 0f);
        [SerializeField] private float openSpeed = 2f;

        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private Coroutine _activeRoutine;
        private bool _isOpen;

        private void Awake()
        {
            if (pivot == null)
            {
                pivot = transform;
            }

            _closedRotation = pivot.localRotation;
            _openRotation = _closedRotation * Quaternion.Euler(openEulerAngles);
        }

        public override void Interact(GameObject interactor)
        {
            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
            }

            _activeRoutine = StartCoroutine(RotateRoutine(_isOpen ? _closedRotation : _openRotation));
            _isOpen = !_isOpen;
        }

        private IEnumerator RotateRoutine(Quaternion targetRotation)
        {
            while (Quaternion.Angle(pivot.localRotation, targetRotation) > 0.1f)
            {
                pivot.localRotation = Quaternion.Slerp(pivot.localRotation, targetRotation, Time.deltaTime * openSpeed);
                yield return null;
            }

            pivot.localRotation = targetRotation;
            _activeRoutine = null;
        }
    }
