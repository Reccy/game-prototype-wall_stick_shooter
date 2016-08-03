using UnityEngine;
using System.Collections;

public class HueChanger : MonoBehaviour {

    public float speed = 1f;
    GameObject targetObject;
    SpriteRenderer spriteRenderer;
    int colorHue;
    Color newColor;

    void Start() 
    {
        try
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        catch
        {
            Debug.LogError("No sprite renderer attached.");
        }
        
    }

	void Update(){
        colorHue = colorHue + Mathf.FloorToInt(speed * 65f * Time.deltaTime);
        if(colorHue >= 360)
        {
            colorHue = 0;
        }

        newColor = Color.HSVToRGB(colorHue/360f, 1, 1);
        
        if(spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
	}
}
