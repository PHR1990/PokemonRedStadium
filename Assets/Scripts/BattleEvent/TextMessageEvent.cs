using UnityEngine;
using UnityEditor;

public class TextMessageEvent : BattleEvent
{
    
    public string textMessage;

    public TextMessageEvent(string textMessage) {
        this.textMessage = textMessage;
    }
}