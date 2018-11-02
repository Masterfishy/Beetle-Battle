using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;	

	void Start ()
    {
        cam = this.GetComponent<Camera>();
	}
	
	void Update ()
    {
        GameObject[] bugs = GameObject.FindGameObjectsWithTag("Bug");
        float min_x = 99999;
        float max_x = -99999;
        float min_y = 99999;
        float max_y = -99999;
        foreach (GameObject bug in bugs)
        {
            if (bug.GetComponent<PlayerController>().myController != PlayerController.owner.NEUTRAL)
            {
                Vector2 pos = bug.transform.position;
                if (pos.x < min_x)
                {
                    min_x = pos.x;
                }
                if (pos.x > max_x)
                {
                    max_x = pos.x;
                }
                if (pos.y < min_y)
                {
                    min_y = pos.y;
                }
                if (pos.y > max_y)
                {
                    max_y = pos.y;
                }
            }            
        }

        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3((min_x + max_x) / 2, (min_y + max_y) / 2, this.transform.position.z), 0.05f);
        float x_dist = max_x - min_x;
        float y_dist = max_y - min_y;
        float scale_size = Mathf.Lerp(cam.orthographicSize, Mathf.Clamp((Mathf.Max(x_dist, y_dist * 2.2f) / 3f), 8, 9999999), 0.05f);
        cam.orthographicSize = scale_size;
    }
}
