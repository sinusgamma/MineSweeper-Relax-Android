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

public class InputTouchManager : MonoBehaviour
{

    private cameraManager cameraManager; // containes the zoom and pan

    private float inpBegT;
    private float inpEndT;
    private Tile tileUsed;
    private float inputThresTime = 0.4f;  // the time for longpress
    private float inputThresDist = 15.0f;  // max distance during inputThresTime to allow flag
    private bool isLong;  // is the touch long?
    private bool isFar;  // is the touch distance far enough to not be click or long?
    private float maxShift = 0.0f;  // max distance from begPosFT during a touch

    private bool sentLongInput;

    private Vector2 begPosFT;  // beginning position of first touch

    private RaycastHit2D hit;  // hited object on click


    // Use this for initialization
    void Start()
    {
        cameraManager = Camera.main.GetComponent<cameraManager>();  // get the cameraManager class

        inputThresTime = PlayerPrefs.GetFloat("FlaggingTime");  // set the flagging time
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !IsPointerOverUIObject()) // only do if there is touch and not above UI object
        {
            Touch touchFirst = Input.GetTouch(0);  // use only the first touch most of the time


            /******************************
            1 TOUCH: LONG-TAP and PAN - with Tile and CameraManager class
            ******************************/
            // use one touch for tap, long and pan
            if (Input.touchCount == 1)
            {
                // do at the beginning of the input - get the tapped tile
                if (touchFirst.phase == TouchPhase.Began)  
                {
                    inpBegT = Time.time;  // check the time off the input beginning
                    begPosFT = touchFirst.position;  // beginning position of first touch

                    hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  // the object clicked
                    if (hit.collider != null && hit.transform.gameObject.GetComponent<Tile>())  // if we aim a tile
                    {
                        tileUsed = hit.transform.gameObject.GetComponent<Tile>();  // the tile first interacted during the input - so the tile clicked
                    }
                }

                // here we set-unset the flag
                if (touchFirst.phase == TouchPhase.Stationary)
                {
                    maxShift = Mathf.Max(Vector2.Distance(begPosFT, touchFirst.position), maxShift);  // count maxShift differentiate between move and long/short

                    // check if during this touch the fagging was sent / the time is long enough for flagging / the maxshift is short enough for flagging
                    if (sentLongInput == false && Time.time - inpBegT > inputThresTime && maxShift < inputThresDist)
                    {
                        isLong = true;
                        if (tileUsed != null)  // if we reached a tile
                        {
                            tileUsed.getInput(isLong);  //  if pressed time over the treshold call the function immediately - don't make wait the user
                            sentLongInput = true;
                        }
                    }
                }

                //PAN GRID - with cameraManager class
                if (touchFirst.phase == TouchPhase.Moved)
                {
                    maxShift = Mathf.Max(Vector2.Distance(begPosFT, touchFirst.position), maxShift);  // count maxShift differentiate between move and long/short
                    if (maxShift >= inputThresDist)  // don't make too small moves
                    {
                        Vector2 delta = touchFirst.deltaPosition;  // the movement in the last frame
                        cameraManager.cameraMoveTouch(delta);  // call the function in cameraManager with last delta
                    }
                }
            }

            /******************************
            2 TOUCHES: ZOOM - with cameraManager class
            ******************************/
            if (Input.touchCount == 2)
            {
                Touch touchSecond = Input.GetTouch(1);
                cameraManager.cameraZoomTouch(touchFirst, touchSecond);
                isLong = true;  // disable taps when two finger tapped the screen
            }

            /******************************
            TAP - with Tile class and Resets
            ******************************/
            // if touch ended, and time and distance is small enough use normal click
            if (touchFirst.phase == TouchPhase.Ended || touchFirst.phase == TouchPhase.Canceled)
            {
                if (isLong == false && maxShift < inputThresDist)  // check for normal tap - no long tap and maxshift was small enough
                {
                    if (tileUsed != null)  // if we reached a tile
                    {
                        tileUsed.getInput(isLong);  //  if press time was short, call the function, when the press is released
                    }
                }

                isLong = false;  //  new click must recalculate presstime
                sentLongInput = false;  // can send new message at next click
                tileUsed = null;  // forget the clicked item
                maxShift = 0.0f;  // next touch, next maxShift
            }

        }  // END if (Input.touchCount > 0)
    }  // END Update


    // detects if the pointer is over an UI object - this is used to disable touch under the UI -  if (!IsPointerOverUIObject()) . . .
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


}  // END Class