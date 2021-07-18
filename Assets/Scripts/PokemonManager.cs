using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonManager : MonoBehaviour
{
    [Header("Trainer Data")]
    public int potionAmount = 5;

    [Header("Own Pokemon Binders")]
    public Team ownTeam;
    public Slider ownPokemonSlider;
    public Text ownPokemonNameText;
    public Text ownPokemonLevelText;
    public Text ownPokemonHpText;
    public Image ownPokemonImage;

    public List<Image> ownPokemonTeamPokeballs;

    [Header("Enemy Pokemon Binders")]
    public Team enemyTeam;
    public Slider enemyPokemonSlider;
    public Text enemyPokemonNameText;
    public Text enemyPokemonLevelText;
    //public Text enemyPokemonHpText;
    public Image enemyPokemonImage;
    public List<Image> enemyPokemonTeamPokeballs;

    [Header("Moves Binders")]
    public Button moveOneBtn;
    public Button moveTwoBtn;
    public Button moveThreeBtn;
    public Button moveFourBtn;
    public Button cancelMovesBtn;

    [Header("Panels")]
    public GameObject movesPanel;
    public GameObject movesCancelPanel;
    public GameObject actionPanel;
    public GameObject itemPanel;

    [Header("Action Panel Binders")]
    public Button fightButton;
    public Button bagButton;
    public Button pokemonButton;
    public Button runButton;

    [Header("Item Binders")]
    public Button cancelButton;

    [Header("Pokemon Switch Menu")]
    public List<Button> pokemonSwitch; 
    public GameObject pokemonSwitchMenu;
    public Button cancelPokemonSwitchButton;

    [Header("Other Controls")]
    public Text messageText;
    public Image ownPokemonSliderFill;
    public Image enemyPokemonSliderFill;

    public GameObject menu;

    public Sprite pokeballEmpty;
    public Sprite pokeballFaint;
    public Sprite pokeballStatus;
    public Sprite pokeballFull;
    
    private int charactersPerTurn = 10;

    private PokemonBattleController pokemonBattleController;
    
    public bool isReady;

    private bool startIsDone = false;

    private EventQueueSystem eventQueueSystem;

    // Start is called before the first frame update

    private PokemonData getActiveEnemyPokemon() {
        return enemyTeam.pokemonData[0];
    }

    private PokemonData getActiveOwnPokemon() {
        return ownTeam.pokemonData[0];
    }
    

    private void healAllPokemon() {

        for (int x = 0; x < ownTeam.pokemonData.Count; x++) {
            ownTeam.pokemonData[x].currentHp = ownTeam.pokemonData[x].getHpStat();
            ownTeam.pokemonData[x].accuracyStatisticsChange = 0;
            ownTeam.pokemonData[x].attackStatisticsChange = 0;
            ownTeam.pokemonData[x].defenseStatisticsChange = 0;
            ownTeam.pokemonData[x].evasionStatisticsChange = 0;
        }

        for (int x = 0; x < enemyTeam.pokemonData.Count; x++) {
            enemyTeam.pokemonData[x].currentHp = enemyTeam.pokemonData[x].getHpStat();
            enemyTeam.pokemonData[x].accuracyStatisticsChange = 0;
            enemyTeam.pokemonData[x].attackStatisticsChange = 0;
            enemyTeam.pokemonData[x].defenseStatisticsChange = 0;
            enemyTeam.pokemonData[x].evasionStatisticsChange = 0;
        }

    }

    void Start()
    {
        eventQueueSystem = gameObject.AddComponent<EventQueueSystem>();
        //eventQueueSystem.pokemonManager = this; // TODO 

        cancelMovesBtn.onClick.AddListener(() => {
            eventQueueSystem.enqueueEvent(new TextMessageEvent(getActiveEnemyPokemon().basePokemon.name.ToUpper() + "\n" +" wants to battle!"));
            moveStateToAwaitingAction();
        });

        pokemonButton.onClick.AddListener(() => {
            pokemonSwitchMenu.SetActive(true);
        });

        runButton.onClick.AddListener(() => {
            restartMatch();
        });

        /*cancelPokemonSwitchButton.onClick.AddListener(() => {
            pokemonSwitchMenu.SetActive(false);
        });
        */
        eventQueueSystem.initiateQueueSystem();
        
        healAllPokemon();
        
        initiateOwnPokemonControls();
        initiateEnemyPokemonControls();

        initiateMoveControllers();

        bindActionPanelButtons();
   
        populateItemPanel();

        eventQueueSystem.enqueueEvent(new TextMessageEvent(getActiveEnemyPokemon().basePokemon.name.ToUpper() + " \n wants to battle!"));
        moveStateToAwaitingAction();

        pokemonBattleController = new PokemonBattleController(getActiveOwnPokemon(), getActiveEnemyPokemon());
        
        pokemonBattleController.triggerTurnsWereExecutedDelegate+=moveStateToAwaitingAction;
        
        pokemonBattleController.emitEventDelegate+=eventQueueSystem.enqueueEvent;

        //bindTeamsToPokeball();

        //bindPokemonSwitchToTeam();

        startIsDone = true;

        
    }
    
    private void restartMatch() {
        
        startIsDone = false;
        healAllPokemon();
        eventQueueSystem.initiateQueueSystem();
        
        initiateOwnPokemonControls();
        initiateEnemyPokemonControls();

        initiateMoveControllers();

        bindActionPanelButtons();

        eventQueueSystem.enqueueEvent(new TextMessageEvent(getActiveEnemyPokemon().basePokemon.name + " wants to battle!"));
        moveStateToAwaitingAction();

        pokemonBattleController = new PokemonBattleController(getActiveOwnPokemon(), getActiveEnemyPokemon());
        
        pokemonBattleController.triggerTurnsWereExecutedDelegate+=moveStateToAwaitingAction;
        
        pokemonBattleController.emitEventDelegate+=eventQueueSystem.enqueueEvent;

        bindTeamsToPokeball();

        bindPokemonSwitchToTeam();

        updateHealthBarColors();

        startIsDone = true;
    }

    private void bindPokemonSwitchToTeam() {
        for (int x =0; x < pokemonSwitch.Count; x++) {
            pokemonSwitch[x].gameObject.SetActive(false);
        }
        
        for (int x = 0; x < ownTeam.pokemonData.Count; x++) {
            if (ownTeam.pokemonData[x] != null) {
                pokemonSwitch[x].GetComponent<Image>().sprite = ownTeam.pokemonData[x].basePokemon.frontSprite;
                pokemonSwitch[x].gameObject.SetActive(true);

                var targetIndex = x;
                pokemonSwitch[x].onClick.AddListener(() => { 

                    Debug.Log("Current" +targetIndex);
                    switchActiveOwnPokemon(targetIndex);
                });
            }
        }
    }

    private void switchActiveOwnPokemon(int targetIndex) {

        PokemonData newlyActive = ownTeam.pokemonData[targetIndex];
        PokemonData currentlyActive = ownTeam.pokemonData[0];

        ownTeam.pokemonData[0] = newlyActive;
        ownTeam.pokemonData[targetIndex] = currentlyActive;

        pokemonBattleController.switchOwnPokemon(ownTeam.pokemonData[0]);

        healAllPokemon();
        updateHealthBarColors();
        updateOwnPokemonHpText();
        
        initiateOwnPokemonControls();
        initiateEnemyPokemonControls();

        initiateMoveControllers();
        
    }
    
    private void bindTeamsToPokeball() {
        
        for (int x = 0; x < ownTeam.pokemonData.Count; x++) {
            if (ownTeam.pokemonData[x] != null) {
                ownPokemonTeamPokeballs[x].sprite = pokeballFull;
            }
        }

        for (int x = 0; x < enemyTeam.pokemonData.Count; x++) {
            if (enemyTeam.pokemonData[x] != null) {
                enemyPokemonTeamPokeballs[x].sprite = pokeballFull;
            }
        }
        
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) ) {
            menu.gameObject.SetActive(!(menu.gameObject.activeInHierarchy));
        }
    }
    
    private void moveStateToAwaitingAction() {
        isReady = true;
        itemPanel.SetActive(false);
        movesCancelPanel.SetActive(false);
        movesPanel.SetActive(false);
        Debug.Log("Enabling action panel");
        actionPanel.SetActive(true);
    }
    
    
    
    private void bindActionPanelButtons() {
        fightButton.onClick.AddListener(fightButtonClicked);
        bagButton.onClick.AddListener(bagButtonClicked);
        
    }

    private void populateItemPanel() {
        
        //Button potion = Instantiate(cancelButton);
        //potion.gameObject.transform.parent = itemPanel.transform;
        //potion.GetComponentInChildren<Text>().text = "potion x " + potionAmount;

        /*
        potion.onClick.AddListener(() => { 
            potionAmount--;
            ownPokemonData.currentHp +=20;
            potion.GetComponentInChildren<Text>().text = "potion x " + potionAmount;

            executeEnemyTurn();

            updateValuesAndSliders();

            moveStateToAwaitingAction();

        });
        */

        cancelButton.onClick.AddListener(() => { 
            itemPanel.SetActive(false); actionPanel.SetActive(true);
        });
    }
    
    private void bagButtonClicked() {
        itemPanel.SetActive(true);
        actionPanel.SetActive(false);
    }

    private void fightButtonClicked() {
        Debug.Log("Disabling action panel");
        actionPanel.SetActive(false);
        movesPanel.SetActive(true);
        movesCancelPanel.SetActive(true);
        messageText.gameObject.SetActive(false);
    }

    private void initiateMoveControllers() {

        if (getActiveOwnPokemon().basePokemon.moves.Count > 0) {
            moveOneBtn.GetComponentInChildren<Text>().text = getActiveOwnPokemon().basePokemon.moves[0].name.ToUpper();
            moveOneBtn.onClick.AddListener(() => pokemonBattleController.makeMove(0));
        }
        if (getActiveOwnPokemon().basePokemon.moves.Count > 1) {
            moveTwoBtn.GetComponentInChildren<Text>().text = getActiveOwnPokemon().basePokemon.moves[1].name.ToUpper();
            moveTwoBtn.onClick.AddListener(() => pokemonBattleController.makeMove(1));
        }
        if (getActiveOwnPokemon().basePokemon.moves.Count > 2) {
            moveThreeBtn.GetComponentInChildren<Text>().text = getActiveOwnPokemon().basePokemon.moves[2].name.ToUpper();
            moveThreeBtn.onClick.AddListener(() => pokemonBattleController.makeMove(2));
        }
        if (getActiveOwnPokemon().basePokemon.moves.Count > 3) {
            moveFourBtn.GetComponentInChildren<Text>().text = getActiveOwnPokemon().basePokemon.moves[3].name.ToUpper();
            moveFourBtn.onClick.AddListener(() => pokemonBattleController.makeMove(3));
        }
        
    }
    
    private void pokemonWasDamaged() {
        disableAllPanelsExceptText();
        if (enemyPokemonSlider.value > getActiveEnemyPokemon().currentHp) {
            StartCoroutine(slowlyReduceHp(getActiveEnemyPokemon()));
        }
        if (ownPokemonSlider.value > getActiveOwnPokemon().currentHp) {
            StartCoroutine(slowlyReduceHp(getActiveOwnPokemon()));
        }
        
    }

    private void disableAllPanelsExceptText() {
        Debug.Log("Disabvling action panel");
        itemPanel.SetActive(false);
        movesCancelPanel.SetActive(false);
        movesPanel.SetActive(false);
        actionPanel.SetActive(false);
    }
    
    private void updateHealthBarColors() {
        if (getActiveEnemyPokemon() != null) enemyPokemonSliderFill.color = decideTargetColor(getActiveEnemyPokemon().getHpStat(), getActiveEnemyPokemon().currentHp);
        if (getActiveOwnPokemon() != null) ownPokemonSliderFill.color = decideTargetColor(getActiveOwnPokemon().getHpStat(), getActiveOwnPokemon().currentHp);
    }

    private Color decideTargetColor(int maxHp, int currentHp) {
        
        float currentPercentage = Mathf.Round((currentHp * 100)/maxHp);

        if (currentPercentage > 60) {
            return Color.green;
        } else if (currentPercentage > 40) {
            return Color.yellow;
        } else if (currentPercentage > 0) {
            return Color.red;
        } else {
            return Color.black;
        }

    }
    
    public IEnumerator slowlyReduceHp(PokemonData defendingPokemon) {
        
        isReady = false;

        Image targetPokemon;
        Slider targetPokemonSlider;
        PokemonData attackingPokemon;
        bool updateText = false;
        if (defendingPokemon.Equals(getActiveOwnPokemon())) {
            updateText=true;
            targetPokemon = ownPokemonImage;
            targetPokemonSlider = ownPokemonSlider;
            attackingPokemon = getActiveEnemyPokemon();
        } else {
            targetPokemon = enemyPokemonImage;
            targetPokemonSlider = enemyPokemonSlider;
            attackingPokemon = getActiveOwnPokemon();
        }
        
        while (targetPokemonSlider.value > 0 
            && targetPokemonSlider.value > defendingPokemon.currentHp) {

            targetPokemon.gameObject.SetActive(!targetPokemon.gameObject.activeSelf);
            targetPokemonSlider.value--;
            
            if (updateText) {
                updateOwnPokemonHpText(Mathf.RoundToInt(targetPokemonSlider.value));
            }
            updateHealthBarColors();

            yield return new WaitForSeconds(0.1f);
        }
        
        targetPokemon.gameObject.SetActive(true);

        isReady = true;
    }
    
    private void initiateOwnPokemonControls() {
        ownPokemonSlider.maxValue = getActiveOwnPokemon().getHpStat();
        ownPokemonSlider.minValue = 0;
        ownPokemonSlider.value = getActiveOwnPokemon().currentHp;

        ownPokemonNameText.text = getActiveOwnPokemon().basePokemon.name.ToUpper();
        ownPokemonLevelText.text = "Lv" + getActiveOwnPokemon().pokemonLevel;
        updateOwnPokemonHpText();
        
        ownPokemonImage.GetComponent<Image>().sprite = getActiveOwnPokemon().basePokemon.backSprite;

        ownPokemonSlider.gameObject.SetActive(true);
        ownPokemonNameText.gameObject.SetActive(true);
        ownPokemonImage.gameObject.SetActive(true);
    }

    private void initiateEnemyPokemonControls() {
        enemyPokemonSlider.maxValue = getActiveEnemyPokemon().getHpStat();
        enemyPokemonSlider.minValue = 0;
        enemyPokemonSlider.value = getActiveEnemyPokemon().currentHp;

        enemyPokemonNameText.text = getActiveEnemyPokemon().basePokemon.name.ToUpper();
        enemyPokemonLevelText.text = "Lv" + getActiveEnemyPokemon().pokemonLevel;
        enemyPokemonImage.GetComponent<Image>().sprite = getActiveEnemyPokemon().basePokemon.frontSprite;

        enemyPokemonSlider.gameObject.SetActive(true);
        enemyPokemonNameText.gameObject.SetActive(true);
        enemyPokemonImage.gameObject.SetActive(true);
    }
    
    private void updateOwnPokemonHpText() {
        ownPokemonHpText.text = getActiveOwnPokemon().currentHp + "/ " + getActiveOwnPokemon().getHpStat();
    }

    private void updateOwnPokemonHpText(int targetCurrentHp) {
        ownPokemonHpText.text = targetCurrentHp + "/ " + getActiveOwnPokemon().getHpStat();
    }
    
    public void faintOwnPokemon() {
        ownPokemonSlider.gameObject.SetActive(false);

        ownPokemonNameText.gameObject.SetActive(false);
        ownPokemonHpText.gameObject.SetActive(false);
        ownPokemonImage.gameObject.SetActive(false);

        ownPokemonTeamPokeballs[0].sprite = pokeballEmpty;

        switchActiveOwnPokemon(1);

        //getActiveOwnPokemon() = null;   
    }

    public void faintEnemyPokemon() {
        enemyPokemonSlider.gameObject.SetActive(false);

        enemyPokemonNameText.gameObject.SetActive(false);
        enemyPokemonLevelText.gameObject.SetActive(false);
        enemyPokemonImage.gameObject.SetActive(false);

        enemyPokemonTeamPokeballs[0].sprite = pokeballFaint;
        //enemyPokemonData = null;
    }

    public IEnumerator faintPokemon(PokemonData targetPokemon) {
        
        isReady = false;

        if (getActiveOwnPokemon() == targetPokemon) {
            faintOwnPokemon();
        } else {
            faintEnemyPokemon();
        }
        isReady = true;

        yield return new WaitForSeconds(0.5f);
    }


    public IEnumerator displayMessage(string msg) {
        
        isReady = false;
        messageText.gameObject.SetActive(true);
        int currentSubstringIndex = 0;
        string currentSubstring = "";

        while (!currentSubstring.Equals(msg)) {
            currentSubstring+= msg.Substring(currentSubstringIndex,  1);
            currentSubstringIndex++;
            messageText.text = currentSubstring;
            yield return new WaitForSeconds(0.01f);
        }
        
        yield return new WaitForSeconds(0.5f);

        isReady = true;
        
    }

    public void showMessage(string msg) {
        
        StartCoroutine(displayMessage(msg));
    }
    
}
