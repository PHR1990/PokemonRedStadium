using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public Button moveBtn;
    public Button actionBtn;
    
    public GameObject player;

    public Text text;

    private bool playerMoving;

    private PlaceInMap currentPlace = PlaceInMap.PalletTown;

    private Vector3 palletCity = new Vector3(-3, -1, 0);
    private Vector3 viridianCity = new Vector3(-3, 0.9f, 0);
    private bool printed = false;

    private string place;
    private string action;
    // Start is called before the first frame update
    void Start()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        moveBtn.onClick.AddListener(() => {
            playerMoving = true;

            
        });
    }

    private void move() {
        if (currentPlace.Equals(PlaceInMap.PalletTown)) {
            place = "Route 1";
            action = "walking";
            if (!printed) {
                Debug.Log(player.transform.position);
                printed = true;
            }
            
            player.transform.position+=new Vector3(0,0.01f,0);
            
            if (Vector3.Distance(player.transform.position, viridianCity) < 0.1f) {
                place = "Viridian City";
                action = "waiting";
                playerMoving = false;
                currentPlace = PlaceInMap.ViridianCity;
            }
        } else {
            place = "Route 1";
            action = "walking";
            Debug.Log(player.transform.position);
            player.transform.position+=new Vector3(0,-0.01f,0);
            
            if (Vector3.Distance(player.transform.position, palletCity) < 0.1f) {
                place = "Pallet Town";
                action = "waiting";
                playerMoving = false;
                currentPlace = PlaceInMap.PalletTown;
            }
        }
        updateText();
        
    }

    private void updateText() {
        text.text = place + " - " + action;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerMoving) {
            move();
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision with" + other.name);
    }

    private enum PlaceInMap {
        PalletTown, ViridianCity
    }
}
