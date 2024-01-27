using System.Collections;
using System.Threading;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossFirstPhase : BossPhase
	{
		/* Dal Vangelo DGG secondo Sebastiano
		Prima fase: il boss lancia delle sfere (o dei raggi se viene più comodo ai programmatori) 
		in stile bullet hell prima in una direzione e poi in un’altra. Le file sono due: 
		La parte blu è la prima parte di raggi, che durano 4-5 secondi, dopodiché si interrompono 
		per far partire quelli verdi, i quali durano 4-5 secondi, per poi tornare a quelli blu. 
		Così fino a che non cambia la fase.
		
		Se il boss scende da 100% HP a 65%, gli attacchi finiscono e spawnano altre wave
		a fine wave, la fase è finita
		*/

		[SerializeField, Tooltip("Spawns enemy waves when the boss HP drops below this percentage value.")] 
		[Range(0, 100)] private float spawnWaveBelowHealth = 65;
		[SerializeField] private float secondsBetweenAttacks = 5f;

		private BossAttackSpheresEven evenSpheresAttack;
		private BossAttackSpheresOdd oddSpheresAttack;
		
		private CancellationTokenSource tokenSource;
		
		
		public override void StartPhase()
		{
			evenSpheresAttack = GetComponent<BossAttackSpheresEven>();
			oddSpheresAttack = GetComponent<BossAttackSpheresOdd>();

			StartCoroutine(AttackLoop());
		}

		private IEnumerator AttackLoop()
		{
			WaitForSeconds attackDelay = new WaitForSeconds(secondsBetweenAttacks);

			// Volevo farlo con le task ma alla fine è piu comodo con le Coroutine
			while (boss.HeathAsPercentage >= spawnWaveBelowHealth)
			{
				yield return evenSpheresAttack.Shoot();
				yield return attackDelay;

				if (boss.HeathAsPercentage < spawnWaveBelowHealth)
					break;

				yield return oddSpheresAttack.Shoot();
				yield return attackDelay;
			}

			print("attack has ended");
			StartCoroutine(WaveLoop());
		}

		private IEnumerator WaveLoop()
		{
			// Spawn enemy wave
			// When the wave ends, go to Phase 2

			yield return null; // Placeholder for spawning enemy wave

			print("wave has ended");

			// Go to Phase 2 or perform other actions as needed
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}
	}
}
