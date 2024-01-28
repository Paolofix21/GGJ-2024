using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Dialogue
{
	public class AnswersUI : MonoBehaviour
	{
		/// <summary>
		/// Invoked when an answer is selected.
		/// </summary>
		public static event Action<Answer> OnAnswerSelected;
		private static AnswersUI instance;
		
		[SerializeField] private TextMeshProUGUI[] options;

		private bool isActive;
		private List<Answer> answers = new();
		public Answer GetRandomAnswer() => answers[Random.Range(0, answers.Count - 1)];

		private enum Option { A, B, C, D }
		
		
		// Singleton
		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			if (!isActive)
				return;
			
			// WASD -> ABCD
			if (Input.GetKeyDown(KeyCode.W))
			{
				Select(Option.A);
			}
			else if (Input.GetKeyDown(KeyCode.A))
			{
				Select(Option.B);
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				Select(Option.C);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				Select(Option.D);
			}
		}

		private void Select(Option option)
		{
			var optionUI = options[(int)option];
			// TODO Visually update optionUI
			//optionUI.GetComponent<Image>();
			
			OnAnswerSelected?.Invoke(answers[(int)option]);
		}

		public static void Enable() => instance.isActive = true;
		public static void Disable()
		{
			instance.isActive = false;
			
			// Also clear answer texts
			for (int i = 0; i < instance.answers.Count; i++)
			{
				instance.options[i].text = "";
			}
		}

		public static void SetAnswers(List<Answer> answers)
		{
			instance.answers = answers;
			for (int i = 0; i < instance.answers.Count; i++)
			{
				instance.options[i].text = instance.answers[i].Text;
			}
		}
	}
}
