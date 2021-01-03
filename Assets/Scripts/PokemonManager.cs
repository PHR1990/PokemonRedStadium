using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PokemonManager : MonoBehaviour
{
    [Header("Trainer Data")]
    public int potionAmount = 5;

    [Header("Own Pokemon Binders")]
    public PokemonData ownPokemonData;
    public Slider ownPokemonSlider;
    public Text ownPokemonNameText;
    public Text ownPokemonLevelText;
    public Text ownPokemonHpText;
    public Image ownPokemonImage;

    [Header("Enemy Pokemon Binders")]
    public PokemonData enemyPokemonData;
    public Slider enemyPokemonSlider;
    public Text enemyPokemonNameText;
    public Text enemyPokemonLevelText;
    //public Text enemyPokemonHpText;
    public Image enemyPokemonImage;

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

    [Header("Item Binders")]
    public Button cancelButton;

    [Header("Other Controls")]
    public Text messageText;
    public Image ownPokemonSliderFill;
    public Image enemyPokemonSliderFill;

    private PokemonBattleController pokemonBattleController;

    
    // Start is called before the first frame update
    void Start()
    {
        
        ownPokemonData.currentHp = ownPokemonData.getHpStat();
        enemyPokemonData.currentHp = enemyPokemonData.getHpStat();


        initiateOwnPokemonControls();
        initiateEnemyPokemonControls();

        initiateMoveControllers();

        bindActionPanelButtons();
   
        populateItemPanel();

        cancelMovesBtn.onClick.AddListener(() => {
            moveStateToAwaitingAction();
        });

        moveStateToAwaitingAction();

        pokemonBattleController = new PokemonBattleController(ownPokemonData, enemyPokemonData);

        pokemonBattleController.triggerDamageWasDealtDelegate+=pokemonWasDamaged;
        pokemonBattleController.triggerDamageWasDealtDelegate+=moveStateToAwaitingAction;
    }

    

    private void moveStateToAwaitingAction() {
        messageText.gameObject.SetActive(true);
        messageText.text = enemyPokemonData.basePokemon.name + " wants to battle!";
        itemPanel.SetActive(false);
        movesCancelPanel.SetActive(false);
        movesPanel.SetActive(false);
        actionPanel.SetActive(true);

    }
    
    private void bindActionPanelButtons() {
        fightButton.onClick.AddListener(fightButtonClicked);
        bagButton.onClick.AddListener(bagButtonClicked);
        
    }

    private void populateItemPanel() {
        
        Button potion = Instantiate(cancelButton);
        potion.gameObject.transform.parent = itemPanel.transform;
        potion.GetComponentInChildren<Text>().text = "potion x " + potionAmount;

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
        actionPanel.SetActive(false);
        movesPanel.SetActive(true);
        movesCancelPanel.SetActive(true);
        messageText.gameObject.SetActive(false);
    }

    private void initiateMoveControllers() {

        if (ownPokemonData.basePokemon.moves.Count > 0) {
            moveOneBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[0].name.ToUpper();
            moveOneBtn.onClick.AddListener(() => pokemonBattleController.makeMove(0));
        }
        if (ownPokemonData.basePokemon.moves.Count > 1) {
            moveTwoBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[1].name.ToUpper();
            moveTwoBtn.onClick.AddListener(() => pokemonBattleController.makeMove(1));
        }
        if (ownPokemonData.basePokemon.moves.Count > 2) {
            moveThreeBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[2].name.ToUpper();
            moveThreeBtn.onClick.AddListener(() => pokemonBattleController.makeMove(2));
        }
        if (ownPokemonData.basePokemon.moves.Count > 3) {
            moveFourBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[3].name.ToUpper();
            moveFourBtn.onClick.AddListener(() => pokemonBattleController.makeMove(3));
        }
        
    }
    
    private void pokemonWasDamaged() {
        if (enemyPokemonSlider.value > enemyPokemonData.currentHp) {
            StartCoroutine(slowlyReduceHp(enemyPokemonData));
        }
        if (ownPokemonSlider.value > ownPokemonData.currentHp) {
            StartCoroutine(slowlyReduceHp(ownPokemonData));
        }
    }
    
    private void updateHealthBarColors() {
        enemyPokemonSliderFill.color = decideTargetColor(enemyPokemonData.getHpStat(), enemyPokemonData.currentHp);
        ownPokemonSliderFill.color = decideTargetColor(ownPokemonData.getHpStat(), ownPokemonData.currentHp);
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
    
    IEnumerator slowlyReduceHp(PokemonData defendingPokemon) {
        
        Image targetPokemon;
        Slider targetPokemonSlider;
        bool updateText = false;
        if (defendingPokemon.Equals(ownPokemonData)) {
            updateText=true;
            targetPokemon = ownPokemonImage;
            targetPokemonSlider = ownPokemonSlider;
        } else {
            targetPokemon = enemyPokemonImage;
            targetPokemonSlider = enemyPokemonSlider;
        }
        
        while (targetPokemonSlider.value > 0 
            && targetPokemonSlider.value > defendingPokemon.currentHp) {

            targetPokemon.gameObject.SetActive(!targetPokemon.gameObject.activeSelf);
            targetPokemonSlider.value--;
            
            if (updateText) {
                updateOwnPokemonHpText(Mathf.RoundToInt(targetPokemonSlider.value));// as
            }
            updateHealthBarColors();

            yield return new WaitForSeconds(0.2f);
        }
        
        targetPokemon.gameObject.SetActive(true);

    }
    
    private void initiateOwnPokemonControls() {
        ownPokemonSlider.maxValue = ownPokemonData.getHpStat();
        ownPokemonSlider.minValue = 0;
        ownPokemonSlider.value = ownPokemonData.currentHp;

        ownPokemonNameText.text = ownPokemonData.basePokemon.name;
        updateOwnPokemonHpText();
        
    }

    private void initiateEnemyPokemonControls() {
        enemyPokemonSlider.maxValue = enemyPokemonData.getHpStat();
        enemyPokemonSlider.minValue = 0;
        enemyPokemonSlider.value = enemyPokemonData.currentHp;

        enemyPokemonNameText.text = enemyPokemonData.basePokemon.name;
        
    }
    
    private void updateOwnPokemonHpText() {
        ownPokemonHpText.text = ownPokemonData.currentHp + "/ " + ownPokemonData.getHpStat();
    }

    private void updateOwnPokemonHpText(int targetCurrentHp) {
        ownPokemonHpText.text = targetCurrentHp + "/ " + ownPokemonData.getHpStat();
    }
    
}
