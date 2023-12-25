//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEditor;

public class RHC_InitLoad : EditorWindow {

    [InitializeOnLoad]
    public class InitOnLoad {

        static InitOnLoad() {

            RHC_SetScriptingSymbol.SetEnabled("BCG_RHC", true);

            if (!EditorPrefs.HasKey("RHC" + RHC_Version.version.ToString())) {

                EditorPrefs.SetInt("RHC" + RHC_Version.version.ToString(), 1);
                EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing and using Realistic Hovercraft Controller. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started");

                //GetWindow<RHC_WelcomeWindow>(true);

            }

        }

    }

}
