using System;
using System.Collections.Generic;
using System.Linq;
using Advepa.SchoolMetaverse.Laboratori;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue", order = 1)]
public class DialogueScriptableObject : ScriptableObject
{
	[Tooltip("Standard displays one line after the other. Randomize chooses one line at random from the list.")]
	public DialogueType dialogueType;
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
			DialogueType.InOrder => lines[0], // TODO give all lines in sequence?
			DialogueType.Randomized => lines[Random.Range(0, lines.Length - 1)],
			_ => default
		};
	}
}

[Serializable]
public class Answer
{
	[SerializeField] public string Text;
	[SerializeField] public bool IsCorrect;
}


public enum DialogueType
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
