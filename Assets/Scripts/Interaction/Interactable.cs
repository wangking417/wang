using UnityEngine;

namespace FirepowerFullBlast.Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private string interactionPrompt = "Interact";

        public string InteractionPrompt => interactionPrompt;

        public abstract void Interact(GameObject interactor);
    }
}
