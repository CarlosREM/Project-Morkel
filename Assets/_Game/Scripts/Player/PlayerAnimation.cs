using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject playerRoot;

    private PlayerControl _playerControl;
    private CharacterHealth _playerHealth;
    private Animator _animator;

    private string _currentState;

    private bool _animationDone;
    public void SetAnimDone() => _animationDone = true;
    
    private void Awake()
    {
        _playerControl = playerRoot.GetComponent<PlayerControl>();
        Assert.IsNotNull(_playerControl, "Could not find PlayerControl component on root");
        _playerHealth = playerRoot.GetComponent<CharacterHealth>();
        Assert.IsNotNull(_playerHealth, "Could not find CharacterHealth component on root");
        
        _animator = GetComponent<Animator>();
        _currentState = "Idle";
    }

    private void OnEnable()
    {
        _playerHealth.OnHurt += OnHurt;
        _playerHealth.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        _playerHealth.OnHurt -= OnHurt;
        _playerHealth.OnDeath -= OnDeath;
    }

    private void Update()
    {
        LookDirection();
        
        AnimationStateLoop();
    }

    private void LookDirection()
    {
        if (_playerControl.MoveInput != 0)
        {
            var rotation = transform.rotation;
            var lookRotation = (_playerControl.IsFacingRight) ? 0f : 180f;
            transform.rotation = Quaternion.Euler(0, lookRotation, rotation.eulerAngles.z);
        }
    }
    
    private void AnimationStateLoop()
    {
                switch (_currentState)
        {
            case "Idle":
            {
                if (AirCheck())
                    break;
                if (_playerControl.IsCrouching)
                {
                    SetAnimatorState("Crouch");
                    break;
                }

                if (_playerControl.MoveInput != 0)
                    SetAnimatorState("Move");

                break;
            }
            case "Move":
            {
                if (AirCheck())
                    break;
                
                if (_playerControl.IsCrouching)
                {
                    SetAnimatorState("Crouch Move");
                    break;
                }
                
                if (_playerControl.MoveInput == 0)
                    SetAnimatorState("Idle");
                
                break;
            }
            case "Air Up":
            {
                AirCheck();
                break;
            }
            case "Air Down":
            {
                if (AirCheck())
                    break;
                if (_playerControl.IsGrounded)
                    SetAnimatorState("Idle");
                break;
            }
            case "Crouch":
            {
                if (!_playerControl.IsCrouching)
                    SetAnimatorState("Idle");

                else if (_playerControl.MoveInput != 0)
                    SetAnimatorState("Crouch Move");
                break;
            }
            case "Crouch Move":
            {
                if (AirCheck())
                    break;
                
                if (!_playerControl.IsCrouching)
                {
                    SetAnimatorState("Move");
                    break;
                }
                
                if (_playerControl.MoveInput == 0)
                    SetAnimatorState("Crouch");
                
                break;
            }
            case "Hurt":
            {
                if (_animationDone)
                    SetAnimatorState((_playerControl.IsGrounded) ? "Idle" : "Air Down");
                break;
            }
            case "Dead":
            {
                break;
            }
        }
    }

    private void SetAnimatorState(string state, bool force = false)
    {
        if (_currentState == state && !force)
            return;

        try
        {
            _animator.Play(state);
            _currentState = state;
            _animationDone = false;
        }
        catch
        {
            Debug.LogWarning($"No animation state {state} was found!");
        }
    }

    private bool AirCheck()
    {
        if (_playerControl.VelocityY > 0)
        {
            SetAnimatorState("Air Up");
            return true;
        }

        if (_playerControl.VelocityY < 0)
        {
            SetAnimatorState("Air Down");
            return true;
        }

        return false;
    }
    
    private void OnHurt(int obj)
    {
        SetAnimatorState("Hurt", true);
    }
    private void OnDeath()
    {
        SetAnimatorState("Dead", true);
    }

}