using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    public InputField nameInputField = null;
    public Button continueButton= null;
    
    public static string DisplayName {get; private set;}
    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start () => SetUpInputField();

    private void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        nameInputField.text= defaultName;
        SetPlayerName();
    }

    public void SetPlayerName()
    {
        string name = nameInputField.text;
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}
