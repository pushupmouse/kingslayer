using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform _healthBar;
    [SerializeField] private Transform _fill;
    [SerializeField] private Transform _view;

    private bool _isShown;
    
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        HideHealthBar();
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (!_isShown)
            ShowHealthBar();
        
        float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);

        Vector3 newScale = _fill.localScale;
        newScale.x = healthPercentage;
        _fill.localScale = newScale;
    }

    private void HideHealthBar()
    {
        _isShown = false;
        _view.gameObject.SetActive(_isShown);
    }

    private void ShowHealthBar()
    {
        _isShown = true;
        _view.gameObject.SetActive(_isShown);
    }
}
