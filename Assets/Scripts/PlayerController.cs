using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // For who controls it
    public enum owner {PLAYER_1, PLAYER_2, NEUTRAL};
	public owner myController;
    public MenuController mc;

    

    public int lives;
    public int color_stability;
    public int[] team_health;
    public Sprite[] color_sprites; 

    // ===== MOVEMENT =====
    public float max_velocity;
    private float max_cpu_velocity;
    public float acceleration_factor;
    public float reverse_factor;
    public float brake_factor;
    // ====================

    // ===== SHOOTING =====
    public float bullet_lifetime;
    public float bullet_rate;
    public float rapid_fire_bullet_rate;
    private float last_fire_time = -1f;
    public float bullet_speed;
    public GameObject bullet;
    private bool holding_trigger;
    
    private Rigidbody2D rb;
    private SpriteRenderer mySprite;
    private Animator anim;

    public bool dead;
    private float deadTime;

    public int num_controllers;

    private GameObject target;

	void Start ()
    {
        rb = this.GetComponent<Rigidbody2D>();
        mySprite = this.GetComponent<SpriteRenderer>();
        team_health = new int[Enum.GetNames(typeof(PlayerController.owner)).Length];
        team_health[(int)myController] = color_stability;
        num_controllers = Input.GetJoystickNames().Length;
        change_color();
        mc = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuController>();
        anim = this.GetComponent<Animator>();
        max_cpu_velocity = max_velocity * cpuSpeedFactor;
        

    }


    void Update()
    {
        if (myController == owner.NEUTRAL)
            DumbCPU();


        if (!dead)
        {
            anim.SetInteger("Color", (int)myController);
            if (mc.started)
            {
                anim.SetBool("Moving", rb.velocity.magnitude > 0.15f * max_velocity);
                CheckFire();
            }
        }
        else
        {
            if (Time.time - deadTime > 4.0f)
            {
                Destroy(this.gameObject);
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.05f);
            }
        }

        
    }

    void FixedUpdate ()
    {
        num_controllers = Input.GetJoystickNames().Length;
        if (!dead && mc.started)
        {
            Move();
            Turn();
        }
	}

    private void CheckFire()
    {
        if (myController == owner.PLAYER_1)
        {
            if ((Input.GetButtonDown("Fire1") || (Input.GetAxis("Fire1J") > 0.05f && !holding_trigger))
                && Time.time - last_fire_time > 1 / rapid_fire_bullet_rate)
            {
                Fire();
                last_fire_time = Time.time;
            }
            else if ((Input.GetButton("Fire1") || Input.GetAxis("Fire1J") > 0.05f)
                && Time.time - last_fire_time > 1 / bullet_rate)
            {
                Fire();
                last_fire_time = Time.time;
            }
        }
        else if (myController == owner.PLAYER_2)
        {
            if (PlayerPrefs.GetInt(MainMenuController.playerCount) >= 2)
            {
                if ((Input.GetButtonDown("Fire2") || (Input.GetAxis("Fire2J") > 0.05f && !holding_trigger))
                    && Time.time - last_fire_time > 1 / rapid_fire_bullet_rate)
                {
                    Fire();
                    last_fire_time = Time.time;
                }
                if ((Input.GetButton("Fire2") || Input.GetAxis("Fire2J") > 0.05f)
                    && Time.time - last_fire_time > 1 / bullet_rate)
                {
                    Fire();
                    last_fire_time = Time.time;
                }
            }
            else
            {
                if (mc.cpuShoot
                    && Time.time - last_fire_time > 1 / rapid_fire_bullet_rate)
                {
                    Fire();
                    last_fire_time = Time.time;
                }
            }
        }
        else if (myController == owner.NEUTRAL)
        {
            if (cpuShoot
                    && Time.time - last_fire_time > 1 / rapid_fire_bullet_rate)
            {
                Fire();
                last_fire_time = Time.time;
            }
        }

        if (myController == owner.PLAYER_1)
            holding_trigger = Input.GetAxis("Fire1J") > 0.05f;
        else if (myController == owner.PLAYER_2)
            holding_trigger = Input.GetAxis("Fire2J") > 0.05f;
    }


    private void Fire()
    {
        GameObject newBullet = Instantiate(bullet, this.transform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().velocity = bullet_speed * Rotate(Vector2.right, this.transform.rotation.eulerAngles.z);  //TODO: FIX
        newBullet.GetComponent<BulletController>().lifetime = bullet_lifetime;
        newBullet.GetComponent<BulletController>().team = myController;
    }

    private void Turn()
    {
        float rotation = -1;
        float x = 0;
        float y = 0;
        if (myController == owner.PLAYER_1)
        {
            if (num_controllers >= 1)
            {
                x = Input.GetAxis("HorizontalRotate1J");
                y = Input.GetAxis("VerticalRotate1J");
            }

            if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f)
            {
                x = Input.GetAxis("HorizontalRotate1");
                y = Input.GetAxis("VerticalRotate1");
            }
        }
        else if (myController == owner.PLAYER_2)
        {
            if (num_controllers >= 2)
            {
                x = Input.GetAxis("HorizontalRotate2J");
                y = Input.GetAxis("VerticalRotate2J");
            }

            if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f)
            {
                x = Input.GetAxis("HorizontalRotate2");
                y = Input.GetAxis("VerticalRotate2");
            }
        }
        else if (myController == owner.NEUTRAL)
        {

        }



        if (x > 0.25f)
        {
            if (y > 0.25f)
                rotation = 45;
            else if (y < -0.25f)
                rotation = 315;
            else
                rotation = 0;

        }
        else if (x < -0.25f)
        {
            if (y > 0.25f)
                rotation = 135;
            else if (y < -0.25f)
                rotation = 225;
            else
                rotation = 180;
        }
        else
        {
            if (y > 0.25f)
                rotation = 90;
            else if (y < -0.25f)
                rotation = 270;
        }

        if (PlayerPrefs.GetInt(MainMenuController.playerCount) < 2 && myController == owner.PLAYER_2)
            rotation = mc.cpuDir;
        if (myController == owner.NEUTRAL)
            rotation = cpuDir;

        if (rotation >= 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, rotation);

        }
    }

    /* Handles movement.
 * 
 */
    private void Move()
    {
        Vector2 input = Vector2.zero;
        if (myController == owner.PLAYER_1)
        {
            if (num_controllers >= 1)
            {
                input = new Vector2(Input.GetAxis("HorizontalMove1J"), Input.GetAxis("VerticalMove1J"));
            }
            if (input.magnitude <= 0.05f)
            {
                input = new Vector2(Input.GetAxis("HorizontalMove1"), Input.GetAxis("VerticalMove1"));
            }
        }
        else if (myController == owner.PLAYER_2)
        {
            if (PlayerPrefs.GetInt(MainMenuController.playerCount) >= 2)
            {
                if (num_controllers >= 2)
                {
                    input = new Vector2(Input.GetAxis("HorizontalMove2J"), Input.GetAxis("VerticalMove2J"));
                }
                if (input.magnitude <= 0.05f)
                {
                    input = new Vector2(Input.GetAxis("HorizontalMove2"), Input.GetAxis("VerticalMove2"));
                }
            }
            else
            {
                input = mc.cpuMove;
            }
        }
        else if (myController == owner.NEUTRAL)
        {
            input = cpuMove * cpuSpeedFactor;
        }
        input *= acceleration_factor;
        Vector2 vel = rb.velocity;
        if (Vector2.Angle(input, vel) > 90)
        {
            input *= reverse_factor;
        }
        else
        {
            if (vel.magnitude >= max_velocity)
            {
                rb.AddForce(-1 * input.magnitude * vel.normalized);
            }
            if (myController == owner.NEUTRAL && vel.magnitude >= max_cpu_velocity)
                rb.AddForce(-1 * input.magnitude * vel.normalized);
        }

        if (input.magnitude < 0.05f)
        {
            rb.velocity = Vector2.Lerp(vel, Vector2.zero, brake_factor / 100f);
        }
        else
        {
            rb.AddForce(input);
        }
    }

    public float cpuDir;
    public bool cpuShoot;
    public Vector2 cpuMove;
    private float cpuShootStartTime = -1f;
    private float cpuShootOnTime = 0.1f;
    public float cpuFireRate;
    private float cpuRecalcTime = -3f;
    public float cpuRecalcDelta;
    public float cpuSpeedFactor;
    public float cpuIdleChance = 0.3f;
    public void DumbCPU()
    {
        if (Time.time - cpuRecalcTime > cpuRecalcDelta && UnityEngine.Random.Range(0f, 1f) < 0.005f)
        {
            cpuRecalcTime = Time.time;

            List<GameObject> otherTeam = new List<GameObject>();
            foreach (GameObject bug in GameObject.FindGameObjectsWithTag("Bug"))
            {
                if (bug != this.gameObject)
                {
                    otherTeam.Add(bug);
                }
            }

            GameObject drivingBug = this.gameObject;
            GameObject targetBug = otherTeam[(int)UnityEngine.Random.Range(0, otherTeam.Count - 0.000001f)];
            cpuMove = targetBug.transform.position - drivingBug.transform.position;
            cpuMove.Normalize();
            cpuMove *= cpuSpeedFactor;

            if (cpuMove.y < 0)
            {
                float ang = Vector2.Angle(Vector2.right, cpuMove);
                if (ang < 22.5f)
                    cpuDir = 0;
                else if (ang < 67.5f)
                    cpuDir = 315;
                else if (ang < 112.5f)
                    cpuDir = 270;
                else if (ang < 157.5)
                    cpuDir = 225;
                else if (ang <= 180)
                    cpuDir = 180;
            }
            else
            {
                float ang = Vector2.Angle(Vector2.right, cpuMove);
                if (ang < 22.5f)
                    cpuDir = 0;
                else if (ang < 67.5f)
                    cpuDir = 45;
                else if (ang < 112.5f)
                    cpuDir = 90;
                else if (ang < 157.5)
                    cpuDir = 135;
                else if (ang <= 180)
                    cpuDir = 180;
            }

            if (UnityEngine.Random.Range(0f, 1f) < cpuIdleChance)
            {
                
                cpuMove = Vector2.zero;
            }
        }

        

        if (Time.time - cpuShootStartTime > cpuShootOnTime)
        {
            cpuShoot = false;
        }

        if (Time.time - cpuShootStartTime > cpuFireRate && UnityEngine.Random.Range(0f, 1f) < 0.005f)
        {
            cpuShoot = true;
            cpuShootStartTime = Time.time;
        }
    }

    public void change_color()
    {
        if (lives <= 0)
        {
            dead = true;
            deadTime = Time.time;
            anim.SetBool("Dead", true);
            Destroy(this.GetComponent<BoxCollider2D>());
            this.gameObject.tag = "Untagged";
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0.25f) ;
        }
        else
        {
            if (myController == owner.PLAYER_1)
            {
                mySprite.sprite = color_sprites[0];
            }
            else if (myController == owner.PLAYER_2)
            {
                mySprite.sprite = color_sprites[1];
            }
            else if (myController == owner.NEUTRAL)
            {
                mySprite.sprite = color_sprites[2];
            }
        }
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        return new Vector2(((cos * v.x) - (sin * v.y)), ((sin * v.x) + (cos * v.y)));
    }

}
