using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator))]
public abstract class EnemyControllerBase : MonoBehaviour
{
	protected Rigidbody2D _enemyRb;
	protected Animator _enemyAnimator;

	[Header("Canvas")]
	[SerializeField] GameObject _canvas;
	[SerializeField] protected Slider _hpSlider;

	[Header("HP")]
	[SerializeField] protected int _maxHp;
	protected int _currentHp;

	[Header("StateChanges")]
	[SerializeField] private float _minStateTime;
	[SerializeField] private float _maxStateTime;
	[SerializeField] private EnemyState[] _availableStates;
	protected EnemyState _currentState;
	private float _lastStateChange;
	private float _timeToNextChange;


	[Header("Movement")]
	[SerializeField] private float _speed;
	[SerializeField] private float _range;
	[SerializeField] private Transform _groundCheck;
	[SerializeField] private LayerMask _whatIsGround;
	protected bool _faceRight = true;

	[Header("Damage dealer")]
	[SerializeField] private DamageType _collisionDamageType;
	[SerializeField] protected int _collisionDamage;
	[SerializeField] protected float _collisionTimeDelay;
	protected float _lastDamageTime;

	// Start is called before the first frame update
	protected virtual void Start()
    {
	    _enemyRb = GetComponent<Rigidbody2D>();
	    _enemyAnimator = GetComponent<Animator>();
	    _currentHp = _maxHp;
	    _hpSlider.maxValue = _maxHp;
	    _hpSlider.value = _maxHp;
    }

    protected virtual void FixedUpdate()
    {
	    if (_currentState == EnemyState.Death)
		    return;

		if (IsGroundEnding())
	    {
		    Flip();
	    }
		if(_currentState == EnemyState.Move)
			Move();


    }
    protected virtual void Update()
    {
	    if (_currentState == EnemyState.Death)
		    return;

		if (Time.time - _lastStateChange > _timeToNextChange)
			GetRandomState();
    }

    protected virtual void OnCollisionEnter2D(Collision2D info)
    {
	    if (_currentState == EnemyState.Death)
		    return;

		TryToDamage(info.collider);
    }

    protected virtual void TryToDamage(Collider2D enemy)
    {
	    if ((Time.time - _lastDamageTime) < _collisionTimeDelay)
	    {
			return;
	    }

	    PlayerController player = enemy.GetComponent<PlayerController>();
	    if (player != null)
	    {
			player.TakeDamage(_collisionDamage, _collisionDamageType, gameObject);
			_lastDamageTime = Time.time;
	    }
    }

	protected void Move()
    {
	    _enemyRb.velocity = transform.right * new Vector2(_speed, _enemyRb.velocity.y);
    }

    protected void Flip()
    {
	    if (_currentState == EnemyState.Death)
		    return;

		_faceRight = !_faceRight;
		transform.Rotate(0, 180, 0);
		_canvas.transform.Rotate(0,180,0);
    }

    protected virtual void OnDrawGizmos()
    {
		Gizmos.DrawWireCube(transform.position, new Vector3(_range * 2, 0.5f, 0));
    }

    public virtual void TakeDamage(int damage, DamageType type = DamageType.Casual, Transform player = null)
    {
	    if (_currentState == EnemyState.Death)
		    return;

		_currentHp -= damage;
	    _hpSlider.value = _currentHp <= 0 ? 0 : _currentHp;
	    ChangeState(EnemyState.Hurt);

		Debug.Log(String.Format("Enemy {0} take damage {1} and his current hp = {2}", gameObject, damage, _currentHp));
		if (_currentHp <= 0)
		{
		    ChangeState(EnemyState.Death);
			return;
		}


		
    }

    public virtual void OnDeath()
    {
		Destroy(gameObject);
    }

    protected virtual bool IsGroundEnding()
    {
	    return !Physics2D.OverlapPoint(_groundCheck.position, _whatIsGround);
    }

    protected void GetRandomState()
    {
	    if (_currentState == EnemyState.Death)
		    return;

		int state = Random.Range(0, _availableStates.Length);
	    //_currentState = _availableStates[state];
	    if (_currentState == EnemyState.Idle && _availableStates[state] == EnemyState.Idle)
	    {
		    GetRandomState();
	    }
	    _timeToNextChange = Random.Range(_minStateTime, _maxStateTime);
		ChangeState(_availableStates[state]);
    }

    protected virtual void ChangeState(EnemyState state)
    {
	    if (_currentState == EnemyState.Death)
		    return;

		ResetState();
		_currentState = EnemyState.Idle;

	    if (state != EnemyState.Idle)
		    _enemyAnimator.SetBool(state.ToString(), true);


	    _currentState = state;
	    _lastStateChange = Time.time;

	    switch (_currentState)
	    {
			case EnemyState.Idle:
				_enemyRb.velocity = Vector2.zero;
				break;
			case EnemyState.Hurt:
				_enemyRb.velocity = Vector2.zero;
				break;
	    }
    }

    protected virtual void EndState()
    {
	    if (_currentState == EnemyState.Death)
	    {
		    OnDeath();
	    }
		
		ResetState();
    }

    protected virtual void ResetState()
    {
		_enemyAnimator.SetBool(EnemyState.Move.ToString(), false);
		_enemyAnimator.SetBool(EnemyState.Death.ToString(), false);
		_enemyAnimator.SetBool(EnemyState.Hurt.ToString(), false);
    }

}


