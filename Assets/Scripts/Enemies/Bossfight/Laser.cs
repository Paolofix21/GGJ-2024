using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	[RequireComponent(typeof(Collider))]
	public class Laser : MonoBehaviour
	{
		[SerializeField, Min(0)] private float damage = 1f;
		/// <summary>
		/// canDamage becomes false after hitting the player, so they don't take damage multiple times from the same sweep (?)
		/// </summary>
		private bool canDamage = true;
		
		private void OnTriggerEnter(Collider other)
		{
			if (!canDamage)
				return;
			
			if (!other.TryGetComponent<PlayerHealth>(out var playerHealth))
				return;
			
			playerHealth.GetDamage(damage);
			canDamage = false;
		}

	}
}
