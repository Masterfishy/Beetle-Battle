using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuText : MonoBehaviour {
    
    [Range(0f, 1f)]
    public float lerpTime;

    [Range(1f, 20f)]
    public float maxDifference;

    private RectTransform rt;
    private Vector2 home;
    private bool top;

    // Use this for initialization
    void Start () {
        rt = GetComponent<RectTransform>();
        home = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y);
    }
	
	// Update is called once per frame
	void Update () {
        bob();
        if (rt.anchoredPosition.y >= home.y + maxDifference - 1) {
            top = true;
        }
        if (rt.anchoredPosition.y <= home.y - maxDifference + 1) {
            top = false;
        }
    }

    void bob() {
        if (top) {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(home.x, home.y - maxDifference), lerpTime);
        }
        else {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(home.x, home.y + maxDifference), lerpTime);
        }
    }
}