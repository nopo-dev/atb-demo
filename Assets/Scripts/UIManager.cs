using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _moveUI;
    [SerializeField] private GameObject _unitSelector;
    [SerializeField] private GameObject _targetSelectorPrefab;
    [SerializeField] private GameObject _victoryMessage;
    [SerializeField] private GameObject _defeatMessage;

    private List<GameObject> _targetSelectors;

    private GameObject _moveListPanel;
    private GameObject _moveDetailsPanel;

    private List<GameObject> _moves;
    private List<GameObject> _moveDetails;

    private GameObject _moveEffect;
    private GameObject _moveTarget;
    private GameObject _moveTargetNumber;
    private GameObject _moveValue;
    
    private UIState _uiState;
    
    public UIState UIState
    {
        get { return _uiState; }
        set
        {
            bool alreadyActive = _moveListPanel.activeSelf;
            switch (value)
            {
                case UIState.UnitSelect:
                    if (alreadyActive)
                        SelectedMove = 0;
                    _moveListPanel.SetActive(false);
                    _moveDetailsPanel.SetActive(false);
                    break;
                case UIState.MoveSelect:
                    _moveListPanel.SetActive(true);
                    _moveDetailsPanel.SetActive(false);
                    if (!alreadyActive)
                        SelectedMove = 0;
                    break;
                case UIState.TargetSelect:
                    _moveListPanel.SetActive(true);
                    _moveDetailsPanel.SetActive(true);
                    break;
            }
            _uiState = value;
        }
    }

    private int _selectedMove;
    public int SelectedMove
    {
        get { return _selectedMove;  }
        set
        {
            _selectedMove = (value + _numMoves) % _numMoves;
            SetSelectedMove(_selectedMove);
        }
    }

    private void Start()
    {
        _moveListPanel = _moveUI.transform.GetChild(0).gameObject;
        _moveDetailsPanel = _moveUI.transform.GetChild(1).gameObject;
        
        _moves = new List<GameObject>();
        _moveDetails = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            _moves.Add(_moveListPanel.transform.GetChild(i).gameObject);
            _moveDetails.Add(_moveDetailsPanel.transform.GetChild(i).gameObject);
        }

        _targetSelectors = new List<GameObject>();
    }

    public void SetMoveUIPosition(Vector3 position)
    {
        _moveUI.transform.position = position;
    }
    
    public void SetPlayerUnitSelectorPosition(Vector3 position)
    {
        _unitSelector.transform.position = position;
    }

    private int _numMoves;

    public void SetMoveList(List<MoveData> moveList)
    {
        _numMoves = 0;
        for (int i = 0; i < 4; i++)
        {
            GameObject move = _moveListPanel.transform.GetChild(i).gameObject;
            if (i >= moveList.Count)
            {
                move.SetActive(false);
                continue;
            }

            _numMoves++;
            move.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = moveList[i].MoveName;
            move.SetActive(true);
        }
    }

    public void SetMoveDetails(MoveData moveData)
    {
        _moveDetails[0].GetComponent<TextMeshProUGUI>().text = moveData.MoveEffect.ToString();
        _moveDetails[1].GetComponent<TextMeshProUGUI>().text = moveData.MoveTarget.ToString();
        _moveDetails[2].GetComponent<TextMeshProUGUI>().text = moveData.MoveTargetNumber.ToString();
        _moveDetails[3].GetComponent<TextMeshProUGUI>().text = moveData.MoveValue.ToString();
    }

    private void SetSelectedMove(int index)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_moves[index]);
    }

    private void DestroyTargetSelectors()
    {
        for (int i = _targetSelectors.Count - 1; i >= 0; i--)
        {
            Destroy(_targetSelectors[i]);
        }
        _targetSelectors.Clear();
    }

    public void SetTargetUI(int numTargets, List<Vector3> positions)
    {
        DestroyTargetSelectors();
        for (int i = 0; i < numTargets; i++)
        {
            _targetSelectors.Add(Instantiate(_targetSelectorPrefab, positions[i], Quaternion.identity));
        }
    }

    public void MoveSingleTargetUI(Vector3 position)
    {
        _targetSelectors[0].transform.position = position;
    }

    public void ClearTargetSelectors()
    {
        DestroyTargetSelectors();
    }

    public void ShowVictoryMessage()
    {
        _victoryMessage.SetActive(true);
    }

    public void ShowDefeatMessage()
    {
        _defeatMessage.SetActive(true);
    }

    public void LogMove(Unit origin, MoveData move, List<Unit> targets)
    {
        string targetsString = targets[0].gameObject.name;
        for (int i = 1; i < targets.Count; i++)
        {
            targetsString += ", " + targets[i].gameObject.name;
        }
        Debug.Log(origin.gameObject.name + " used " + move.MoveName + " on " + targetsString);
    }

    public void HideSelector()
    {
        _unitSelector.SetActive(false);
    }
}
