using System;
using System.Collections.Generic;
using System.Linq;
using Advepa.SchoolMetaverse.Laboratori;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Dialogue
{
	[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue", order = 1)]

	public class DialogueData : ScriptableObject
	{
		public enum BossModifier
		{
			TakeDoubleDamage,
			TakeHalfDamage,
			DealsMoreDamage,
			Heal
		}

		[Tooltip("Standard displays one line after the other. Randomize chooses one line at random from the list.")]
		public MessageType dialogueType;

		[SerializeField] private Message[] lines;
		[SerializeField] private Answer[] answers;


		public List<Message> Lines => lines.ToList();
		public List<Answer> Answers => answers.ToList();


		// Unused for now	
		public Message GetLine()
		{
			if (lines.Length == 0)
				return default;

			return dialogueType switch
			{
				MessageType.InOrder => lines[0], // TODO give all lines in sequence?
				MessageType.Randomized => lines[Random.Range(0, lines.Length - 1)],
				_ => default
			};
		}
	}

	[Serializable]
	public class Answer
	{
		[SerializeField] public string Text;
		[SerializeField] public DialogueData.BossModifier Modifier;
		[SerializeField] public List<Message> DialogueIfChosen;
	}


	public enum MessageType
	{
		/// <summary>
		/// Dialogues lines are displayed one after the other.
		/// </summary>
		InOrder,

		/// <summary>
		/// Chooses one random line from the list.
		/// </summary>
		Randomized
	}

}
