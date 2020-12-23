using UnityEngine;

public class LvlEnder : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		ServiceManager.Instance.EndLevel();
	}
}
