using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData _unitData;
    [SerializeField] private List<MoveData> _moves;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _actionBar;
    
    private int _currentHealth;
    private float _currentStamina;
    private float _currentActionValue;
    private Color _actionReadyColor = Color.white;
    private Color _actionNotReadyColor = new Color(1f, 1f, 1f, 0.5f);
    private SpriteRenderer _actionBarSprite;

    public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, _unitData.MaxHealth);
        }
    }

    private void Start()
    {
        _currentHealth = _unitData.MaxHealth;
        _currentStamina = 0f;
        _currentActionValue = 0f;
        _actionBarSprite = _actionBar.GetComponent<SpriteRenderer>();

        SetHealthUI();
        SetActionUI();
        SetActionUIColor();
    }
    
    public List<MoveData> GetMoveList()
    {
        return _moves;
    }

    private void SetHealthUI()
    {
        _healthBar.transform.localScale = new Vector3((float)_currentHealth / _unitData.MaxHealth, 0.2f, 1f);
    }

    private void SetActionUI()
    {
        _actionBar.transform.localScale = new Vector3(_currentActionValue / _unitData.MaxActionValue, 0.2f, 1f);
    }

    private void SetActionUIColor()
    {
        _actionBarSprite.color =
            (_currentActionValue == _unitData.MaxActionValue) ? _actionReadyColor : _actionNotReadyColor;
    }
    
    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        SetHealthUI();
    }

    public void HealHealth(int amount)
    {
        CurrentHealth += amount;
        SetHealthUI();
    }

    public void RegenStamina()
    {
        _currentStamina += _unitData.Regen * Time.deltaTime;
    }

    public void RegenAction()
    {
        if (_currentActionValue == _unitData.MaxActionValue)
            return;
        
        _currentActionValue += _unitData.Speed * Time.deltaTime;
        _currentActionValue = Mathf.Clamp(_currentActionValue, 0f, _unitData.MaxActionValue);
        SetActionUI();
        if (_currentActionValue == _unitData.MaxActionValue)
            SetActionUIColor();
    }

    public bool IsActionReady()
    {
        return _currentActionValue == _unitData.MaxActionValue;
    }

    public void ResetActionValue()
    {
        _currentActionValue = 0f;
        SetActionUI();
        SetActionUIColor();
    }

    public MoveData GetMoveData(int index)
    {
        return _moves[index];
    }

    public MoveData GetRandomMoveData()
    {
        return _moves[Random.Range(0, _moves.Count)];
    }
}
