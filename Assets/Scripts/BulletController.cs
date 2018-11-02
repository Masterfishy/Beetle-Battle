using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float liveTime;
    public float lifetime = 1;
    public PlayerController.owner team;
    public Sprite[] bullet_sprites;
    private SpriteRenderer mySprite;
    public MenuController mc;

	void Start ()
    {
        mc = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuController>();

        mySprite = this.GetComponent<SpriteRenderer>();
        liveTime = Time.time;

        if (team == PlayerController.owner.PLAYER_1)
        {
            mySprite.sprite = bullet_sprites[0];
        }
        else if (team == PlayerController.owner.PLAYER_2)
        {
            mySprite.sprite = bullet_sprites[1];
        }
        else if (team == PlayerController.owner.NEUTRAL)
        {
            mySprite.sprite = bullet_sprites[2];
        }
    }
	
	void Update ()
    {
        if (Time.time - liveTime > lifetime)
            Destroy(this.gameObject);
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        GameObject other = coll.gameObject;
        if (other.tag.Equals("Bug"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc.myController == team)
            {
                //Do nothing
            }
            else
            {
                pc.team_health[(int)team]++;

                if (team == PlayerController.owner.PLAYER_1)
                {
                    if (pc.team_health[(int)PlayerController.owner.NEUTRAL] > 0)
                    {
                        pc.team_health[(int)PlayerController.owner.NEUTRAL]--;
                    }
                    else if (pc.team_health[(int)PlayerController.owner.PLAYER_2] > 0)
                    {
                        pc.team_health[(int)PlayerController.owner.PLAYER_2]--;
                    }
                }
                else if (team == PlayerController.owner.PLAYER_2)
                {
                    if (pc.team_health[(int)PlayerController.owner.NEUTRAL] > 0)
                    {
                        pc.team_health[(int)PlayerController.owner.NEUTRAL]--;
                    }
                    else if (pc.team_health[(int)PlayerController.owner.PLAYER_1] > 0)
                    {
                        pc.team_health[(int)PlayerController.owner.PLAYER_1]--;
                    }
                }
                else if (team == PlayerController.owner.NEUTRAL)
                {
                    int rand = (int)UnityEngine.Random.Range(0, 1.999999f);
                    if (pc.team_health[rand] > 0)
                    {
                        pc.team_health[rand]--;
                    }
                    else if (pc.team_health[(rand + 1) % 2] > 0)
                    {
                        pc.team_health[(rand + 1) % 2]--;
                    }
                }


                if (pc.team_health[(int)team] == pc.color_stability)
                {
                    pc.myController = team;
                    pc.lives--;
                    pc.change_color();
                    
                    
                }
                Destroy(this.gameObject);
            }
        }
        else if (other.tag.Equals("Bullet"))
        {
            //Do nothing
        }
        else //Walls, etc
        {
            Destroy(this.gameObject);
        }
    }

}
