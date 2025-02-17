using System.Collections.Generic;
using Obvious.Soap;
using UnityEditor;
using UnityEngine;
using Yade.Runtime;

public class BoostCenter : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _siContainer;
    [SerializeField] private StatIncreaseButton _siButton;
    [SerializeField] private GameObject _miniSIContainer;
    [SerializeField] private GameObject _miniSI;
    [SerializeField] private RarityPreview _rarityPreview;
    
    [Header("Stat Increases")]
    [SerializeField] private YadeSheetData _statIncreaseChanceData;
    [SerializeField] private List<StatIncreaseData> _statIncreaseList;
    [SerializeField] private IntVariable _currentPlayerLevel;
    [SerializeField] private IntVariable _excessPlayerLevel;
    
    [SerializeField] private SceneAsset _gameScene;

    private List<StatIncreaseData> _commonStatIncreaseList = new List<StatIncreaseData>();
    private List<StatIncreaseData> _rareStatIncreaseList = new List<StatIncreaseData>();
    private List<StatIncreaseData> _epicStatIncreaseList = new List<StatIncreaseData>();
    private List<StatIncreaseData> _legendaryStatIncreaseList = new List<StatIncreaseData>();
    
    private class StatIncreaseChance
    {
        [DataField(0)] public string Level;
        [DataField(1)] public string Common;
        [DataField(2)] public string Rare;
        [DataField(3)] public string Epic;
        [DataField(4)] public string Legendary;
    }

    private List<RarityType> _rarities = new List<RarityType>(); 
    private List<StatIncreaseButton> _siButtons = new List<StatIncreaseButton>(); 
    private List<GameObject> _miniSIs = new List<GameObject>(); // Store references to Mini SIs
    private int _effectiveLevel;
    private int _numMiniContainers;
    private int _numSIButtons = 3;
    private int _numMiniPreviewsPerContainer = 3;
    private void Awake()
    {
        _statIncreaseList.ForEach(a => a.Reset());
        
        SortAbilitiesByRarity();
    }
    
    private void Start()
    {
        _effectiveLevel = _currentPlayerLevel.Value - _excessPlayerLevel.Value + 1;
        _numMiniContainers = Mathf.Max(0, _excessPlayerLevel.Value - 1);
        
        if (_excessPlayerLevel.Value >= 1)
        {
            DetermineAllRarities();
            InstantiateUIElements();
        }
    }

    private void SortAbilitiesByRarity()
    {
        _commonStatIncreaseList.Clear();
        _rareStatIncreaseList.Clear();
        _epicStatIncreaseList.Clear();
        _legendaryStatIncreaseList.Clear();

        foreach (var statIncrese in _statIncreaseList)
        {
            RarityType rarity = statIncrese.GetRarityType();

            switch (rarity)
            {
                case RarityType.Common:
                    _commonStatIncreaseList.Add(statIncrese);
                    break;
                case RarityType.Rare:
                    _rareStatIncreaseList.Add(statIncrese);
                    break;
                case RarityType.Epic:
                    _epicStatIncreaseList.Add(statIncrese);
                    break;
                case RarityType.Legendary:
                    _legendaryStatIncreaseList.Add(statIncrese);
                    break;
                default:
                    Debug.LogWarning("Something went wrong");
                    break;
            }
        }
    }
    
    private void DetermineAllRarities()
    {
        // Roll rarities for SI buttons using the first available level
        for (int i = 0; i < _numSIButtons; i++)
        {
            _rarities.Add(RollRarity(_effectiveLevel));
        }

        // Roll rarities for Mini SI previews, progressing in level
        for (int j = 0; j < _numMiniContainers; j++)
        {
            int levelForThisContainer = _effectiveLevel + j + 1; // Increase level for each mini container

            for (int k = 0; k < _numMiniPreviewsPerContainer; k++)
            {
                _rarities.Add(RollRarity(levelForThisContainer));
            }
        }
    }

    private void InstantiateUIElements()
    {
        int rarityIndex = 0;

        for (int i = 0; i < 3; i++)
        {
            StatIncreaseButton newButton = Instantiate(_siButton, _siContainer.transform);
            RarityType rarity = _rarities[rarityIndex++];
            StatIncreaseData statIncrease = GetRandomStatIncreaseByRarity(rarity);

            if (newButton != null)
            {
                _siButtons.Add(newButton);
                newButton.Init(statIncrease.GetDescription(), statIncrease.ApplyCount, rarity);

                // Attach click event
                newButton.Button.onClick.AddListener(() =>
                {
                    statIncrease.Apply();
                    GetNextSelector();
                });
            }
        }

        int numMiniContainers = Mathf.Max(0, _excessPlayerLevel.Value - 1);
        for (int j = 0; j < numMiniContainers; j++)
        {
            GameObject newMiniSI = Instantiate(_miniSI, _miniSIContainer.transform);
            _miniSIs.Add(newMiniSI);

            for (int k = 0; k < 3; k++)
            {
                RarityPreview newRarityPreview = Instantiate(_rarityPreview, newMiniSI.transform);
                RarityType rarity = _rarities[rarityIndex++];

                if (newRarityPreview != null)
                {
                    newRarityPreview.Init(rarity);
                }
            }
        }
    }

    private void GetNextSelector()
    {
        // Destroy the current SI buttons
        foreach (var button in _siButtons)
        {
            Destroy(button.gameObject);
        }
        _siButtons.Clear();
        
        if (_miniSIs.Count == 0)
        {
            EnterGameScene();
            return;
        }

        // Get the first mini SI and extract its rarity previews
        GameObject firstMiniSI = _miniSIs[0];
        _miniSIs.RemoveAt(0);

        RarityPreview[] rarityPreviews = firstMiniSI.GetComponentsInChildren<RarityPreview>();

        if (rarityPreviews.Length == 0)
        {
            Destroy(firstMiniSI);
            return;
        }

        // Convert RarityPreviews into new SI buttons
        foreach (RarityPreview preview in rarityPreviews)
        {
            StatIncreaseButton newButton = Instantiate(_siButton, _siContainer.transform);
            RarityType rarity = preview.Rarity; // Get rarity directly from preview
            StatIncreaseData statIncrease = GetRandomStatIncreaseByRarity(rarity);

            if (newButton != null)
            {
                _siButtons.Add(newButton);
                newButton.Init(statIncrease.GetDescription(), statIncrease.ApplyCount, rarity);

                newButton.Button.onClick.AddListener(() =>
                {
                    statIncrease.Apply();
                    GetNextSelector();
                });
            }
        }

        // Remove the first Mini SI container since it's now moved to the SI container
        Destroy(firstMiniSI);
    }
    
    private RarityType RollRarity(int playerLevel)
    {
        var list = _statIncreaseChanceData.AsList<StatIncreaseChance>();
     
        int index = Mathf.Min(playerLevel, list.Count - 1);
    
        float commonRate = float.Parse(list[index].Common);
        float rareRate = float.Parse(list[index].Rare);
        float epicRate = float.Parse(list[index].Epic);
        
        float roll = Random.Range(0f, 100f);
        float threshold = 0f;
        
        if (roll < (threshold += commonRate)) return RarityType.Common;
        if (roll < (threshold += rareRate)) return RarityType.Rare;
        if (roll < (threshold += epicRate)) return RarityType.Epic;
        
        return RarityType.Legendary; // Default to Legendary if none match
    }
    
    private StatIncreaseData GetRandomStatIncreaseByRarity(RarityType rarity)
    {
        switch (rarity)
        {
            case RarityType.Legendary:
                return _legendaryStatIncreaseList.Count > 0
                    ? _legendaryStatIncreaseList[Random.Range(0, _legendaryStatIncreaseList.Count)]
                    : _epicStatIncreaseList[Random.Range(0, _epicStatIncreaseList.Count)]; //FIX
            case RarityType.Epic:
                return _epicStatIncreaseList[Random.Range(0, _epicStatIncreaseList.Count)];
            case RarityType.Rare:
                return _rareStatIncreaseList[Random.Range(0, _rareStatIncreaseList.Count)];
            case RarityType.Common:
                return _commonStatIncreaseList[Random.Range(0, _commonStatIncreaseList.Count)];
            default:
                Debug.LogWarning("Invalid rarity type. Returning Common abilities.");
                return _commonStatIncreaseList[Random.Range(0, _commonStatIncreaseList.Count)];
        }
    }

    private void EnterGameScene()
    {
        _excessPlayerLevel.Value = 0;
        GameManager.Instance.ChangeScene(_gameScene.name);
    }
}
