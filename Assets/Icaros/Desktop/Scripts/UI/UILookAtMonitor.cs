using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtMonitor : MonoBehaviour {

    public Canvas canvas;

    public void Show() {
        canvas.gameObject.SetActive(true);
    }

    public void Hide() {
        canvas.gameObject.SetActive(false);
    }
}
