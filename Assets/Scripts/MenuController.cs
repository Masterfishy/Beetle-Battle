using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] bugs;
    public int[] team_ranks;

    private bool paused;
    private bool teamDead;
    private bool gameOver;
    public Text winText;
    public GameObject pauseMenu;
    public GameObject endMenu;

    public float cpuDir;
    public bool cpuShoot;
    public Vector2 cpuMove;
    private float cpuShootStartTime;
    private float cpuShootOnTime = 0.1f;
    public float cpuFireRate;
    private float cpuRecalcTime;
    public float cpuRecalcDelta;

    private String endQuote;
    private float endTime;

    public bool started;
    private float startCountdownTime;
    public Text startCountText;

    void Start()
    {
        Time.timeScale = 1;
        bugs = GameObject.FindGameObjectsWithTag("Bug");
        team_ranks = new int[Enum.GetNames(typeof(PlayerController.owner)).Length];
        CountTeams();

        paused = false;
        teamDead = false;
        gameOver = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        cpuRecalcTime = -100;

        started = false;
        startCountdownTime = Time.time;
        startCountText.text = "3";

    }

    void Update()
    {
        if (!started)
        {
            
            if (Time.time - startCountdownTime > 3.0f)
            {
                started = true;
                startCountText.text = "BEETLE BATTLE!";
            }
            else if (Time.time - startCountdownTime > 2.0f)
            {
                startCountText.text = "1";
            }
            else if (Time.time - startCountdownTime > 1.0f)
            {
                startCountText.text = "2";
            }
        }
        if (Time.time - startCountdownTime > 4.0f)
        {
            startCountText.gameObject.SetActive(false);
        }


        if (Input.GetButtonDown("Pause") && !gameOver)
        {
            Pause();
        }
        if (PlayerPrefs.GetInt(MainMenuController.playerCount) == 1)
        {
            SmartCPU();
        }

        if (!gameOver)
            CountTeams();

        if (teamDead && Time.time - endTime > 2.0f)
        {
            gameOver = true;
            EndGame(endQuote);
        }

    }

    public void SmartCPU()
    {
        if (Time.time - cpuRecalcTime > cpuRecalcDelta)
        {
            cpuRecalcTime = Time.time;

            List<GameObject> myTeam = new List<GameObject>();
            List<GameObject> otherTeam = new List<GameObject>();
            foreach (GameObject bug in bugs)
            {
                if (bug.GetComponent<PlayerController>().myController == PlayerController.owner.PLAYER_2)
                {
                    myTeam.Add(bug);
                }
                else
                {
                    otherTeam.Add(bug);
                }
            }

            GameObject drivingBug = myTeam[(int)UnityEngine.Random.Range(0, myTeam.Count - 0.000001f)];
            GameObject targetBug = otherTeam[(int)UnityEngine.Random.Range(0, otherTeam.Count - 0.000001f)];
            cpuMove = targetBug.transform.position - drivingBug.transform.position;
            cpuMove.Normalize();

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
        }
        
        if (Time.time - cpuShootStartTime > cpuShootOnTime)
        {
            cpuShoot = false;
        }

        if (Time.time - cpuShootStartTime > cpuFireRate)
        {
            cpuShoot = true;
            cpuShootStartTime = Time.time;
        }
    }
            
    public void Pause()
    {
        if (started)
        {
            if (paused)
            {
                paused = false;
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
            else
            {
                paused = true;
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }
    }

    public void EndGame(String endTitle)
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(false);
        gameOver = true;
        endMenu.SetActive(true);
        winText.text = endTitle;

    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RandomLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene((int)UnityEngine.Random.Range(1, SceneManager.sceneCountInBuildSettings - 0.000001f));
    }

    public void Quit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    public void CountTeams()
    {
        
        bugs = GameObject.FindGameObjectsWithTag("Bug");
        for (int i = 0; i < team_ranks.Length; i++)
        {
            team_ranks[i] = 0;
        }

        for (int i = 0; i < bugs.Length; i++)
        {
            if (bugs[i].GetComponent<PlayerController>().myController == PlayerController.owner.PLAYER_1)
            {
                team_ranks[(int)PlayerController.owner.PLAYER_1]++;
            }
            else if (bugs[i].GetComponent<PlayerController>().myController == PlayerController.owner.PLAYER_2)
            {
                team_ranks[(int)PlayerController.owner.PLAYER_2]++;
            }
            else if (bugs[i].GetComponent<PlayerController>().myController == PlayerController.owner.NEUTRAL)
            {
                team_ranks[(int)PlayerController.owner.NEUTRAL]++;
            }
        }

        
        if (team_ranks[(int)PlayerController.owner.PLAYER_1] == 0)
        {
            if (PlayerPrefs.GetInt(MainMenuController.playerCount) < 2)
            {
                endQuote = "CPU (Red) Wins!";
            }
            else
            {
                endQuote = "Player 2 (Red) Wins!";
            }
            if (!teamDead)
            {
                teamDead = true;
                endTime = Time.time;
            }
        }
        else if (team_ranks[(int)PlayerController.owner.PLAYER_2] == 0)
        {
            
            endQuote = "Player 1 (Blue) Wins!";
            if (!teamDead)
            {
                teamDead = true;
                endTime = Time.time;
            }
        }
        else
        {
            teamDead = false;
        }
        
    }
}
