using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Icaros.Desktop.Localization {
    public class LocalizationMenuItems {

        [MenuItem("ICAROS/Localization/Load Desktop Version")]
        static void loadXLSFile() {
            LocalizationManager manager = GameObject.FindObjectOfType<LocalizationManager>();
            manager.loadFiles();
            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}