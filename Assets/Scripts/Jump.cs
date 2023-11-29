using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpButton;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private float jumpGravityValue = -1.0f; // Gravity while jumping
    [SerializeField] private float gravityValue = -9.81f; // Regular gravity

    private CharacterController _characterController;
    private Vector3 _playerVelocity;
    private bool _isJumping;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        jumpButton.action.performed += JumpPerformed;
    }

    private void OnDisable()
    {
        jumpButton.action.performed -= JumpPerformed;
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded && !_isJumping)
        {
            _isJumping = true;
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * -1.5f * gravityValue);
        }
    }

    private void FixedUpdate()
    {
        if (_characterController.isGrounded)
        {
            if (_isJumping)
            {
                _isJumping = false;
            }
            _playerVelocity.y = 0f;
        } else {
            float appliedGravityValue = _isJumping ? jumpGravityValue : gravityValue;
            _playerVelocity.y += appliedGravityValue * Time.fixedDeltaTime;
        }

        _characterController.Move(_playerVelocity * Time.fixedDeltaTime);
    }
}
