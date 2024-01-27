using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Dialogue
{
	public class AnswersUI : MonoBehaviour
	{
		/// <summary>
		/// Invoked when an answer is selected. The bool is whether it was correct or not.
		/// </summary>
		public static event Action<bool> OnAnswerSelected;
		private static AnswersUI instance;
		
		[SerializeField] private TextMeshProUGUI[] options;

		private bool isActive;
		private List<Answer> answers = new();

		private enum Option { A, B, C }
		
		
		// Singleton
		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			if (!isActive)
				return;
			
			if (Input.GetKeyDown(KeyCode.A))
			{
				Select(Option.A);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				Select(Option.B);
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				Select(Option.C);
			}
		}

		private void Select(Option option)
		{
			var optionUI = options[(int)option];
			// TODO Visually update optionUI
			
			OnAnswerSelected?.Invoke(answers[(int)option].IsCorrect);
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