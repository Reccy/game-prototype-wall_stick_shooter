using UnityEngine;
using System.Collections;

public class HueChanger : MonoBehaviour {

    SpriteRenderer sprRenderer;
    int colorHue;
    Color newColor;

    void Start() 
    {
        sprRenderer = GetComponent<SpriteRenderer>();
    }

	void Update(){
        colorHue++;
        if(colorHue >= 360)
        {
            colorHue = 0;
        }

        newColor = Color.HSVToRGB(colorHue/360f, 1, 1);
        sprRenderer.color = newColor;
	}
}
