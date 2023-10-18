using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Icaros.Desktop.UI {
    public class UIWarningScreen : MonoBehaviour {

        public Canvas canvas;
        public Button firstButton;

        public void Show() {
            canvas.gameObject.SetActive(true);
            firstButton.Select();
        }

        public void Hide() {
            canvas.gameObject.SetActive(false);
        }

        public void OnButtonClicked() {
            UISystem.Instance.OnWarningScreenClosed();
            canvas.gameObject.SetActive(false);
        }
    }
}