using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Advepa.SchoolMetaverse.Laboratori;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Dialogue
{
	/// <summary>
	/// Handles the Dialogue system on the UI. Call <see cref="Play(Dialogue)"/> to start playing the passed dialogue sequence.
	/// </summary>
	public class DialogueSystem : MonoBehaviour
	{
		public enum Dialogue
		{
			Tutorial,
			Boss1,Boss2,Boss3
		}

		/// <summary>
		/// Invoked when a correct dialogue answer is chosen.
		/// </summary>
		public static event Action OnCorrectAnswer;
		/// <summary>
		/// Invoked when a wrong dialogue answer is chosen.
		/// </summary>
		public static event Action OnWrongAnswer;
		
		[SerializeField] private DialogueData tutorialDialogue;
		[SerializeField] private DialogueData bossDialogue1;
		[SerializeField] private DialogueData bossDialogue2;
		[SerializeField] private DialogueData bossDialogue3;
		[Space]
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private CharMap charMap;
		[SerializeField] private TextMeshProUGUI label;
		[SerializeField, Min(0)] private float delayBetweenLines = 1f;

		private DialogueData currentDialogueData;
		private int currentDialogueLine;
		private CancellationTokenSource cancellationTokenSource;
		private CancellationToken cancellationToken;
		private Task displayTask;

		
		private void Awake()
		{
			AnswersUI.OnAnswerSelected += CheckAnswer;
			DialogueTimer.OnTimeout += WrongAnswer;
		}

		/// <summary>
		/// Starts playing the passed dialogue.
		/// </summary>
		public async void Play(Dialogue dialogue)
		{
			// Cancel prev task if it was active
			if (displayTask is { IsCompleted: false })
				cancellationTokenSource.Cancel();

			currentDialogueData = GetDialogue(dialogue);
			currentDialogueLine = 0;
			
			for (int i = 0; i < currentDialogueData.Lines.Count; i++)
			{
				await NextLine();
				currentDialogueLine++;
			}

			// If the dialogue has answers, show them
			if (currentDialogueData.Answers.Count > 0)
			{
				AnswersUI.SetAnswers(currentDialogueData.Answers);
				AnswersUI.Enable();
				DialogueTimer.StartTimer();
			}
		}

		private async Task NextLine()
		{
			await Task.Delay((int)(delayBetweenLines * 1000), cancellationToken);
			
			// Interrupt prev dialogue
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
			cancellationToken = cancellationTokenSource.Token;

			await DisplayLine(currentDialogueData.Lines);
		}

		private async Task DisplayLine(List<Message> lines)
		{
			displayTask = DialogueManager.DisplayText(lines, currentDialogueLine, label, audioSource, charMap, cancellationTokenSource, cancellationToken);
			await displayTask;
		}

		private void WrongAnswer() => CheckAnswer(false);
		private async void CheckAnswer(bool isCorrect)
		{
			DialogueTimer.StopTimer();
			AnswersUI.Disable();
			label.text = "";
			
			// Display the final line based on the answer result
			var finalLine = new List<Message>{ isCorrect ? currentDialogueData.Bonus : currentDialogueData.Malus };
			displayTask = DialogueManager.DisplayText(finalLine, 0, label, audioSource, charMap, cancellationTokenSource, cancellationToken);
			await displayTask;

			label.text = "";

			// TODO close the dialogue? Or does another system take care of that?
			
			if (isCorrect)
			{
				OnCorrectAnswer?.Invoke();;
			}
			else
			{
				OnWrongAnswer?.Invoke();
			}
		}

		/// <summary>
		/// Returns the correct <see cref="DialogueData"/> based on the passed <see cref="Dialogue"/>.
		/// </summary>
		private DialogueData GetDialogue(Dialogue dialogue)
		{
			return dialogue switch
			{
				Dialogue.Tutorial => tutorialDialogue,
				Dialogue.Boss1 => bossDialogue1,
				Dialogue.Boss2 => bossDialogue2,
				Dialogue.Boss3 => bossDialogue3,
				_ => null
			};
		}
		
		// Get/Clear the char map is necessary because...
		private void Start()
		{
			GetMapInfo();
		}

		private void OnDestroy()
		{
			ClearMapInfo();
		}
		
		private void GetMapInfo()
		{
			for (int i = 0; i < charMap.DialogueChars.Count; i++)
			{
				charMap.mappedInfo.Add(charMap.DialogueChars[i].Letter, charMap.DialogueChars[i].CharClip);
			}
		}

		private void ClearMapInfo()
		{
			cancellationTokenSource?.Cancel();
			charMap.mappedInfo.Clear();
		}
	}

#if UNITY_EDITOR // TEMP to test
	[CustomEditor(typeof(DialogueSystem))]
	public class DialogueSystemCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			if (!Application.isPlaying)
				return;
			
			if (GUILayout.Button("Boss Phase 1"))
			{
				((DialogueSystem)target).Play(DialogueSystem.Dialogue.Boss1);
			}
			else if (GUILayout.Button("Boss Phase 2"))
			{
				((DialogueSystem)target).Play(DialogueSystem.Dialogue.Boss2);
			}
			else if (GUILayout.Button("Boss Phase 3"))
			{
				((DialogueSystem)target).Play(DialogueSystem.Dialogue.Boss3);
			}
		}
	}
#endif
}
