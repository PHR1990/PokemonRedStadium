using UnityEngine;
using UnityEngine.UI;

public class BattleControlsComponent : MonoBehaviour
{
    // Will be dynamically loaded in the future
    [Header("Data")]
    [Header("Own Pokemon Data")]
    public Team ownTeam;
    [Header("Own Item Data")]
    [Header("Enemy Pokemon Data")]
    public Team enemyTeam;

    // Permanentely bound to a scene
    [Header("UI Elements")]
    public Text messageText;

    [Header("Menus")]
    public GameObject movesMenu;
    public GameObject movesCancelMenu;
    public GameObject actionMenu;
    public GameObject itemMenu;

    [Header("Action Menu")]
    public Button fightButton;
    public Button bagButton;
    public Button pokemonButton;
    public Button runButton;
    //[Header("Item Binders")]
    //[Header("Pokemon Switch Menu")]

    [Header("Move Menu Buttons")]
    public Button moveOneBtn;
    public Button moveTwoBtn;
    public Button moveThreeBtn;
    public Button moveFourBtn;
    public Button cancelMovesBtn;

    [Header("Own Pokemon UI Elements")]
    public Slider ownPokemonSlider;
    public Image ownPokemonSliderFill;
    public Text ownPokemonNameText;
    public Text ownPokemonLevelText;
    public Text ownPokemonHpText;
    public Image ownPokemonImage;
    //public List<Image> ownPokemonTeamPokeballs;

    [Header("Enemy Pokemon UI Elements")]
    public Slider enemyPokemonSlider;
    public Image enemyPokemonSliderFill;
    public Text enemyPokemonNameText;
    public Text enemyPokemonLevelText;
    //public Text enemyPokemonHpText;
    public Image enemyPokemonImage;
    //public List<Image> enemyPokemonTeamPokeballs;

    // Prefab
    [Header("Prefab Configs")]
    [Header("Pre-configured Sprites")]
    public Sprite pokeballEmpty;
    public Sprite pokeballFaint;
    public Sprite pokeballStatus;
    public Sprite pokeballFull;

    // Inner controllers
    //[Header("Pre-configured Controllers")]
    private MessageBoxController messageBoxController;
    private AnimationController animationController;
    private BattleController battleController;
    private BattleUiController battleUiController;

    private EventQueueSystem eventQueueSystem;

    public bool isReady;

    private void initiateControllers() {
        
        messageBoxController = gameObject.AddComponent<MessageBoxController>();
        messageBoxController.messageText = messageText;
        messageBoxController.doneWithLastActionDelegate = eventQueueSystem.callerIsDone;

        animationController = gameObject.AddComponent<AnimationController>();
        animationController.ownPokemonSlider = ownPokemonSlider;
        animationController.ownPokemonNameText = ownPokemonNameText;
        animationController.ownPokemonLevelText = ownPokemonLevelText;
        animationController.ownPokemonHpText = ownPokemonHpText;
        animationController.ownPokemonImage = ownPokemonImage;
                
        animationController.enemyPokemonSlider = enemyPokemonSlider;
        animationController.enemyPokemonNameText = enemyPokemonNameText;
        animationController.enemyPokemonLevelText = enemyPokemonLevelText;
        animationController.enemyPokemonImage = enemyPokemonImage;
        
        battleController = gameObject.AddComponent<BattleController>();
        battleController.ownTeam = ownTeam;
        battleController.enemyTeam = enemyTeam;
        battleController.animationController = animationController;
        battleController.enqueueBattleEventDelegate= eventQueueSystem.enqueueEvent;
        
        battleUiController = gameObject.AddComponent<BattleUiController>();
        battleUiController.moveOneBtn = moveOneBtn;
        battleUiController.moveTwoBtn= moveTwoBtn;
        battleUiController.moveThreeBtn= moveThreeBtn;
        battleUiController.moveFourBtn= moveFourBtn;
        battleUiController.cancelMovesBtn= cancelMovesBtn;
        battleUiController.enemyPokemonSlider= enemyPokemonSlider;
        battleUiController.enemyPokemonNameText= enemyPokemonNameText;
        battleUiController.enemyPokemonLevelText= enemyPokemonLevelText;
        battleUiController.enemyPokemonImage= enemyPokemonImage;
        battleUiController.ownPokemonSlider = ownPokemonSlider;
        battleUiController.ownPokemonNameText = ownPokemonNameText;
        battleUiController.ownPokemonLevelText = ownPokemonLevelText;
        battleUiController.ownPokemonHpText = ownPokemonHpText;
        battleUiController.ownPokemonImage = ownPokemonImage;
        battleUiController.movesMenu = movesMenu;
        battleUiController.movesCancelMenu = movesCancelMenu;
        battleUiController.actionMenu = actionMenu;
        battleUiController.itemMenu = itemMenu;
        
        battleUiController.fightButton = fightButton;
        battleUiController.bagButton = bagButton;
        battleUiController.pokemonButton = pokemonButton;
        battleUiController.runButton = runButton;

        battleUiController.messageBoxController = messageBoxController;

        battleUiController.battleController = battleController;

        battleUiController.ownPokemonSliderFill = ownPokemonSliderFill;
        battleUiController.enemyPokemonSliderFill = enemyPokemonSliderFill;
        battleUiController.getCurrentOwnActive = getCurrentOwnActivePokemon;
        battleUiController.getCurrentEnemyActive = getCurrentEnemyActivePokemon;
        battleUiController.componentIsDoneDelegate = unblockPlayer;

    }

    

    private PokemonData getCurrentOwnActivePokemon() {
        return ownTeam.pokemonData[0];
    }
    private PokemonData getCurrentEnemyActivePokemon() {
        return enemyTeam.pokemonData[0];
    }
    
    // Use this for initialization
    void Start()
    {
        // Step1 Initiate queue system
        eventQueueSystem = gameObject.AddComponent<EventQueueSystem>();
        eventQueueSystem.battleControlsComponent = this;
        eventQueueSystem.initiateQueueSystem();

        // Step2 Initiate child controlers
        initiateControllers();

        // Step 3 bind remaining buttons (shouldnt be done here) TODO
        cancelMovesBtn.onClick.AddListener(() => {
            eventQueueSystem.enqueueEvent(new TextMessageEvent(getCurrentEnemyActivePokemon().basePokemon.name.ToUpper() + "\n" +" wants to battle!"));
            battleUiController.moveStateToAwaitingAction();
        });

        pokemonButton.onClick.AddListener(() => {
            //pokemonSwitchMenu.SetActive(true);
        });

        runButton.onClick.AddListener(() => {
            //restartMatch();
        });

        /*cancelPokemonSwitchButton.onClick.AddListener(() => {
            pokemonSwitchMenu.SetActive(false);
        });
        */
        
        battleUiController.initiateOwnPokemonControls(getCurrentOwnActivePokemon());
        battleUiController.initiateEnemyPokemonControls(getCurrentEnemyActivePokemon());
        battleUiController.initiateMoveControllers(getCurrentOwnActivePokemon());
        battleUiController.bindActionPanelButtons();

        eventQueueSystem.enqueueEvent(new TextMessageEvent(getCurrentEnemyActivePokemon().basePokemon.name.ToUpper() + " \n wants to battle!"));
        battleUiController.moveStateToAwaitingAction();

        //pokemonBattleController = new PokemonBattleController(getCurrentOwnActivePokemon, getCurrentEnemyActivePokemon);
        
        battleController.triggerTurnsWereExecutedDelegate=unblockPlayer;
        
        battleController.enqueueBattleEventDelegate=eventQueueSystem.enqueueEvent;

        //bindTeamsToPokeball();

        //bindPokemonSwitchToTeam();

        //startIsDone = true;
    }

    public void displayMessage(string txtMessage) {
        messageBoxController.displayWaitMessage(txtMessage);
    }
    public void slowlyReduceHp(PokemonData pokemonData) {
        
        battleUiController.slowlyReduceHp(pokemonData);
    }

    public void faintPokemon(PokemonData pokemonData) {
    }

    public void blockPlayer() {
        
        battleUiController.moveStateToPlayerIsWatchingBattleAnimation();
        isReady = true;
    }
    
    public void unblockPlayer() {
        
        battleUiController.moveStateToAwaitingAction();
        isReady = true;
    }
}
