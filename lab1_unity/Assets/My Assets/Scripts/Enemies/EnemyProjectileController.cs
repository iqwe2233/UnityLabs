using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
	[SerializeField] private int _damage;
	[SerializeField] private DamageType _projectileDamageType;

	[SerializeField] public int _howManyManaToDrain;
	private float _lastEncounter;
	private void OnTriggerEnter2D(Collider2D info)
	{
		if (info.GetComponent<EnemyMageController>() != null || info.GetComponent<EnemyBossController>() != null)
		{
			return;
		}

		if (Time.time - _lastEncounter < 0.2f)
			return;

		_lastEncounter = Time.time;
		PlayerController player = info.GetComponent<PlayerController>();

		if (player != null)
			player.TakeDamage(_damage, _projectileDamageType, gameObject);

		
		Destroy(gameObject);
	}


}
