using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Dialogue
{
	/// <summary>
	/// Handles the timer during the dialogue QTE.
	/// </summary>
	public class DialogueTimer : MonoBehaviour
	{
		/// <summary>
		/// Invoked when the dialogue timer runs out before the player chooses anything.
		/// </summary>
		public static event Action OnTimeout;
		private static DialogueTimer instance; // Singleton

		[SerializeField] private Image timerBar;
		[Space]
		[SerializeField, Min(0), Tooltip("How many seconds the player has to choose an answer.")]
		private float timePerQuestion = 5f;
		private float timeLeft;


		private void Awake() => instance = this;

		/// <summary>
		/// Starts the dialogue timer. If it runs out, <see cref="OnTimeout"/> is called.
		/// </summary>
		public static void StartTimer()
		{
			instance.StartCoroutine(instance.TimerCoroutine());
		}

		public static void StopTimer()
		{
			instance.StopAllCoroutines();
			instance.timerBar.gameObject.SetActive(false);
		}

		private IEnumerator TimerCoroutine()
		{
			timerBar.gameObject.SetActive(true);
			timerBar.fillAmount = 1;
			timeLeft = timePerQuestion;
			
			while (timeLeft > 0)
			{
				timeLeft -= Time.deltaTime;
				timerBar.fillAmount = timeLeft / timePerQuestion;
				yield return null;
			}
			
			timerBar.fillAmount = 0;
			OnTimeout?.Invoke();
		}
	}
}
