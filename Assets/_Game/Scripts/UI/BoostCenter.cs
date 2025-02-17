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
    
    private List<RarityType> _rarities = new List<RarityType>(); 
    private List<StatIncreaseButton> _siButtons = new List<StatIncreaseButton>(); 
    private List<GameObject> _miniSIs = new List<GameObject>();
    private int _effectiveLevel;
    private int _numMiniContainers;
    private int _numSIButtons = 3;
    private int _numMiniPreviewsPerContainer = 3;

    private class StatIncreaseChance
    {
        [DataField(0)] public string Level;
        [DataField(1)] public string Common;
        [DataField(2)] public string Rare;
        [DataField(3)] public string Epic;
        [DataField(4)] public string Legendary;
    }
    
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
        ClearStatIncreaseLists();

        foreach (var statIncrease in _statIncreaseList)
        {
            RarityType rarity = statIncrease.GetRarityType();
            AddToRarityList(rarity, statIncrease);
        }
    }

    private void ClearStatIncreaseLists()
    {
        _commonStatIncreaseList.Clear();
        _rareStatIncreaseList.Clear();
        _epicStatIncreaseList.Clear();
        _legendaryStatIncreaseList.Clear();
    }

    private void AddToRarityList(RarityType rarity, StatIncreaseData statIncrease)
    {
        switch (rarity)
        {
            case RarityType.Common:
                _commonStatIncreaseList.Add(statIncrease);
                break;
            case RarityType.Rare:
                _rareStatIncreaseList.Add(statIncrease);
                break;
            case RarityType.Epic:
                _epicStatIncreaseList.Add(statIncrease);
                break;
            case RarityType.Legendary:
                _legendaryStatIncreaseList.Add(statIncrease);
                break;
            default:
                Debug.LogWarning("Something went wrong");
                break;
        }
    }

    private void DetermineAllRarities()
    {
        for (int i = 0; i < _numSIButtons; i++)
        {
            _rarities.Add(RollRarity(_effectiveLevel));
        }

        for (int j = 0; j < _numMiniContainers; j++)
        {
            int levelForThisContainer = _effectiveLevel + j + 1;
            for (int k = 0; k < _numMiniPreviewsPerContainer; k++)
            {
                _rarities.Add(RollRarity(levelForThisContainer));
            }
        }
    }

    private void InstantiateUIElements()
    {
        int rarityIndex = 0;

        InstantiateStatIncreaseButtons(rarityIndex);
        InstantiateMiniContainers(rarityIndex);
    }

    private void InstantiateStatIncreaseButtons(int rarityIndex)
    {
        for (int i = 0; i < 3; i++)
        {
            StatIncreaseButton newButton = Instantiate(_siButton, _siContainer.transform);
            RarityType rarity = _rarities[rarityIndex++];
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
    }

    private void InstantiateMiniContainers(int rarityIndex)
    {
        for (int j = 0; j < _numMiniContainers; j++)
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
        ClearStatIncreaseLists();
        SortAbilitiesByRarity();

        DestroyButtons();
        
        if (_miniSIs.Count == 0)
        {
            EnterGameScene();
            return;
        }

        GameObject firstMiniSI = _miniSIs[0];
        _miniSIs.RemoveAt(0);

        RarityPreview[] rarityPreviews = firstMiniSI.GetComponentsInChildren<RarityPreview>();

        if (rarityPreviews.Length == 0)
        {
            Destroy(firstMiniSI);
            return;
        }

        InstantiateButtonsFromRarityPreviews(rarityPreviews);

        Destroy(firstMiniSI);
    }

    private void DestroyButtons()
    {
        foreach (var button in _siButtons)
        {
            Destroy(button.gameObject);
        }
        _siButtons.Clear();
    }

    private void InstantiateButtonsFromRarityPreviews(RarityPreview[] rarityPreviews)
    {
        foreach (RarityPreview preview in rarityPreviews)
        {
            StatIncreaseButton newButton = Instantiate(_siButton, _siContainer.transform);
            RarityType rarity = preview.Rarity;
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

        return RarityType.Legendary; 
    }

    private StatIncreaseData GetRandomStatIncreaseByRarity(RarityType rarity)
    {
        StatIncreaseData selectedStatIncrease = null;
        
        switch (rarity)
        {
            case RarityType.Legendary:
                selectedStatIncrease = GetAndRemoveStatIncrease(_legendaryStatIncreaseList, _epicStatIncreaseList);//fix
                break;

            case RarityType.Epic:
                selectedStatIncrease = GetAndRemoveStatIncrease(_epicStatIncreaseList);
                break;

            case RarityType.Rare:
                selectedStatIncrease = GetAndRemoveStatIncrease(_rareStatIncreaseList);
                break;

            case RarityType.Common:
                selectedStatIncrease = GetAndRemoveStatIncrease(_commonStatIncreaseList);
                break;

            default:
                Debug.LogWarning("Invalid rarity type. Returning Common abilities.");
                selectedStatIncrease = GetAndRemoveStatIncrease(_commonStatIncreaseList);
                break;
        }

        return selectedStatIncrease;
    }

    private StatIncreaseData GetAndRemoveStatIncrease(List<StatIncreaseData> list, List<StatIncreaseData> fallbackList = null)
    {
        if (list.Count > 0)
        {
            StatIncreaseData selectedStatIncrease = list[Random.Range(0, list.Count)];
            list.Remove(selectedStatIncrease);
            return selectedStatIncrease;
        }
        //fix
        return fallbackList != null && fallbackList.Count > 0
            ? fallbackList[Random.Range(0, fallbackList.Count)]
            : null;
    }

    private void EnterGameScene()
    {
        _excessPlayerLevel.Value = 0;
        GameManager.Instance.GoToNextRound();
    }
}
