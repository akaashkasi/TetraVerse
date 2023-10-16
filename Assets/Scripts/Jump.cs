using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpButton;
    [SerializeField] private float jumpHeight = 9.0f;
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController _characterController;
    private Vector3 _playerVelocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerVelocity = Vector3.zero;
    }

    private void OnEnable() => jumpButton.action.performed += Jumping;

    private void OnDisable() => jumpButton.action.performed -= Jumping;

    private void Jumping(InputAction.CallbackContext context)
    {
        if (_characterController.isGrounded)
        {
            _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }

    private void Update()
    {
        if (_characterController.isGrounded && _playerVelocity.y < 0)
        {
             _playerVelocity.y = 0f;
        }

        _playerVelocity.y += gravityValue * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }
}