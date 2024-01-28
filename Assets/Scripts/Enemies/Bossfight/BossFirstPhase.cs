using System.Collections;
using System.Threading;
using Code.Dialogue;
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

		[SerializeField, Tooltip("(Obsolete perche non facciamo in tempo) Spawns enemy waves when the boss HP drops below this percentage value.")] 
		[Range(0, 100)] private float spawnWaveBelowHealth = 65;
		[SerializeField] private float secondsBetweenAttacks = 5f;

		protected override IEnumerator PhaseCoroutine()
		{
			// Main sections of the boss phase
			yield return DialogueLoop();
			yield return AttackLoop();
			yield return WaveLoop();
			EndPhase();
		}

		private IEnumerator DialogueLoop()
		{
			DialogueSystem.OnAnswerChosen -= boss.ApplyModifier;
			DialogueSystem.OnAnswerChosen += boss.ApplyModifier;
			DialogueSystem.OnAnswerChosen -= OnAnswerChosen;
			DialogueSystem.OnAnswerChosen += OnAnswerChosen;
			
			// Start playing the dialogue
			dialogueSystem.Play(DialogueType.Boss1);

			// Wait until the Answer event is fired
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			bool hasAnswered = false;
			while (!hasAnswered)
			{
				yield return waitForEndOfFrame;
			}

			// Orribile, ma mi basta che funzioni a questo punto
			void OnAnswerChosen(DialogueData.BossModifier _)
			{
				hasAnswered = true;
				dialogueSystem.Stop();
			}
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
		}

		private IEnumerator WaveLoop()
		{
			// The WaveSpawner has a Start() that automatically starts the first wave
			waveSpawner.gameObject.SetActive(true);
			//yield return new WaitUntil(() => waveSpawner.AllEnemiesAreDead); // TODO
			

			yield return null;
		}

		private void OnDestroy()
		{
			DialogueSystem.OnAnswerChosen -= boss.ApplyModifier;
			StopAllCoroutines();
		}
	}
}
