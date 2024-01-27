using System.Collections;
using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	[RequireComponent(typeof(SphereCollider))]
	public class Fireball : MonoBehaviour
	{
		[SerializeField, Min(0.0001f)] private float movementSpeed = 1f;
		[SerializeField, Min(1)] private float lifetime = 20f;
		[SerializeField, Min(0)] private float damage = 1f;

		private void Start()
		{
			Move();
		}

		/// <summary>
		/// Moves the fireball in its forward direction. Dies on hit or at the end of its lifetime.
		/// </summary>
		private void Move()
		{
			StartCoroutine(MoveCoroutine());

			IEnumerator MoveCoroutine()
			{
				while (true)
				{
					Vector3 movement = transform.forward * (movementSpeed * Time.deltaTime);
					transform.Translate(movement);
					lifetime -= Time.deltaTime;
					yield return null;

					if (lifetime <= 0)
					{
						Destroy(gameObject);
					}
				}
			}
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent<PlayerHealth>(out var player))
				return;
			
			player.GetDamage(damage);
			Destroy(gameObject);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}
	}
}
