using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Dialogue
{
	public class AnswersUI : MonoBehaviour
	{
		private static AnswersUI Instance;
		public static event Action<bool> OnAnswerSelected;
		
		[SerializeField] private TextMeshProUGUI[] options;

		private bool isActive;
		private List<Answer> answers = new();

		private enum Option
		{
			A,B,C
		}
		
		
		// Singleton
		private void Awake()
		{
			Instance = this;
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

		private void Select(Option _option)
		{
			var optionUI = options[(int)_option];
			// TODO Update option UI
			
			OnAnswerSelected?.Invoke(answers[(int)_option].IsCorrect);
		}

		public static void Enable() => Instance.isActive = true;
		public static void Disable()
		{
			Instance.isActive = false;
			
			// Also clear answers
			for (int i = 0; i < Instance.answers.Count; i++)
			{
				Instance.options[i].text = "";
			}
		}

		public static void SetAnswers(List<Answer> _answers)
		{
			Instance.answers = _answers;
			for (int i = 0; i < Instance.answers.Count; i++)
			{
				Instance.options[i].text = Instance.answers[i].Text;
			}
		}
	}
}
