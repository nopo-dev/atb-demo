using UnityEngine;
using System.Collections.Generic;

public enum BattleState
{
    Active, Paused, Finished
}

public enum UIState
{
    UnitSelect, MoveSelect, TargetSelect
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private List<Unit> _enemyUnits;
    [SerializeField] private UIManager _uiManager;
    
    private BattleState _battleState;
    private int _selectedUnit;
    private GameObject _moveListPanel;
    private GameObject _moveDetailsPanel;
    private Camera _camera;
    
    private List<Unit> _currentTargets;
    private int _currentTargetIndex;
    
    private void Start()
    {
        _camera = Camera.main;
        _currentTargets = new List<Unit>();
        _currentTargetIndex = -1;
        
        _battleState = BattleState.Active;
        _uiManager.UIState = UIState.UnitSelect;
        
        _selectedUnit = 0;
        _uiManager.SetPlayerUnitSelectorPosition(_playerUnits[_selectedUnit].transform.position);
        _uiManager.SetMoveUIPosition(_camera.WorldToScreenPoint(_playerUnits[_selectedUnit].transform.position));
    }

    private void Update()
    {
        if (_battleState == BattleState.Paused || _battleState == BattleState.Finished)
            return;
        foreach (Unit unit in _playerUnits)
        {
            unit.RegenAction();
        }
        foreach (Unit unit in _enemyUnits)
        {
            unit.RegenAction();
            if (unit.IsActionReady())
            {
                MoveData move = unit.GetRandomMoveData();
                List<Unit> targets = SelectRandomTargets(unit, move);
                PerformEnemyMove(move, targets);
                unit.ResetActionValue();
                if (_playerUnits.Count == 0)
                    EndBattle(false);
            }
        }
    }

    private void PerformEnemyMove(MoveData move, List<Unit> targets)
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            Unit unit = targets[i];
            switch (move.MoveEffect)
            {
                case Effect.Damage:
                    unit.TakeDamage(move.MoveValue);
                    if (unit.CurrentHealth == 0)
                    {
                        targets.Remove(unit);
                        _playerUnits.Remove(unit);
                        Destroy(unit.gameObject);
                    }
                    break;
                case Effect.Heal:
                    unit.HealHealth(move.MoveValue);
                    break;
            }
        }
    }

    private List<Unit> SelectRandomTargets(Unit unit, MoveData move)
    {
        List<Unit> targets = new List<Unit>();
        
        if (move.MoveTargetNumber == TargetNumber.Single)
        {
            if (move.MoveTarget == Target.Enemy)
            {
                targets.Add(_playerUnits[Random.Range(0, _playerUnits.Count)]);
            }
            else if (move.MoveTarget == Target.Ally)
            {
                targets.Add(_enemyUnits[Random.Range(0, _enemyUnits.Count)]);
            }
            else if(move.MoveTarget == Target.Self)
            {
                targets.Add(unit);
            }
        }
        else if (move.MoveTargetNumber == TargetNumber.All)
            switch (move.MoveTarget)
            {
                case Target.Enemy:
                {
                    foreach (Unit u in _playerUnits)
                    {
                        targets.Add(u);
                    }
                    break;
                }
                case Target.Ally:
                {
                    foreach (Unit u in _enemyUnits)
                    {
                        targets.Add(u);
                    }
                    break;
                }
            }
        
        return targets;
    }

    public void Select()
    {
        if (_battleState == BattleState.Finished)
            return;
        switch (_uiManager.UIState)
        {
            case UIState.UnitSelect:
                if (!_playerUnits[_selectedUnit].IsActionReady())
                    break;
                _battleState = BattleState.Paused;
                _uiManager.SetMoveList(_playerUnits[_selectedUnit].GetMoveList());
                _uiManager.UIState = UIState.MoveSelect;
                break;
            case UIState.MoveSelect:
                MoveData move = _playerUnits[_selectedUnit].GetMoveData(_uiManager.SelectedMove);
                GetTargets(move);
                _uiManager.SetMoveDetails(move);
                _uiManager.SetTargetUI(_currentTargets.Count, GetTargetPositions());
                _uiManager.UIState = UIState.TargetSelect;
                break;
            case UIState.TargetSelect:
                move = _playerUnits[_selectedUnit].GetMoveData(_uiManager.SelectedMove);
                PerformMove(move);
                _playerUnits[_selectedUnit].ResetActionValue();
                _battleState = BattleState.Active;
                _uiManager.UIState = UIState.UnitSelect;
                _uiManager.ClearTargetSelectors();
                if (_enemyUnits.Count == 0)
                    EndBattle(true);
                break;
        }
    }

    private void PerformMove(MoveData move)
    {
        for (int i = _currentTargets.Count - 1; i >= 0; i--)
        {
            Unit unit = _currentTargets[i];
            switch (move.MoveEffect)
            {
                case Effect.Damage:
                    unit.TakeDamage(move.MoveValue);
                    if (unit.CurrentHealth == 0)
                    {
                        _currentTargets.Remove(unit);
                        _enemyUnits.Remove(unit);
                        Destroy(unit.gameObject);
                    }
                    break;
                case Effect.Heal:
                    unit.HealHealth(move.MoveValue);
                    break;
            }
        }
    }

    private void EndBattle(bool playerVictory)
    {
        _battleState = BattleState.Finished;
        _uiManager.HideSelector();
        if (playerVictory)
            _uiManager.ShowVictoryMessage();
        else
            _uiManager.ShowDefeatMessage();
    }

    public void Cancel()
    {
        if (_battleState == BattleState.Finished)
            return;
        switch (_uiManager.UIState)
        {
            case UIState.MoveSelect:
                _battleState = BattleState.Active;
                _uiManager.UIState = UIState.UnitSelect;
                break;
            case UIState.TargetSelect:
                _uiManager.UIState = UIState.MoveSelect;
                _uiManager.ClearTargetSelectors();
                break;
        }
    }

    public void NavigateUp()
    {
        if (_battleState == BattleState.Finished)
            return;
        switch (_uiManager.UIState)
        {
            case UIState.UnitSelect:
                MovePlayerUnitSelector(-1);
                _uiManager.SetMoveUIPosition(_camera.WorldToScreenPoint(_playerUnits[_selectedUnit].transform.position));
                break;
            case UIState.MoveSelect:
                _uiManager.SelectedMove--;
                break;
            case UIState.TargetSelect:
                MoveData move = _playerUnits[_selectedUnit].GetMoveData(_uiManager.SelectedMove);
                MoveCurrentTarget(move, -1);
                break;
        }
    }

    public void NavigateDown()
    {
        if (_battleState == BattleState.Finished)
            return;
        switch (_uiManager.UIState)
        {
            case UIState.UnitSelect:
                MovePlayerUnitSelector(1);
                _uiManager.SetMoveUIPosition(_camera.WorldToScreenPoint(_playerUnits[_selectedUnit].transform.position));
                break;
            case UIState.MoveSelect:
                _uiManager.SelectedMove++;
                break;
            case UIState.TargetSelect:
                MoveData move = _playerUnits[_selectedUnit].GetMoveData(_uiManager.SelectedMove);
                MoveCurrentTarget(move, 1);
                break;
        }
    }

    private void MovePlayerUnitSelector(int direction)
    {
        _selectedUnit = (_selectedUnit + _playerUnits.Count + direction) % _playerUnits.Count;
        _uiManager.SetPlayerUnitSelectorPosition(_playerUnits[_selectedUnit].transform.position);
    }

    private void MoveCurrentTarget(MoveData move, int direction)
    {
        if (move.MoveTargetNumber != TargetNumber.Single || move.MoveTarget == Target.Self)
            return;
        switch (move.MoveTarget)
        {
            case Target.Ally:
                _currentTargetIndex = (_currentTargetIndex + _playerUnits.Count + direction) % _playerUnits.Count;
                _currentTargets.Clear();
                _currentTargets.Add(_playerUnits[_currentTargetIndex]);
                break;
            case Target.Enemy:
                _currentTargetIndex = (_currentTargetIndex + _enemyUnits.Count + direction) % _enemyUnits.Count;
                _currentTargets.Clear();
                _currentTargets.Add(_enemyUnits[_currentTargetIndex]);
                break;
        }
        _uiManager.MoveSingleTargetUI(_currentTargets[0].transform.position);
    }

    private void ClearCurrentTargets()
    {
        _currentTargets.Clear();
        _currentTargetIndex = -1;
    }

    private void GetTargets(MoveData move)
    {
        ClearCurrentTargets();
        
        if (move.MoveTargetNumber == TargetNumber.Single)
        {
            if (move.MoveTarget == Target.Enemy)
            {
                _currentTargets.Add(_enemyUnits[0]);
                _currentTargetIndex = 0;
            }

            if (move.MoveTarget == Target.Ally || move.MoveTarget == Target.Self)
            {
                _currentTargets.Add(_playerUnits[_selectedUnit]);
                _currentTargetIndex = _selectedUnit;
            }
        }
        else if(move.MoveTargetNumber == TargetNumber.All)
            switch (move.MoveTarget)
            {
                case Target.Ally:
                {
                    foreach (Unit unit in _playerUnits)
                    {
                        _currentTargets.Add(unit);
                    }
                    break;
                }
                case Target.Enemy:
                {
                    foreach (Unit unit in _enemyUnits)
                    {
                        _currentTargets.Add(unit);
                    }
                    break;
                }
            }
    }
    
    private List<Vector3> GetTargetPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Unit unit in _currentTargets)
        {
            positions.Add(unit.transform.position);
        }
        
        return positions;
    }
}