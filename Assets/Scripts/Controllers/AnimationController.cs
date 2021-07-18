using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    public Slider ownPokemonSlider;
    public Text ownPokemonNameText;
    public Text ownPokemonLevelText;
    public Text ownPokemonHpText;
    public Image ownPokemonImage;

    public Slider enemyPokemonSlider;
    public Text enemyPokemonNameText;
    public Text enemyPokemonLevelText;
    public Image enemyPokemonImage;


    public bool isBusy = false;

    public delegate void OnDoneDonePlayingAnimationDelegate();
    public static event OnDoneDonePlayingAnimationDelegate doneDonePlayingAnimationDelegate;

    public bool faintOwnPokemon() {
        if (isBusy) return false;
        isBusy = true;
        ownPokemonSlider.gameObject.SetActive(false);

        ownPokemonNameText.gameObject.SetActive(false);
        ownPokemonHpText.gameObject.SetActive(false);
        ownPokemonImage.gameObject.SetActive(false);

        //ownPokemonTeamPokeballs[0].sprite = pokeballEmpty;

        //switchActiveOwnPokemon(1);

        //getActiveOwnPokemon() = null;   
        isBusy = false;
        doneDonePlayingAnimationDelegate();
        return true;
    }

    public bool faintEnemyPokemon() {
        if (isBusy) return false;
        isBusy = true;

        enemyPokemonSlider.gameObject.SetActive(false);

        enemyPokemonNameText.gameObject.SetActive(false);
        enemyPokemonLevelText.gameObject.SetActive(false);
        enemyPokemonImage.gameObject.SetActive(false);

        //enemyPokemonTeamPokeballs[0].sprite = pokeballFaint;
        //enemyPokemonData = null;
        isBusy = true;
        doneDonePlayingAnimationDelegate();
        return true;
    }

}
