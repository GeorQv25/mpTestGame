using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerNameInput : MonoBehaviour
{
    public static string DisplayName { get; private set; }
    private const string PlayerPrefsNameKey = "PlayerName";
    
    [Header("UI")]
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _continueButton;


    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
            return;

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        _nameInputField.text = defaultName;

        SetPlayerName();
    }

    public void SetPlayerName()
    {
        string value = _nameInputField.text;
        _continueButton.interactable = !string.IsNullOrEmpty(value);
    }

    public void SavePlayerName()
    {
        DisplayName = _nameInputField.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}