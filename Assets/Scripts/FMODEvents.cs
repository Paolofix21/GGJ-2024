using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Mono.Cecil;
using EventReference = FMODUnity.EventReference;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambienceEvent { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference musicEvent { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootstepsEvent { get; private set; }
    [field: SerializeField] public EventReference playerCrouchEvent { get; private set; }
    [field: SerializeField] public EventReference playerJumpEvent { get; private set; }
    [field: SerializeField] public EventReference playerHealEvent { get; private set; }
    [field: SerializeField] public EventReference playerTakeDamageEvent { get; private set; }
    [field: SerializeField] public EventReference playerDeathEvent { get; private set; }
    
    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference spawnEvent { get; private set; }


    public static FMODEvents instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events script in the scene.");
        }
        instance = this;
    }
}
