using UnityEngine;
using System;

public class DamageDealer : MonoBehaviour
{
	[SerializeField] private bool _oneShot;
	[SerializeField] private int _damage;
	[SerializeField] private float _timeDelay;
	private PlayerController _player;
	private DateTime _lastEncounter;
	private bool _isOnTriggerDamaged = false;

	private void OnTriggerEnter2D(Collider2D info)
	{
		_player = info.GetComponent<PlayerController>();

		if(_oneShot && _player != null)
		{
			_player.OnDeath();
		}
		
		if ((DateTime.Now - _lastEncounter).TotalSeconds < 0.01f)
			return;

		_lastEncounter = DateTime.Now;
		if(_player != null && !_isOnTriggerDamaged)
		{
			_player.TakeDamage(_damage);
			_isOnTriggerDamaged = true;
		}

	}

	private void OnTriggerExit2D(Collider2D info)
	{
		if(_player == info.GetComponent<PlayerController>())
			_player = null;
		_isOnTriggerDamaged = false;
	}

	private void Update()
	{
		if(_player != null && (DateTime.Now - _lastEncounter).TotalSeconds > _timeDelay)
		{
			_player.TakeDamage(_damage);
			_lastEncounter = DateTime.Now;
		}
	}
}
