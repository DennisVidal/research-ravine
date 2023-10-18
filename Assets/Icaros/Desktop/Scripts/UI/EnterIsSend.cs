using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Icaros.Desktop.UI {
    public class EnterIsSend : MonoBehaviour {

        public Button toBePressed;

        private void Update() {
            if (!(UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter) || UnityEngine.Input.GetKeyDown(KeyCode.Return)) || EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
                return;

            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)
                return;

            toBePressed.OnSubmit(new PointerEventData(EventSystem.current));
        }
    }
}
