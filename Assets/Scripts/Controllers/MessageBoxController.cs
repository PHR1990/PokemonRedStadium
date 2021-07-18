using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxController : MonoBehaviour {

    public delegate void OnDoneWithLastActionDelegate();
    public OnDoneWithLastActionDelegate doneWithLastActionDelegate;

    public Text messageText;
    


    public void displayWaitMessage(string msg) {
        StartCoroutine(displayWaitMessageCoroutine(msg));
    }
    private IEnumerator displayWaitMessageCoroutine(string msg) {
        
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

        doneWithLastActionDelegate();
    }

    public void hideMessageBox() {
        messageText.gameObject.SetActive(false);
    }

    public void displayAckableMessage() {

    }
    
}