using System.Collections;
using Code.Dialogue;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossSecondPhase : BossPhase
	{
		/* Seconda fase: Tira delle sventagliate da destra verso sinistra
		 e viceversa per 5 metri di lunghezza e larghezza. Dopo 4-5 secondi
		  da questa azione, ripete i la fase 1.
		*/

		[SerializeField, Tooltip("Spawns enemy waves when the boss HP drops below this percentage value.")] 
		[Range(0, 100)] private float spawnWaveBelowHealth = 35;
		[SerializeField] private float secondsBetweenAttacks = 5f;

		
		protected override IEnumerator PhaseCoroutine()
		{
			// Main sections of the boss phase
			yield return DialogueLoop();
			yield return AttackLoop();
			//yield return WaveLoop();
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
				yield return laserAttack.FireLeft();
				yield return attackDelay;

				if (boss.HeathAsPercentage < spawnWaveBelowHealth)
					break;

				yield return laserAttack.FireRight();
				yield return attackDelay;
				
				if (boss.HeathAsPercentage < spawnWaveBelowHealth)
					break;
				
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
