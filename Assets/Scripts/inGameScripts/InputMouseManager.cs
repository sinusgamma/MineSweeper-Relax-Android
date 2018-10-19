using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/*  HELP for porting
GetMouseButton(0)       = TouchPhase.Began
GetMouseButtonDown(0)   = TouchPhase.Stationary
GetMouseButtonUp(0)     = TouchPhase.End
keys                    = TouchPhase.Moved
*/

public class InputMouseManager : MonoBehaviour
{
    private cameraManager cameraManager; // containes the zoom and pan

    private float inpBegT;
    private float inpEndT;
    private Tile tileUsed;
    private float inputThres = 0.4f;
    private bool isLong;
    private bool sentLongInput;

    private RaycastHit2D hit;  // hited object on click


    // Use this for initialization
    void Start()
    {
        cameraManager = Camera.main.GetComponent<cameraManager>();  // get the cameraManager class
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {   // this disables clicks trough UI
        #if UNITY_EDITOR_WIN // if unity editor

            /****************************
            MOVING THE TABLE - with cameraManager class
            *****************************/

            // mouse + key zoom trigger
            if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
            {
                cameraManager.cameraZoomMouse();
            }

            // mouse + key pan trigger
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                cameraManager.cameraMoveKey();
            }


            /****************************
            CICKING THE TILES - Tile class
            *****************************/

            if (Input.GetMouseButtonDown(0))  // do at the beginning of input
            {
                inpBegT = Time.time;  // check the time off the input beginning

                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  // the object clicked
                if (hit.collider != null && hit.transform.gameObject.GetComponent<Tile>())  // if we aim a tile
                {
                    tileUsed = hit.transform.gameObject.GetComponent<Tile>();  // the tile first interacted during the input - so the tile clicked
                }
            }

            if (Input.GetMouseButton(0))  // check during input is permanent - long touch/press
            {
                if (Time.time - inpBegT > inputThres && sentLongInput == false)  // check if the press long enough and if there was a sent input during this click
                {
                    isLong = true;
                    tileUsed.getInput(isLong);  //  if pressed time over the treshold call the function immediately - don't make wait the user
                    sentLongInput = true;
                }
            }

            if (Input.GetMouseButtonUp(0))  // end of input
            {
                if (isLong == false)
                {
                    if (tileUsed != null)
                    {
                        tileUsed.getInput(isLong);  //  if press time was short, call the function, when the press is released - here with isLong = false
                    }
                }

                isLong = false;  //  new click must recalculate presstime
                sentLongInput = false;  // can send new message at next click
                tileUsed = null;  // forget the clicked item
            }
        #endif
        }
    }  // END Update

}  // END Class