using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventQueueSystem : MonoBehaviour
{
    public BattleControlsComponent battleControlsComponent;
    
    //private bool isReady = false;

    private Queue<BattleEvent> battleEventQueue;

    public void initiateQueueSystem() {
        battleControlsComponent.isReady = true;
        battleEventQueue = new Queue<BattleEvent>();
        StartCoroutine(messageProcessor());
    }
    
    public void enqueueEvent(BattleEvent battleEvent) {

        battleEventQueue.Enqueue(battleEvent);
    }
    
    private IEnumerator messageProcessor() {
        while (true) {
            if (battleEventQueue.Count > 0 && battleControlsComponent.isReady) {
                battleControlsComponent.isReady = false;
                BattleEvent battleEvent = battleEventQueue.Dequeue();

                if (battleEvent.GetType() == typeof(PlayerMustWaitEvent)) {
                    battleControlsComponent.blockPlayer();
                    
                } else if (battleEvent.GetType() == typeof(TextMessageEvent)) {
                    
                    battleControlsComponent.displayMessage(((TextMessageEvent)battleEvent).textMessage);

                } else if (battleEvent.GetType() == typeof(MoveEvent)) {
                    
                    battleControlsComponent.slowlyReduceHp(((MoveEvent)battleEvent).pokemon);

                } else if (battleEvent.GetType() == typeof(FaintEvent)) {
                    
                    //StartCoroutine(pokemonManager.faintPokemon(((FaintEvent)battleEvent).faintTarget));
                }

            } 
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void callerIsDone () {
        battleControlsComponent.isReady = true;
    }
    

}
