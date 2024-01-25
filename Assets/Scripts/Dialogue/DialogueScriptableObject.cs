using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue", order = 1)]
public class DialogueScriptableObject : ScriptableObject
{
	// Necessary?
	public int dialogueID;
	[Tooltip("Standard displays one line after the other. Randomize chooses one line at random from the list.")]
	public DialogueType dialogueType;
	[SerializeField, TextArea] private string[] dialogues;

	public string[] Dialogues => dialogues;

	
	public string GetLine()
	{
		if (dialogues.Length == 0)
			return string.Empty;
		
		
		switch (dialogueType)
		{
			case DialogueType.Standard:
				return dialogues[0];
			
			case DialogueType.Randomized:
				return dialogues[Random.Range(0, dialogues.Length - 1)];
			default:
				return string.Empty;
		}
	}
}


public enum DialogueType
{
	Standard,
	Randomized
}
