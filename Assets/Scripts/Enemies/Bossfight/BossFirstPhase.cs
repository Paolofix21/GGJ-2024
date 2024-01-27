using System;
using System.Threading;
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
		[Range(0, 100)] private float spawnWaveBelowHealth = 65;
		[SerializeField] private float secondsBetweenAttacks = 5f;

		private BossAttackSpheresEven evenSpheresAttack;
		private BossAttackSpheresOdd oddSpheresAttack;
		
		private CancellationTokenSource tokenSource;
		
		
		public override async void StartPhase()
		{
			evenSpheresAttack = GetComponent<BossAttackSpheresEven>();
			oddSpheresAttack = GetComponent<BossAttackSpheresOdd>();

			// TODO Force-turn player towards boss?
			
			// dialogueResult should be an enum with 4 types of answers
			// TODO
			//var dialogueResult = await DialogueSystem.Dialogue();
			//boss.ApplyMalusOrBonus(dialogueResult);

			tokenSource = new CancellationTokenSource();
			Task attackTask = AttackLoop(tokenSource.Token);

			await attackTask;
			print("attack has ended");
			// When HP drops below 65%, attacks stop and enemy waves are spawned

			Task waveTask = WaveLoop(tokenSource.Token);
			await waveTask;
			print("wave has ended");
		}

		/// <summary>
		/// Starts shooting a series of bullet-hell spheres. Returns when the boss HP reaches <see cref="spawnWaveBelowHealth"/>.
		/// </summary>
		private async Task AttackLoop(CancellationToken token)
		{
			while (!token.IsCancellationRequested && boss.HeathAsPercentage >= spawnWaveBelowHealth)
			{
				await evenSpheresAttack.Shoot();

				if (token.IsCancellationRequested || boss.HeathAsPercentage < spawnWaveBelowHealth)
					return;

				await Task.Delay((int)(secondsBetweenAttacks * 1000), token);

				if (token.IsCancellationRequested || boss.HeathAsPercentage < spawnWaveBelowHealth)
					return;

				
				await oddSpheresAttack.Shoot();

				if (token.IsCancellationRequested || boss.HeathAsPercentage < spawnWaveBelowHealth)
					return;

				await Task.Delay((int)(secondsBetweenAttacks * 1000), token);
			}
		}
		
		private async Task WaveLoop(CancellationToken token)
		{
			// Spawn enemy wave
			// When the wave ends, go to Phase 2
		}


		private void OnDestroy()
		{
			tokenSource.Cancel();
		}
	}
}
