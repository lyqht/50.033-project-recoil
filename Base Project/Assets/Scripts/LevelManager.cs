﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // game attributes
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    public static bool audioIsPlaying = false;
    public static bool InputSettingIsOn = false;
    public GameObject InputSettingUI;

    // level attributes
    public static LevelManager Instance;
    public static int currentLevel;

    // player attributes
    private GameObject player;
    public Vector3 playerSpawnPosition;
    public delegate void PlayerDeath();
    public static event PlayerDeath PlayerDie;
    public static void onPlayerDeath() { PlayerDie(); }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            
        } else
        {
            Destroy(this);
        }
        
    }

    void Start()
    {
        updateLevel();
        SceneManager.sceneLoaded += delegate { updateLevel(); };
        PlayerDie += delegate { respawn(); };
        if (!audioIsPlaying)
        {
            print("playing audio");
            audioIsPlaying = true;
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
        PauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = !GameIsPaused;
        savePlayerData();
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = !GameIsPaused;
    }

    public void OpenInputSetting()
    {
        PauseMenuUI.SetActive(false);
        InputSettingUI.SetActive(true);
        InputSettingIsOn = true;
    }

    IEnumerator TimesleepCoroutine(int duration)
    {
        yield return new WaitForSecondsRealtime(duration);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    void updateLevel()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel >= 0)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerSpawnPosition = player.transform.position;
            Debug.Log("updated level, now level: " + currentLevel);
        }
    }

    public void respawn()
    {
        Debug.Log("Respawning");
        player.transform.position = playerSpawnPosition;
    }

    public void savePlayerData()
    {
        SaveSystem.SavePlayer(this);
    }

    public void loadPlayerData()
    {
        PlayerData playerData = SaveSystem.LoadPlayer();
        int levelToLoad = playerData.level;
        SceneManager.LoadScene(levelToLoad);
    }

    #region UI Related Functions

    // Progress UI related functions
    private float maxDistance = 0;
    private float currentDistance = 0; // affected by how much player moves from starting point of the level.
    public Slider progressSlider; // UI to show how much the player has progressed in the level.
    public Image progressFill;
    public Gradient progressColorGradient;

    public void SetLevelProgress (float distance)
    {
        if (distance > currentDistance)
        {
            Debug.Log("setting level progress");
            currentDistance = distance;
            progressSlider.value = currentDistance;
            progressFill.color = progressColorGradient.Evaluate(progressSlider.normalizedValue);
        }
    }

    public void SetMaxDistance(float maxDistance)
    {

        progressSlider.maxValue = maxDistance;
    }

    void fillProgressSlider()
    {
        currentDistance = 0;
        progressSlider.value = currentDistance;
        progressFill.color = progressColorGradient.Evaluate(currentDistance / maxDistance);
    }

    #endregion 
}
