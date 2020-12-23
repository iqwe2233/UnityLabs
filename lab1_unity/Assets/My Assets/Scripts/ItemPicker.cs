using UnityEngine;

public class ItemPicker : MonoBehaviour
{
	[SerializeField] private int MP_HP_DMG_ChangeValue;
	private void OnTriggerEnter2D(Collider2D info)
	{
		if (WhatIsIt("HPPotion"))
		{
			info.GetComponent<PlayerController>().RestoreHP(MP_HP_DMG_ChangeValue);
		}
		else if(WhatIsIt("MPPotion"))
		{
			info.GetComponent<PlayerController>().ChangeMP(MP_HP_DMG_ChangeValue);
		}
		else if (WhatIsIt("DmgUP"))
		{
			//if (info.GetComponent<MovementController>())
			//{
				info.GetComponent<MovementController>()._strikeDamage += MP_HP_DMG_ChangeValue;
			//}
		}
		Debug.Log(info.name);
		Destroy(gameObject);
	}

	private bool WhatIsIt(string type)
	{
		return gameObject.transform.name.Contains(type);
	}
}
