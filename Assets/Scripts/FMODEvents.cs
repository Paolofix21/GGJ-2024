using UnityEngine;
using EventReference = FMODUnity.EventReference;

public class FMODEvents : MonoBehaviour
{
    //Creato parametro GLOBALE FMOD di tipo labeled/string ("JUMP"), usato per cambiare Ambience e Music da main menu, exploration e boss
    
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
    [field: SerializeField] public EventReference deathCoughEvent { get; private set; }
    [field: SerializeField] public EventReference enemyLaughEvent { get; private set; }
    [field: SerializeField] public EventReference explosionEvent { get; private set; }
    [field: SerializeField] public EventReference swingEvent { get; private set; }
    [field: SerializeField] public EventReference wakakaVerseEvent { get; private set; }
    [field: SerializeField] public EventReference wakakatuVerseEvent { get; private set; }
    [field: SerializeField] public EventReference wakakatupureVerseEvent { get; private set; }

    [field: Header("Dialogue")]
    [field: SerializeField] public EventReference dialogueHighEvent { get; private set; }
    [field: SerializeField] public EventReference dialogueLowEvent { get; private set; }

    [field: Header("Weapon")]
    [field: SerializeField] public EventReference arEvent { get; private set; }
    [field: SerializeField] public EventReference granadeEvent { get; private set; }
    [field: SerializeField] public EventReference mysteriousEvent { get; private set; }
    [field: SerializeField] public EventReference pistolOneShootEvent { get; private set; }
    [field: SerializeField] public EventReference pistolTwoShoootEvent { get; private set; }
    [field: SerializeField] public EventReference pumpEvent { get; private set; }
    [field: SerializeField] public EventReference rifleTikEvent { get; private set; }
    [field: SerializeField] public EventReference shotgunFartEvent { get; private set; }
    [field: SerializeField] public EventReference shotgunPtewfEvent { get; private set; }
    [field: SerializeField] public EventReference swordHitEvent { get; private set; }
    [field: SerializeField] public EventReference swordSiumEvent { get; private set; }
    [field: SerializeField] public EventReference upgradeEvent { get; private set; }
    [field: SerializeField] public EventReference whipHitEvent { get; private set; }
    [field: SerializeField] public EventReference whipSqadushEvent { get; private set; }

    [field: Header("VO")]
    [field: SerializeField] public EventReference evilLaughEvent { get; private set; }
    [field: SerializeField] public EventReference voBossEvent { get; private set; }
    [field: SerializeField] public EventReference voDropAmmunitionEvent { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference uiClickEvent { get; private set; }

    [field: Header("Game")]
    //Evento FMOD con parametro ("Pause") dove vengono attenuati tutti gli altri volumi
    //pensato per l'apertuta del menu opzioni in game
    [field: SerializeField] public EventReference pauseEvent { get; private set; }

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
