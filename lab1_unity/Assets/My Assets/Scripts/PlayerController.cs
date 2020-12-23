using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	private ServiceManager _serviceManager;
	
	[Header("HP")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private int _maxHP;
    private int _currentHP;

    [Header("MP")]
    [SerializeField] private Slider _mpSlider;
    [SerializeField] private int _maxMP;
    private int _currentMP;


    private MovementController _playerMovement;

    private bool _canBeDamaged = true;

    // Start is called before the first frame update
    void Start()
    {
	    _playerMovement = GetComponent<MovementController>();
        _serviceManager = ServiceManager.Instance;
	    _playerMovement.OnGetHurt += OnGetHurt;
        _currentHP = _maxHP;
        _currentMP = _maxMP;
        _hpSlider.maxValue = _maxHP;
        _hpSlider.value = _maxHP;
        _mpSlider.maxValue = _maxHP;
        _mpSlider.value = _maxMP;
    }


    
    public void TakeDamage(int damage, DamageType type = DamageType.Casual, GameObject enemy = null)
    {
	    if (!_canBeDamaged)
	    {
		    return;
	    }
	    _currentHP -= damage;

	    if (_currentHP <= 0)
	    {
		    OnDeath();
	    }

	    switch (type)
	    {
            case DamageType.PowerStrike:
                _playerMovement.GetHurt(enemy.transform.position);
                break;
            case DamageType.ManaDrain:
	            ChangeMP(-enemy.GetComponent<EnemyProjectileController>()._howManyManaToDrain);
                break;
	    }

	    _hpSlider.value = _currentHP;

    }

    private void OnGetHurt(bool canBeDamaged)
    {
	    _canBeDamaged = canBeDamaged;
    }

    public void RestoreHP(int hp)
    {
	    _currentHP += hp;
	    if (_currentHP > _maxHP)
	    {
		    _currentHP = _maxHP;
	    }

	    _hpSlider.value = _currentHP;
    }

    public bool ChangeMP(int value)
    {

        if (value < 0 && _currentMP < Mathf.Abs(value))
            return false;

        _currentMP += value;
        if (_currentMP > _maxMP)
            _currentMP = _maxMP;

        _mpSlider.value = _currentMP;

        return true;
	}


    public void OnDeath()
    {
        _serviceManager.Restart();
	}
}
