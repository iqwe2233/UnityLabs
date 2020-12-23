using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class MovementController : MonoBehaviour
{
	public event Action<bool> OnGetHurt = delegate { };

    private Rigidbody2D _playerRB;
    private Animator _playerAnimator;
    private PlayerController _playerController;

    [Header("Horizontal Movement")]
    [SerializeField] private float _speed;
    [Range(0, 1)]
    [SerializeField] private float _crouchSpeedReduce;

    private bool _faceRight = true;
    private bool _canMove = true;

    [Header("Jumping")]
    [SerializeField] private float _jumpforce;
    [SerializeField] private float _radiusGC;
    [SerializeField] private bool _airControl;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _grounded;

    [Header("Crouching")]
    [SerializeField] private float _radiusCC;
    [SerializeField] private Transform _cellCheck;
    [SerializeField] private Collider2D _headCollider;
    private bool _canStand;

    [Header("Casting")]
    [SerializeField] private GameObject _bolt;
    [SerializeField] private Transform _boltPoint;
    [SerializeField] private float _boltSpeed;
    [SerializeField] private int _castMPCost;
    private bool _isCasting;

    [Header("Striking")]
    [SerializeField] private Transform _strikePoint;
    [SerializeField] public int _strikeDamage;
    [SerializeField] private float _strikeRange;
    [SerializeField] private LayerMask _enemies;
    private bool _isStriking;

    [Header("Shield Attack")]
    [SerializeField] private float _chargeTime;
    public float ChargeTime => _chargeTime;
    [SerializeField] private float _shieldAttackSpeed;
    [SerializeField] private int _shieldAttackDamage;
    [SerializeField] private Collider2D _shieldAttackZone;
    private List<EnemyControllerBase> _damagedEnemies = new List<EnemyControllerBase>();

    [SerializeField] private float _pushForce;
    private float _lastHurtTime;

    // Start is called before the first frame update
    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();

    }

    private void FixedUpdate()
    {
        if (_playerAnimator.GetBool("Hurt") && _grounded && Time.time - _lastHurtTime > 0.5f)
	    {
		    EndHurt();
	    }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, _radiusGC);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_cellCheck.position, _radiusCC);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_strikePoint.position, _strikeRange);
    }

	private void OnCollisionEnter2D(Collision2D info)
	{
        if (!_shieldAttackZone.enabled)
            return;
        
        EnemyControllerBase enemy = info.collider.GetComponent<EnemyControllerBase>();
        if (enemy == null || _damagedEnemies.Contains(enemy))
            return;

        enemy.TakeDamage(_shieldAttackDamage, DamageType.PowerStrike);
        _damagedEnemies.Add(enemy);
	}

	void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    public void Move(float move, bool jump, bool crouching)
    {
	    _grounded = Physics2D.OverlapCircle(_groundCheck.position, _radiusGC, _whatIsGround);

        if (!_canMove)
            return;

        #region Movement



        if (move != 0 && (_grounded || _airControl))
            _playerRB.velocity = new Vector2(_speed * move, _playerRB.velocity.y);

        else if (move == 0 && _grounded && !_playerAnimator.GetBool("Jump"))
        {
            _playerRB.velocity = Vector2.zero;
        }

        if (move > 0 && !_faceRight)
        {
            Flip();
        }
        else if (move < 0 && _faceRight)
        {
            Flip();
        }
        #endregion

        #region Jump
        if (jump && _grounded)
        {
            _playerRB.AddForce(Vector2.up * _jumpforce);
        }


        #endregion

        #region Crouching
        _canStand = !Physics2D.OverlapCircle(_cellCheck.position, _radiusCC, _whatIsGround);

        if (crouching)
        {
            _headCollider.enabled = false;
        }
        else if (!crouching && _canStand)
        {
            _headCollider.enabled = true;
        }
        #endregion

        #region Animation
        _playerAnimator.SetFloat("Speed", Mathf.Abs(move));
        _playerAnimator.SetBool("Jump", !_grounded);
        _playerAnimator.SetBool("Crouch", !_headCollider.enabled);
        #endregion

    }


    public void StartCasting()
    {
        if (_isCasting || _playerAnimator.GetBool("Crouch") || !_playerController.ChangeMP(-_castMPCost))
        {
            return;
        }

        _playerAnimator.SetBool("Casting", _isCasting = true);

    }

    private void CastBolt()
    {
        GameObject bolt = Instantiate(_bolt, _boltPoint.position, Quaternion.identity);
        bolt.GetComponent<Rigidbody2D>().velocity = transform.right * _boltSpeed;
        bolt.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(bolt, 5f);
    }

    private void EndCasting()
    {
        _playerAnimator.SetBool("Casting", _isCasting = false);
    }

    public void StartStriking(float holdtime)
    {
        if (_isStriking || !_canStand)
        {
            return;
        }
        if(holdtime >= _chargeTime && !_playerAnimator.GetBool("Jump") )
        {
            _playerAnimator.SetBool("ShieldAttack", _isStriking = true);
            _canMove = false;
        }
        else
        {
            _playerAnimator.SetBool("Striking", _isStriking = true);
		}

    }

    private void EndAnimation()
    {
		_playerAnimator.SetBool("Striking", _isStriking = false);
		_playerAnimator.SetBool("ShieldAttack", _isStriking = false);
		_playerAnimator.SetBool("Casting", _isCasting = false);
    }
    public void GetHurt(Vector2 position)
    {
	    _lastHurtTime = Time.time;
	    _canMove = false;
	    OnGetHurt(false);
        Vector2 pushDirection = new Vector2();
        pushDirection.x = position.x > transform.position.x ? -1 : 1;
        pushDirection.y = 1;
        _playerAnimator.SetBool("Hurt", true);
        EndAnimation();
        _playerRB.AddForce(pushDirection * _pushForce, ForceMode2D.Impulse);
    }


    private void ResetPlayer()
    {
	    _playerAnimator.SetBool("Striking", _isStriking = false);
	    _playerAnimator.SetBool("ShieldAttack", _isStriking = false);
	    _playerAnimator.SetBool("Casting", _isCasting = false);
	    _playerAnimator.SetBool("Hurt", _isCasting = false);
	    _isCasting = false;
	    _isStriking = false;
	    _canMove = true;
    }

    private void EndHurt()
    {
	    _canMove = true;
        _playerAnimator.SetBool("Hurt", false);
        OnGetHurt(true);
    }

	private void StartShieldAttackMove()
    {
        _playerRB.velocity = transform.right * _shieldAttackSpeed;
	}

    private void StartShieldAttackHurting()
    {
        _shieldAttackZone.enabled = true;
	}

    private void DisableShieldAttack()
    {
        _playerRB.velocity = Vector2.zero;
        _shieldAttackZone.enabled = false;
        _damagedEnemies.Clear();
	}

    private void EndShieldAttack()
    {
        _playerAnimator.SetBool("ShieldAttack", _isStriking = false);
        _canMove = true;
	}

    private void Strike()
    {
	    Collider2D[] enemies = Physics2D.OverlapCircleAll(_strikePoint.position, _strikeRange, _enemies);
	    foreach (Collider2D t in enemies)
	    {
		    t.GetComponent<EnemyControllerBase>().TakeDamage(_strikeDamage);
	    }
    }
    private void EndStriking()
    {
        _playerAnimator.SetBool("Striking", _isStriking = false);
    }

	
}
