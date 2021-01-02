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
    public Text enemyPokemonHpText;
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

        potion.onClick.AddListener(() => { 
            potionAmount--;
            ownPokemonData.currentHp +=20;
            potion.GetComponentInChildren<Text>().text = "potion x " + potionAmount;

            executeEnemyTurn();

            updateValuesAndSliders();

            moveStateToAwaitingAction();

        });

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
            moveOneBtn.onClick.AddListener(() => makeMove(0));
        }
        if (ownPokemonData.basePokemon.moves.Count > 1) {
            moveTwoBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[1].name.ToUpper();
            moveTwoBtn.onClick.AddListener(() => makeMove(1));
        }
        if (ownPokemonData.basePokemon.moves.Count > 2) {
            moveThreeBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[2].name.ToUpper();
            moveThreeBtn.onClick.AddListener(() => makeMove(2));
        }
        if (ownPokemonData.basePokemon.moves.Count > 3) {
            moveFourBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[3].name.ToUpper();
            moveFourBtn.onClick.AddListener(() => makeMove(3));
        }
        
    }

    private int calculateDamage(PokemonData attacking, PokemonData defending, Move move) {

        int attack = attacking.getAttackStat();
        int defense = defending.getDefenseStat();
        if (move.effect == Effect.Special) {

            attack = attacking.getSpecialStat();
            defense = defending.getSpecialStat();
        }

        return Mathf.RoundToInt( 
            (((((attacking.pokemonLevel * 2)/5) + 2) * move.power *attack/defense)/50) +2 );
    }
    

    private void makeMove(int move) {

        if (ownPokemonData.getSpeedStat() > enemyPokemonData.getSpeedStat()) {
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            executeEnemyTurn();
        } else {
            executeEnemyTurn();
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
        }
        
        updateValuesAndSliders();

        moveStateToAwaitingAction();
        
    }
    
    private void updateValuesAndSliders() {
        updateEnemyPokemonHpText();
        enemyPokemonSlider.value = enemyPokemonData.currentHp;
        enemyPokemonSliderFill.color = decideTargetColor(enemyPokemonData.getHpStat(), enemyPokemonData.currentHp);

        updateOwnPokemonHpText();
        ownPokemonSlider.value = ownPokemonData.currentHp;
        ownPokemonSliderFill.color = decideTargetColor(ownPokemonData.getHpStat(), ownPokemonData.currentHp);
    }

    private Color decideTargetColor(int maxHp, int currentHp) {
        // maxHp - 100
        // currentHp - x
        // 40 - 100
        // 20 - x
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


    private void executeEnemyTurn() {
        int moveToUse = Random.Range(0, enemyPokemonData.basePokemon.moves.Count);
        executeMove(enemyPokemonData, ownPokemonData, enemyPokemonData.basePokemon.moves[moveToUse]);
    }

    private bool willMoveHit(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {
        float accuracyBase = move.accuracy;
        float accuracy = evasionAccuracyFlatToPercentage(attackingPokemon.accuracyStatisticsChange);
        float evasion = evasionAccuracyFlatToPercentage(defendingPokemon.evasionStatisticsChange);

        float calculatedAccuracy = (accuracyBase * (accuracy/evasion));
        if (calculatedAccuracy >= Random.Range(1, 100)) {
            
            return true;
        }
        
        return false;

    }

    private int evasionAccuracyFlatToPercentage(int flatValue) {
        switch (flatValue) {
            case -6 :
                return 33;
            case -5 :
                return 36;
            case -4 :
                return 43;
            case -3 :
                return 50;
            case -2 :
                return 66;
            case -1 :
                return 75;
            case 0 :
                return 100;
            case 1 :
                return 133;
            case 2 :
                return 166;
            case 3 :
                return 200;
            case 4 :
                return 250;
            case 5 :
                return 266;
            case 6 :
                return 300;
                
        }
        return 100;
    }

    IEnumerator slowlyReduceHp(int damage, PokemonData defendingPokemon) {
        
        Image targetPokemon;

        if (defendingPokemon.Equals(ownPokemonData)) {
            targetPokemon = ownPokemonImage;
        } else {
            targetPokemon = enemyPokemonImage;
        }

        for (int iterations = 0; iterations <= damage; iterations++) {
            targetPokemon.gameObject.SetActive(!targetPokemon.gameObject.activeSelf);

            defendingPokemon.currentHp = defendingPokemon.currentHp - 1; 
            updateValuesAndSliders();
            
            yield return new WaitForSeconds(0.2f);

        }

        targetPokemon.gameObject.SetActive(true);

    }

    private void executeMove(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {

        bool didMoveHit = willMoveHit(attackingPokemon, defendingPokemon, move);

        if (!didMoveHit) {
            return;
        }

        
        
        if (move.effect.Equals(Effect.Physical) || move.effect.Equals(Effect.Special) ) {
            
            int damage = calculateDamage(attackingPokemon, defendingPokemon, move);

            StartCoroutine(slowlyReduceHp(damage, defendingPokemon));
            //defendingPokemon.currentHp = defendingPokemon.currentHp - damage; 
        }
        if (move.effect.Equals(Effect.StatusEnemyAttack)) {
            if (defendingPokemon.attackStatisticsChange > -6) {
                defendingPokemon.attackStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyDefense)) {
            if (defendingPokemon.defenseStatisticsChange > -6) {
                defendingPokemon.defenseStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyAccuracy)) {
            
            if (defendingPokemon.accuracyStatisticsChange > -6 && defendingPokemon.accuracyStatisticsChange < 6) {
                defendingPokemon.accuracyStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
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
        updateEnemyPokemonHpText();
        
    }
    private void updateEnemyPokemonHpText() {
        enemyPokemonHpText.text = enemyPokemonData.currentHp + "/ " + enemyPokemonData.getHpStat();
    }

    private void updateOwnPokemonHpText() {
        ownPokemonHpText.text = ownPokemonData.currentHp + "/ " + ownPokemonData.getHpStat();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
