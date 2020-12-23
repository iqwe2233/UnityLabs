using System.Collections;
using UnityEngine;

public class EnemyBossController : EnemyMageController
{
	[Header("Strike")] [SerializeField] private Transform _strikePoint;
	[SerializeField] private int _damage;
	[SerializeField] private float _strikeRange;
	[SerializeField] private LayerMask _enemies;

	[Header("PowerStrike")] [SerializeField]
	private Collider2D _strikeCollider;
	[SerializeField] private int _powerStrikeDamage;
	[SerializeField] private float _powerStrikeRange;
	[SerializeField] private float _powerStrikeSpeed;

	[Header("Transition")] [SerializeField]
	private float _waitTime;

	private float _currentStrikeRange;
	private bool _fightStarted;
	private bool _inRage;

	private EnemyState _stateOnHold;
	private EnemyState[] _attackStates = {EnemyState.Strike , EnemyState.PowerStrike, EnemyState.Cast};


	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (_currentState == EnemyState.Move && _attacking)
		{
			TurnToPlayer();
			if (CanAttack())
				ChangeState(_stateOnHold);
		}
	}

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_strikePoint.position, new Vector3(_strikeRange, _strikeRange, 0));
	}

	public override void TakeDamage(int damage, DamageType type = DamageType.Casual, Transform player = null)
	{
		if (_currentState == EnemyState.PowerStrike && type != DamageType.Projectile || _currentState == EnemyState.Hurt)
			return;

		base.TakeDamage(damage, type, player);

		if (_currentHp <= _maxHp / 2 && !_inRage)
		{
			_inRage = true;
			ChangeState(EnemyState.Hurt);
		}
	}

	protected override void OnCollisionEnter2D(Collision2D info)
	{
		if (_strikeCollider.enabled)
		{
			PlayerController player = info.collider.GetComponent<PlayerController>();
			if (player != null)
			{

				player.TakeDamage(_powerStrikeDamage, DamageType.PowerStrike, gameObject);
				_lastDamageTime = Time.time;
			}
			return;
		}
		base.OnCollisionEnter2D(info);
	}

	protected void Strike()
	{
		Collider2D player = Physics2D.OverlapBox(_strikePoint.position, new Vector2(_strikeRange, _strikeRange), 0, _enemies);
		if (player != null)
		{
			PlayerController playerController = player.GetComponent<PlayerController>();
			int damage = _inRage ? _damage * 2 : _damage;
			if (playerController != null)
				playerController.TakeDamage(damage);
		}
	}

	protected void StrikeWithPower()
	{
		_strikeCollider.enabled = true;
		_enemyRb.velocity = transform.right * _powerStrikeSpeed;
	}

	protected void EndPowerStrike()
	{
		_strikeCollider.enabled = false;
		_enemyRb.velocity = Vector2.zero;
	}

	protected override void TryToDamage(Collider2D enemy)
	{
		if (_currentState == EnemyState.Idle || _currentState == EnemyState.Cast)
		{
			return;
		}
		base.TryToDamage(enemy);
	}

	protected override void CheckPlayerInRange()
	{
		if (_player == null || _isAngry)
            return;

        if (Vector2.Distance(transform.position, _player.transform.position) < _angerRange)
        {
            _isAngry = true;
            if (!_fightStarted)
            {
                StopAllCoroutines();
                StartCoroutine(BeginNewCircle());
            }
        }
        else
            _isAngry = false;
	}

	protected override void ChangeState(EnemyState state)
	{

		base.ChangeState(state);

		switch (_currentState)
		{
			case EnemyState.PowerStrike:
			case EnemyState.Strike:
				_attacking = true;
				_currentStrikeRange = state == EnemyState.Strike ? _strikeRange : _powerStrikeRange;
				_enemyRb.velocity = Vector2.zero;
				if (!CanAttack())
				{
					_stateOnHold = state;
					_enemyAnimator.SetBool(_currentState.ToString(), false);
					ChangeState(EnemyState.Move);
				}
				break;
			case EnemyState.Hurt:
				_attacking = false;
				StopAllCoroutines();
				break;

		}
	}

	private bool CanAttack()
	{
		return Vector2.Distance(transform.position, _player.transform.position) < _currentStrikeRange;
	}

	protected override void DoStateAction()
	{
		base.DoStateAction();
		switch (_currentState)
		{
			case EnemyState.Strike:
				Strike();
				break;
			case EnemyState.PowerStrike:
				StrikeWithPower();
				break;
		}
	}

	protected override void EndState()
	{
		switch (_currentState)
		{
			case EnemyState.PowerStrike:
				EndPowerStrike();
				_attacking = false;
				break;
			case EnemyState.Strike:
				_attacking = false;
				break;
			case EnemyState.Hurt:
				if (_inRage)
				{
					_enemyAnimator.SetBool("Rage", true);
				}
				_fightStarted = false;
				break;
		}

		base.EndState();

		if (_currentState == EnemyState.Cast || _currentState == EnemyState.PowerStrike || _currentState == EnemyState.Strike || _currentState == EnemyState.Hurt)
		{
			StartCoroutine(BeginNewCircle());
		}
	}

	private IEnumerator BeginNewCircle()
	{
		if (_currentState == EnemyState.Death)
			yield break;

		if (_fightStarted)
		{
			_isAngry = false;
			CheckPlayerInRange();
			if (!_isAngry)
			{
				_fightStarted = false;
				StartCoroutine(ScanForPlayer());
				yield break;
			}
			yield return new WaitForSeconds(_waitTime);
		}
		_fightStarted = true;
		TurnToPlayer();
		ChooseNextAttackState();
	}

	protected void ChooseNextAttackState()
	{
		int state = Random.Range(0, _attackStates.Length);
		ChangeState(_attackStates[state]);
	}

	protected override void ResetState()
	{
		base.ResetState();
		_enemyAnimator.SetBool(EnemyState.PowerStrike.ToString(), false);
		_enemyAnimator.SetBool(EnemyState.Strike.ToString(), false);
	}
}
