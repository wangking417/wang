using FirepowerFullBlast.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirepowerFullBlast.Player
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private LayerMask interactMask = ~0;

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame || playerCamera == null)
            {
                return;
            }

            Ray ray = new(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask, QueryTriggerInteraction.Collide))
            {
                Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact(gameObject);
                }
            }
        }
    }
}

