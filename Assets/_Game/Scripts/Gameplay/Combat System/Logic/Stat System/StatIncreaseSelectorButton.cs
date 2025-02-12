using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatIncreaseSelectorButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _applyCount;
    [field:SerializeField] public Button Button { get; private set; }
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Color _commonColor = new Color(0.5f, 0.5f, 0.5f); // Default Grey for common
    [SerializeField] private Color _rareColor = new Color(0.2f, 0.6f, 1f); // Default Blue for rare
    [SerializeField] private Color _epicColor = new Color(0.8f, 0f, 1f); // Default Purple for epic
    [SerializeField] private Color _legendaryColor = new Color(1f, 0.8f, 0f); // Default Yellow for legendary

    public void Init(string description, int applyCount, RarityType rarityType)
    {
        _description.text = description;
        _applyCount.text = $"Applied {applyCount} times";
        SetButtonColor(rarityType);
        Button.onClick.RemoveAllListeners();
    }
    
    private void SetButtonColor(RarityType rarityType)
    {
        switch (rarityType)
        {
            case RarityType.Common:
                _buttonImage.color = _commonColor;
                break;
            case RarityType.Rare:
                _buttonImage.color = _rareColor;
                break;
            case RarityType.Epic:
                _buttonImage.color = _epicColor;
                break;
            case RarityType.Legendary:
                _buttonImage.color = _legendaryColor;
                break;
            default:
                Debug.LogWarning("Invalid RarityType, setting default color.");
                _buttonImage.color = _commonColor; // Default to Common color if something goes wrong
                break;
        }
    }
}
