using System.Collections;
using UnityEngine;

public class EnemyMageController : EnemyControllerBase
{
	[Header("Casting")]
	[SerializeField] private GameObject _projectilePrefab;
	[SerializeField] private Transform _shootPoint;
	[SerializeField] private float _boltSpeed;
	[SerializeField] protected float _angerRange;

	protected bool _isAngry;
	protected bool _attacking;

	protected PlayerController _player;

	protected override void Start()
	{
		base.Start();
        _player = FindObjectOfType<PlayerController>();
        StartCoroutine(ScanForPlayer());
	}

	protected override void Update()
	{
		if (_isAngry)
		{
			return;	
		}
		base.Update();
	}

	protected void Cast()
	{
		GameObject bolt = Instantiate(_projectilePrefab, _shootPoint.position, Quaternion.identity);
		bolt.GetComponent<Rigidbody2D>().velocity = transform.right * _boltSpeed;
		bolt.GetComponent<SpriteRenderer>().flipX = !_faceRight;
        Destroy(bolt, 5f);
	}

    protected virtual IEnumerator ScanForPlayer()
    {
	    while (true)
	    {
		    CheckPlayerInRange();
			yield return new WaitForSeconds(1f);
	    }
    }

    protected virtual void CheckPlayerInRange()
    {
	    if (_player == null || _attacking)
			return;

	    if (Vector2.Distance(transform.position, _player.transform.position) < _angerRange)
	    {
		    _isAngry = true;
		    TurnToPlayer();
			ChangeState(EnemyState.Cast);
	    }
	    else
	    {
		    _isAngry = false;
	    }
	}

    protected void TurnToPlayer()
    {
	    if (_player.transform.position.x - transform.position.x > 0 && !_faceRight)
	    {
		    Flip();
	    }
	    else if(_player.transform.position.x - transform.position.x < 0 && _faceRight)
	    {
		    Flip();
	    }
    }

    protected override void ChangeState(EnemyState state)
    {
	    base.ChangeState(state);
	    switch (state)
	    {
			case EnemyState.Cast:
				_attacking = true;
				_enemyRb.velocity = Vector2.zero;
				break;
	    }
    }

    protected override void EndState()
    {
	    switch (_currentState)
	    {
		    case EnemyState.Cast:
			    _attacking = false;
			    break;
	    }

	    base.EndState();
    }

    protected virtual void DoStateAction()
    {
	    switch (_currentState)
	    {
			case EnemyState.Cast:
			    Cast();
			    break;
	    }
    }

    protected override bool IsGroundEnding()
    {
	    if (_currentState == EnemyState.Cast)
	    {
		    return false;
	    }
	    return base.IsGroundEnding();
    }

    protected override void ResetState()
    {
	    base.ResetState();
	    _enemyAnimator.SetBool(EnemyState.Cast.ToString(), false);
	    _enemyAnimator.SetBool(EnemyState.Hurt.ToString(), false);
    }
}
