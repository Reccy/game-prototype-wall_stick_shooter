using UnityEngine;
using System.Collections;

public class EditorLagFix : MonoBehaviour {

    //Sets quality level to lowest in editor
	void Start () {
	    if(Application.isEditor)
        {
            QualitySettings.antiAliasing = 0;
        }
        
        Debug.Log("Current AA: " + QualitySettings.antiAliasing);
	}
}
