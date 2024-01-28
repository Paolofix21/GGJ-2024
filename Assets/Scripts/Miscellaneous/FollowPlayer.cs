using UnityEngine;

/// <summary>
/// Keeps rotating the object so that it faces the player
/// </summary>
public class FollowPlayer : MonoBehaviour
{
	private Transform player;
		
	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player")?.transform;
	}
	
	void Update()
	{
		if (player == null)
			return;
		
		transform.LookAt(player);
	}
}
