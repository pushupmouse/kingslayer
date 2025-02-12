using System;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Serialization;
using Yade.Runtime;

public class StatIncreaseSelector : MonoBehaviour
{
    [Header("Stat Increases")]
    [SerializeField] private YadeSheetData _statIncreaseChanceData;
    [SerializeField] private IntVariable _currentPlayerLevel;
    [SerializeField] private List<StatIncreaseData> _statIncreaseList;
    
    [Header("UI Elements")]
    [SerializeField] private StatIncreaseSelectorButton[] _statIncreaseSelectorButtons;
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
    
    private void Awake()
    {
        _statIncreaseList.ForEach(a => a.Reset());
        
        SortAbilitiesByRarity();
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
    
    public void DisplayStatIncreaseOptions()
    {
        gameObject.SetActive(true);

        GetRandomStatIncreases();
    }
    
    private void GetRandomStatIncreases()
{
    // Step 1: Roll rarity for each button and count occurrences
    List<RarityType> rolledRarities = new List<RarityType>();
    Dictionary<RarityType, List<StatIncreaseData>> rarityPools = new Dictionary<RarityType, List<StatIncreaseData>>();

    for (int i = 0; i < _statIncreaseSelectorButtons.Length; i++)
    {
        RarityType rarity = RollRarity();
        rolledRarities.Add(rarity);

        // Only create the pool if it hasn't been created yet
        if (!rarityPools.ContainsKey(rarity))
        {
            rarityPools[rarity] = GetListByRarity(rarity);
        }
    }

    // Step 2: Select abilities based on stored rarities
    List<StatIncreaseData> selectedAbilities = new List<StatIncreaseData>();

    foreach (RarityType rarity in rolledRarities)
    {
        if (!rarityPools.ContainsKey(rarity) || rarityPools[rarity].Count == 0)
            continue; // Skip if no ability found

        var statIncreaseList = rarityPools[rarity];
        var statIncrease = statIncreaseList[UnityEngine.Random.Range(0, statIncreaseList.Count)];
        
        selectedAbilities.Add(statIncrease);
        statIncreaseList.Remove(statIncrease); // Remove ability to prevent duplicates
    }

    // Step 3: Assign abilities to buttons
    for (int i = 0; i < _statIncreaseSelectorButtons.Length; i++)
    {
        if (i >= selectedAbilities.Count) break; // Safety check

        var statIncrease = selectedAbilities[i];
        var button = _statIncreaseSelectorButtons[i];

        button.Init(statIncrease.GetDescription(), statIncrease.ApplyCount, statIncrease.GetRarityType());
        button.Button.onClick.AddListener(() =>
        {
            statIncrease.Apply();
            gameObject.SetActive(false);
        });
    }
}

private RarityType RollRarity()
{
    var list = _statIncreaseChanceData.AsList<StatIncreaseChance>();
 
    int index = Mathf.Min(_currentPlayerLevel, list.Count - 1);

    float commonRate = float.Parse(list[index].Common);
    float rareRate = float.Parse(list[index].Rare);
    float epicRate = float.Parse(list[index].Epic);
    
    float roll = UnityEngine.Random.Range(0f, 100f);
    float threshold = 0f;
    
    if (roll < (threshold += commonRate)) return RarityType.Common;
    if (roll < (threshold += rareRate)) return RarityType.Rare;
    if (roll < (threshold += epicRate)) return RarityType.Epic;
    
    return RarityType.Legendary; // Default to Legendary if none match
}

private List<StatIncreaseData> GetListByRarity(RarityType rarity)
{
    switch (rarity)
    {
        case RarityType.Common:
            return new List<StatIncreaseData>(_commonStatIncreaseList);
        case RarityType.Rare:
            return new List<StatIncreaseData>(_rareStatIncreaseList);
        case RarityType.Epic:
            return new List<StatIncreaseData>(_epicStatIncreaseList);
        case RarityType.Legendary:
            return new List<StatIncreaseData>(_legendaryStatIncreaseList);
        default:
            Debug.LogWarning("Invalid rarity type. Returning Common abilities.");
            return new List<StatIncreaseData>(_commonStatIncreaseList);
    }
}

    
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
