using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleUiController : MonoBehaviour
{
    public Button moveOneBtn;
    public Button moveTwoBtn;
    public Button moveThreeBtn;
    public Button moveFourBtn;
    public Button cancelMovesBtn;

     public Slider enemyPokemonSlider;
    public Text enemyPokemonNameText;
    public Text enemyPokemonLevelText;
    //public Text enemyPokemonHpText;
    public Image enemyPokemonImage;

    public Slider ownPokemonSlider;
    public Text ownPokemonNameText;
    public Text ownPokemonLevelText;
    public Text ownPokemonHpText;
    public Image ownPokemonImage;

    public GameObject movesMenu;
    public GameObject movesCancelMenu;
    public GameObject actionMenu;
    public GameObject itemMenu;

    public MessageBoxController messageBoxController;

    public BattleController battleController;

    public Image ownPokemonSliderFill;
    public Image enemyPokemonSliderFill;

    public Button fightButton;
    public Button bagButton;
    public Button pokemonButton;
    public Button runButton;

    public delegate PokemonData getCurrentOwnActivePokemonDelegate();
    public getCurrentOwnActivePokemonDelegate getCurrentOwnActive;

    public delegate PokemonData getCurrentEnemyActivePokemonDelegate();
    public getCurrentEnemyActivePokemonDelegate getCurrentEnemyActive;

    public delegate void battleControllerIsDoneDelegate();
    public battleControllerIsDoneDelegate componentIsDoneDelegate;

    public void updateOwnPokemonHpText(PokemonData targetPokemonData) {
        ownPokemonHpText.text = targetPokemonData.currentHp + "/ " + targetPokemonData.getHpStat();
    }

    private void fightButtonClicked() {
        
        actionMenu.SetActive(false);
        movesMenu.SetActive(true);
        movesCancelMenu.SetActive(true);
        messageBoxController.hideMessageBox();
    }

    private void hideAllPanelsExceptText() {
        itemMenu.SetActive(false);
        movesCancelMenu.SetActive(false);
        movesMenu.SetActive(false);
        actionMenu.SetActive(false);
    }

    public void initiateEnemyPokemonControls(PokemonData enemyPokemon) {
        enemyPokemonSlider.maxValue = enemyPokemon.getHpStat();
        enemyPokemonSlider.minValue = 0;
        enemyPokemonSlider.value = enemyPokemon.currentHp;

        enemyPokemonNameText.text = enemyPokemon.basePokemon.name.ToUpper();
        enemyPokemonLevelText.text = "Lv" + enemyPokemon.pokemonLevel;
        enemyPokemonImage.GetComponent<Image>().sprite = enemyPokemon.basePokemon.frontSprite;

        enemyPokemonSlider.gameObject.SetActive(true);
        enemyPokemonNameText.gameObject.SetActive(true);
        enemyPokemonImage.gameObject.SetActive(true);
    }

    public void bindActionPanelButtons() {
        fightButton.onClick.AddListener(fightButtonClicked);
        // bagButton.onClick.AddListener(bagButtonClicked);
        
    }

    public void initiateOwnPokemonControls(PokemonData ownPokemonData) {
        ownPokemonSlider.maxValue = ownPokemonData.getHpStat();
        ownPokemonSlider.minValue = 0;
        ownPokemonSlider.value = ownPokemonData.currentHp;

        ownPokemonNameText.text = ownPokemonData.basePokemon.name.ToUpper();
        ownPokemonLevelText.text = "Lv" + ownPokemonData.pokemonLevel;
        updateOwnPokemonHpText(ownPokemonData);
        
        ownPokemonImage.GetComponent<Image>().sprite = ownPokemonData.basePokemon.backSprite;

        ownPokemonSlider.gameObject.SetActive(true);
        ownPokemonNameText.gameObject.SetActive(true);
        ownPokemonImage.gameObject.SetActive(true);
    }
    

    public void initiateMoveControllers(PokemonData ownPokemonData) {

        if (ownPokemonData.basePokemon.moves.Count > 0) {
            moveOneBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[0].name.ToUpper();
            moveOneBtn.onClick.AddListener(() => battleController.executeMove(0));
            // TODO migrar makeMove para executeMove em battleController
            // Decompor o pokemonBattleControllerdireito
        }
        if (ownPokemonData.basePokemon.moves.Count > 1) {
            moveTwoBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[1].name.ToUpper();
            moveTwoBtn.onClick.AddListener(() => battleController.executeMove(1));
        }
        if (ownPokemonData.basePokemon.moves.Count > 2) {
            moveThreeBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[2].name.ToUpper();
            moveThreeBtn.onClick.AddListener(() => battleController.executeMove(2));
        }
        if (ownPokemonData.basePokemon.moves.Count > 3) {
            moveFourBtn.GetComponentInChildren<Text>().text = ownPokemonData.basePokemon.moves[3].name.ToUpper();
            moveFourBtn.onClick.AddListener(() => battleController.executeMove(3));
        }
        
    }

    public void slowlyReduceHp(PokemonData defendingPokemon) {
        StartCoroutine(slowlyReduceHpEnumerator(defendingPokemon));
    }

    private IEnumerator slowlyReduceHpEnumerator(PokemonData defendingPokemon) {
        
        //isReady = false;

        Image targetPokemon;
        Slider targetPokemonSlider;

        bool updateText = false;
        if (defendingPokemon.Equals(getCurrentOwnActive())) {
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
                updateOwnPokemonHpText(defendingPokemon);
            }
            

            yield return new WaitForSeconds(0.1f);
        }
        
        targetPokemon.gameObject.SetActive(true);
        
        updateHealthBarColors(defendingPokemon);
        
        componentIsDoneDelegate();
        //isReady = true;
    }

    private void updateHealthBarColors(PokemonData defendingPokemon) {
        if (getCurrentEnemyActive() == defendingPokemon) {
            updateEnemyPokemonHpBarColor();
        }

        if (getCurrentOwnActive() == defendingPokemon) {
            updateOwnPokemonHpBarColor();
        }
    }

    private void updateOwnPokemonHpBarColor() {
        ownPokemonSliderFill.color
            = decideTargetColor(getCurrentOwnActive().getHpStat(), getCurrentOwnActive().currentHp);
    }

    private void updateEnemyPokemonHpBarColor() {
        enemyPokemonSliderFill.color
            = decideTargetColor(getCurrentEnemyActive().getHpStat(), getCurrentEnemyActive().currentHp);
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

    public void moveStateToAwaitingAction() {
        //isReady = true;
        itemMenu.SetActive(false);
        movesCancelMenu.SetActive(false);
        movesMenu.SetActive(false);
        //Debug.Log("Enabling action panel");
        actionMenu.SetActive(true);
        
    }
    
}
