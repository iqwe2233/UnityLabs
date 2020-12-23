using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MovementController))]
public class PC_InputController : MonoBehaviour
{
    MovementController _playerMovement;
    DateTime _strikeClickTime;
    float _move;
    bool _jump;
    bool _crouching;
    bool _canAttack;
    // Start is called before the first frame update
    void Start()
    {
        _playerMovement = GetComponent<MovementController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _move = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }

        _crouching = Input.GetKey(KeyCode.LeftControl);

        if(Input.GetKey(KeyCode.E))
        {
            _playerMovement.StartCasting();
		}

        //if (Input.GetButtonUp("Fire1"))
        if (!IsPointerOverUI())
        {
	        if(Input.GetKeyDown(KeyCode.Mouse0))
	        {
	            _strikeClickTime = DateTime.Now;
	            _canAttack = true;
			}

	        if (Input.GetKeyUp(KeyCode.Mouse0))
	        {
	            float holdtime = (float)(DateTime.Now - _strikeClickTime).TotalSeconds;
	            if(_canAttack)
	                _playerMovement.StartStriking(holdtime);
	            _canAttack = false;
	        }
        }

        if (_canAttack && (DateTime.Now - _strikeClickTime).TotalSeconds > _playerMovement.ChargeTime * 2)
        {
	        _playerMovement.StartStriking(_playerMovement.ChargeTime * 2);
	        _canAttack = false;
        }
    }

	private void FixedUpdate()
	{
        _playerMovement.Move(_move, _jump, _crouching);
        _jump = false;
	}

	private bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}
