using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputAction _submitAction;
    private InputAction _cancelAction;
    private InputAction _navigateUpAction;
    private InputAction _navigateDownAction;
    
    [SerializeField] private BattleManager _battleManager;

    private void Start()
    {
        _submitAction = InputSystem.actions.FindAction("Submit");
        _cancelAction = InputSystem.actions.FindAction("Cancel");
        _navigateUpAction = InputSystem.actions.FindAction("NavigateUp");
        _navigateDownAction = InputSystem.actions.FindAction("NavigateDown");
    }

    private void Update()
    {
        if (_submitAction.WasPressedThisFrame())
            _battleManager.Select();

        if (_cancelAction.WasPressedThisFrame())
            _battleManager.Cancel();

        if (_navigateUpAction.WasPressedThisFrame())
            _battleManager.NavigateUp();

        if (_navigateDownAction.WasPressedThisFrame())
            _battleManager.NavigateDown();
    }
}
