using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventQueueSystem : MonoBehaviour
{
    public PokemonManager pokemonManager;
    
    //private bool isReady = false;

    private Queue<BattleEvent> battleEventQueue;

    public void initiateQueueSystem() {
        pokemonManager.isReady = true;
        battleEventQueue = new Queue<BattleEvent>();
        StartCoroutine(messageProcessor());
    }
    
    public void enqueueEvent(BattleEvent battleEvent) {
        Debug.Log("Event enqueued");
        
        battleEventQueue.Enqueue(battleEvent);
    }
    
    private IEnumerator messageProcessor() {
        while (true) {
            if (battleEventQueue.Count > 0 && pokemonManager.isReady) {
                pokemonManager.isReady = false;
                BattleEvent battleEvent = battleEventQueue.Dequeue();

                if (battleEvent.GetType() == typeof(TextMessageEvent)) {
                    
                    StartCoroutine(pokemonManager.displayMessage(((TextMessageEvent)battleEvent).textMessage));

                } else if (battleEvent.GetType() == typeof(MoveEvent)) {
                    
                    StartCoroutine(pokemonManager.slowlyReduceHp(((MoveEvent)battleEvent).pokemon));

                } else if (battleEvent.GetType() == typeof(FaintEvent)) {
                    
                    StartCoroutine(pokemonManager.faintPokemon(((FaintEvent)battleEvent).faintTarget));
                }

            } 
            yield return new WaitForSeconds(0.5f);
        }
    }

    

}
