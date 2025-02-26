using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class StopWatch : MonoBehaviour
{
    private bool timerActive;
    private float currentTime;
    private float recordTime;

    public GameObject recordPanel;
    [SerializeField] private TMP_Text timerDisplay;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text recordTimeText; 
    [SerializeField] private TMP_Text playerNameText; 
    [SerializeField] private TMP_InputField nameInputField; 
    [SerializeField] private Transform leaderboardPanel; 
    [SerializeField] private GameObject scoreEntryPrefab; 

    private string playerName = "Player"; 
    private List<(string playerName, float time)> leaderboard = new List<(string, float)>(); 

    void Start()
    {
        currentTime = 0;
        
        LoadLeaderboard();
        DisplayLeaderboard();
    }

    void Update()
    {
        if (timerActive)
        {
            currentTime += Time.deltaTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerDisplay.text = time.ToString(@"mm\:ss\:fff");
    }

    public void StartTimer()
    {
        
        playerName = string.IsNullOrWhiteSpace(nameInputField.text) ? "Player" : nameInputField.text;
        playerNameText.text = playerName; 
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
        recordPanel.SetActive(true);
        
        AddNewScore(playerName, currentTime);
        
        SaveLeaderboard();
        
        DisplayLeaderboard();
    }

    void AddNewScore(string name, float time)
    {
        leaderboard.Add((name, time));
        
        leaderboard.Sort((a, b) => a.time.CompareTo(b.time));
        
        if (leaderboard.Count > 1)
        {
            leaderboard.RemoveAt(leaderboard.Count - 1);
        }
    }

    void DisplayLeaderboard()
    {
        int index = 0;
        
        foreach (Transform child in leaderboardPanel)
        {
            if (index < leaderboard.Count)
            {
                child.gameObject.SetActive(true);
                TMP_Text[] texts = child.GetComponentsInChildren<TMP_Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = leaderboard[index].playerName;
                    texts[1].text = TimeSpan.FromSeconds(leaderboard[index].time).ToString(@"mm\:ss\:fff");
                }
            }
            else
            {
              
                child.gameObject.SetActive(false);
            }
            index++;
        }
        
        while (index < leaderboard.Count)
        {
            GameObject entry = Instantiate(scoreEntryPrefab, leaderboardPanel);
            entry.SetActive(true);
            TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = leaderboard[index].playerName;
                texts[1].text = TimeSpan.FromSeconds(leaderboard[index].time).ToString(@"mm\:ss\:fff");
            }
            index++;
        }
    }

    void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerPrefs.SetString($"LeaderboardName_{i}", leaderboard[i].playerName);
            PlayerPrefs.SetFloat($"LeaderboardTime_{i}", leaderboard[i].time);
        }
        PlayerPrefs.Save();
    }

    void LoadLeaderboard()
    {
        leaderboard.Clear();

        for (int i = 0; i < 1; i++) 
        {
            if (PlayerPrefs.HasKey($"LeaderboardTime_{i}"))
            {
                string name = PlayerPrefs.GetString($"LeaderboardName_{i}", "Player");
                float time = PlayerPrefs.GetFloat($"LeaderboardTime_{i}", Mathf.Infinity);
                leaderboard.Add((name, time));
            
                Debug.Log($"Chargé : {name} - {time}");
            }
        }

        Debug.Log($"Nombre de scores chargés : {leaderboard.Count}");
    }
    
    
    public void ValidateName()
    {
        if (!string.IsNullOrWhiteSpace(nameInputField.text))
        {
            playerName = nameInputField.text;
            playerNameText.text = playerName;
            Debug.Log("Nom validé : " + playerName);
        }
        else
        {
            Debug.LogWarning("Veuillez entrer un nom avant de valider !");
        }
    }
}