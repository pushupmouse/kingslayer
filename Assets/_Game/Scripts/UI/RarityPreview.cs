using UnityEngine;
using UnityEngine.UI;

public class RarityPreview : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _commonColor;
    [SerializeField] private Color _rareColor;
    [SerializeField] private Color _epicColor;
    [SerializeField] private Color _legendaryColor;
    
    public RarityType Rarity { get; private set; }
    
    public void Init(RarityType rarityType)
    {
        Rarity = rarityType;
        SetPreviewColor(rarityType);
    }
    
    private void SetPreviewColor(RarityType rarityType)
    {
        switch (rarityType)
        {
            case RarityType.Common:
                _image.color = _commonColor;
                break;
            case RarityType.Rare:
                _image.color = _rareColor;
                break;
            case RarityType.Epic:
                _image.color = _epicColor;
                break;
            case RarityType.Legendary:
                _image.color = _legendaryColor;
                break;
            default:
                Debug.LogWarning("Invalid RarityType, setting default color.");
                _image.color = _commonColor; // Default to Common color if something goes wrong
                break;
        }
    }
}
