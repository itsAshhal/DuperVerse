using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;
    private int coins = 0;
    private string coinsKey = "PlayerCoins";
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(this);
        LoadCoins();
    }
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        Debug.Log("Coins added: " + amount);
    }

    public void OnLevelCompleted()
    {
        AddCoins(250); 
    }
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(coinsKey, coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        if (PlayerPrefs.HasKey(coinsKey))
        {
            coins = PlayerPrefs.GetInt(coinsKey);
        }
    }

    public void ResetCoins()
    {
        coins = 0;
        SaveCoins();
    }
    public int GetCoins()
    {
        if (PlayerPrefs.HasKey(coinsKey))
        {
            coins = PlayerPrefs.GetInt(coinsKey);
        }
        return coins;
    }
}
