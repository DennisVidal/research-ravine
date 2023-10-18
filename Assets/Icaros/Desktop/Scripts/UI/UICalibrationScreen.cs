using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Desktop.UI {
    public class UICalibrationScreen : MonoBehaviour {
        
        public Canvas canvas;
        public GameObject getOnText;
        public GameObject lookStraightText;
        public Canvas menuCamOverlay;
        public GameObject menuGetOnText;
        public GameObject menuLookStraightText;

        public void Show(int textNr) {
            canvas.gameObject.SetActive(true);
            menuCamOverlay.gameObject.SetActive(true);

            if (textNr == 0) {
                getOnText.gameObject.SetActive(true);
                lookStraightText.gameObject.SetActive(false);
                menuGetOnText.gameObject.SetActive(true);
                menuLookStraightText.gameObject.SetActive(false);
            } else {
                getOnText.gameObject.SetActive(false);
                lookStraightText.gameObject.SetActive(true);
                menuGetOnText.gameObject.SetActive(false);
                menuLookStraightText.gameObject.SetActive(true);
            }
        }

        public void Hide() {
            getOnText.SetActive(false);
            lookStraightText.SetActive(false);
            canvas.gameObject.SetActive(false);
            menuCamOverlay.gameObject.SetActive(false);
            menuGetOnText.SetActive(false);
            menuLookStraightText.SetActive(false);
        }
    }
}