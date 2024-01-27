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

		[SerializeField] private DialogueScriptableObject tutorialDialogue;
		[SerializeField] private DialogueScriptableObject bossDialogue1;
		[SerializeField] private DialogueScriptableObject bossDialogue2;
		[SerializeField] private DialogueScriptableObject bossDialogue3;
		[Space]
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private CharMap charMap;
		[SerializeField] private TextMeshProUGUI label;
		[SerializeField, Min(0)] private float delayBetweenLines = 1f;

		private int dialogueLength;
		private List<Message> currentDialogue;
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

			DialogueScriptableObject dialogueData = GetDialogue(dialogue);
			currentDialogue = dialogueData.Lines;
			dialogueLength = currentDialogue.Count;
			currentDialogueLine = 0;
			
			for (int i = 0; i < dialogueLength; i++)
			{
				await NextLine();
				currentDialogueLine++;
			}
			
			AnswersUI.SetAnswers(dialogueData.Answers);
			AnswersUI.Enable();
			DialogueTimer.StartTimer();
		}

		private async Task NextLine()
		{
			await Task.Delay((int)(delayBetweenLines * 1000), cancellationToken);
			
			// Interrupt prev dialogue
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
			cancellationToken = cancellationTokenSource.Token;

			displayTask = DialogueManager.DisplayText(currentDialogue, currentDialogueLine, label, audioSource, charMap,
				cancellationTokenSource, cancellationToken);

			await displayTask;
		}

		private void WrongAnswer() => CheckAnswer(false);
		private void CheckAnswer(bool isCorrect)
		{
			DialogueTimer.StopTimer();
			AnswersUI.Disable();
			label.text = "";
			
			// TODO what happens if the answer is correct? OnCorrectAnswer event?
		}

		/// <summary>
		/// Returns the correct <see cref="DialogueScriptableObject"/> based on the passed <see cref="Dialogue"/>.
		/// </summary>
		private DialogueScriptableObject GetDialogue(Dialogue dialogue)
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

			if (GUILayout.Button("Tutorial"))
			{
				var dialogueSystem = (DialogueSystem)target;
				dialogueSystem.Play(DialogueSystem.Dialogue.Boss1);
			}
		}
	}
#endif
}
