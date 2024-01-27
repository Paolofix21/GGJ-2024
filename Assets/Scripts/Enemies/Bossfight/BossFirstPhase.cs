using System.Threading.Tasks;
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
		private float spawnWaveBelowHealth = 65;

		[SerializeField] private float secondsBetweenSpheres = 5f;

		private BossAttackSpheresEven evenSpheresAttack;
		private BossAttackSpheresOdd oddSpheresAttack;


		
		public override async void StartPhase()
		{
			evenSpheresAttack = GetComponent<BossAttackSpheresEven>();
			oddSpheresAttack = GetComponent<BossAttackSpheresOdd>();
			
			// dialogueResult should be an enum with 4 types of answers
			// TODO
			//var dialogueResult = await DialogueSystem.Dialogue();
			//boss.ApplyMalusOrBonus(dialogueResult);
			
			await AttackLoop();
			// When HP drops below 65%, attacks stop and enemy waves are spawned
			await WaveLoop();
		}

		/// <summary>
		/// Starts shooting a series of bullet-hell spheres. Returns when the boss HP reaches <see cref="spawnWaveBelowHealth"/>.
		/// </summary>
		private async Task AttackLoop()
		{
			while (boss.HeathAsPercentage <= spawnWaveBelowHealth)
			{
				await evenSpheresAttack.Shoot();

				if (boss.HeathAsPercentage > spawnWaveBelowHealth)
					return;

				await Task.Delay((int)(secondsBetweenSpheres * 1000));

				if (boss.HeathAsPercentage > spawnWaveBelowHealth)
					return;

				
				await oddSpheresAttack.Shoot();

				if (boss.HeathAsPercentage > spawnWaveBelowHealth)
					return;

				await Task.Delay((int)(secondsBetweenSpheres * 1000));
			}
		}
		
		private async Task WaveLoop()
		{
			// Spawn enemy wave
			// When the wave ends, go to Phase 2
		}

		
	}
}
