using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject[] levelSelect;
    public GameObject[] howTo;
    public GameObject[] credits;
    public static string playerCount = "2P";
    

	void Start ()
    {
        levelSelect = GameObject.FindGameObjectsWithTag("MenuLevelSelect");
        howTo = GameObject.FindGameObjectsWithTag("MenuHowTo");
        credits = GameObject.FindGameObjectsWithTag("MenuCredits");

        mainMenu.SetActive(true);
        bulkToggle(levelSelect, false);
        bulkToggle(howTo, false);
        bulkToggle(credits, false);
        PlayerPrefs.SetInt(playerCount, 2);
        
    }
    
    public void ReloadMain()
    {
        mainMenu.SetActive(true);
        bulkToggle(levelSelect, false);
        bulkToggle(howTo, false);
        bulkToggle(credits, false);
    }

    public void LevelSelect(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void RandomLevel()
    {
        Time.timeScale = 1;
        int q = (int)UnityEngine.Random.Range(1, SceneManager.sceneCountInBuildSettings - 0.000001f);
        SceneManager.LoadScene(q);
    }

    public void OpenLevelSelectMenu(bool twoPlayer)
    {
        PlayerPrefs.SetInt(playerCount, twoPlayer ? 2 : 1);
        mainMenu.SetActive(false);
        bulkToggle(levelSelect, true);
        bulkToggle(howTo, false);
        bulkToggle(credits, false);
    }

    public void OpenHowToMenu()
    {
        mainMenu.SetActive(false);
        bulkToggle(levelSelect, false);
        bulkToggle(howTo, true);
        bulkToggle(credits, false);
    }

    public void OpenCreditsMenu()
    {
        mainMenu.SetActive(false);
        bulkToggle(levelSelect, false);
        bulkToggle(howTo, false);
        bulkToggle(credits, true);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    private void bulkToggle(GameObject[] go, bool on)
    {
        foreach (GameObject g in go)
        {
            g.SetActive(on);
        }
    }
}
