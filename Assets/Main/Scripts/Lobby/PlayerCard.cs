using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class PlayerCard : MonoBehaviour
{
    [SerializeField] private ColorByType[] _colors;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private Image _colorImage;

    private const string ReadyStatus = "<color=green>Ready</color>";
    private const string UnReadyStatus = "<color=red>Not Ready</color>";

    public void SetCard(string name, bool readyStatus, PlayerColor color)
    {
        _colorImage.color = _colors.First(config => config.Type == color).Color;
        _nameText.text = name;
        _readyText.text = readyStatus ? ReadyStatus : UnReadyStatus;
    }
}

[System.Serializable]
public class ColorByType
{
    [SerializeField] private PlayerColor _type;
    public PlayerColor Type => _type;

    [SerializeField] private Color32 _color;
    public Color32 Color => _color;
}