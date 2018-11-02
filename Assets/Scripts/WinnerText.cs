using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerText : MonoBehaviour {

    private RectTransform rt;
    private Text winText;
    private int size;

	// Use this for initialization
	void Start () {
        rt = GetComponent<RectTransform>();
        winText = GetComponent<Text>();
        size = winText.fontSize;
        winText.fontSize = size * 25;
	}

    // Update is called once per frame
    void Update() {
        if (winText.fontSize >= size) {
            winText.fontSize = (int)Mathf.Lerp(winText.fontSize, size, 0.15f);
            
        }
    }
}
