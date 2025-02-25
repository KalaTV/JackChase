using UnityEngine;

public class DianaAnimationController : MonoBehaviour
{
    [Header("Paramètres de l'Animator")]
    [Tooltip("Nom du paramètre entier dans l'Animator qui correspond à l'état de Diana (ex: 'State').")]
    public string stateParameter = "State";

    private Animator _animator;
    private DianaStateMachine _stateMachine;
    private DianaStateMachine.DianaState _lastState;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _stateMachine = GetComponent<DianaStateMachine>();
        _lastState = _stateMachine.currentState;
      
        _animator.SetInteger(stateParameter, (int)_lastState);
    }

    void Update()
    {
        if (_stateMachine.currentState != _lastState)
        {
            _animator.SetInteger(stateParameter, (int)_stateMachine.currentState);
            _lastState = _stateMachine.currentState;
        }
    }
}