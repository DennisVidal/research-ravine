using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Icaros.Desktop.UI {
    public class TabTargetting : MonoBehaviour {
        private void Update() {
            if (!UnityEngine.Input.GetKeyDown(KeyCode.Tab) || EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
                return;

            Selectable current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            if (current == null)
                return;

            bool up = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
            Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

            // We are at the end of the chain
            if (next == null) {
                next = current;

                Selectable pnext;
                if (up) while ((pnext = next.FindSelectableOnDown()) != null) next = pnext;
                else while ((pnext = next.FindSelectableOnUp()) != null) next = pnext;
            }

            // Simulate Inputfield MouseClicked
            InputField inputfield = next.GetComponent<InputField>();
            if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(EventSystem.current));

            EventSystem.current.SetSelectedGameObject(next.gameObject);
        }
    }
}
