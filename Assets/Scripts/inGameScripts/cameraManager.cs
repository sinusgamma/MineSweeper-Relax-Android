using UnityEngine;
using System.Collections;


public class cameraManager : MonoBehaviour {

    Camera cameraThis;
    private Vector2 leftBottom = new Vector2(0f, 0f);

    private float cameraSize = 8.0f;
  
    private float cameraUpperBound;  // the camera boundaries - lower and left comes from the grid left bottom (0,0)
    private float cameraLowerBound = 1.0f;
    private float cameraRightBound;
    private float cameraLeftBound = 1.0f;

    // for mouse + keys
    private float cameraSpeed = 4.0f;
    private float zoomSpeed = 4.0f;

    // for touch
    private float touchSensitivity = -1.5f;
    private float orthoZoomSpeed = 0.03f;


    private float zoomMin = 4.0f;
    private float zoomMax;
    private float zoomDefault = 8.0f; // the default camera size - zoomMax can't be smaller



    //*** move Camera by WASD - called from inputMouseManager
    public void cameraMoveKey()
    {
        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < cameraRightBound)
        {
            transform.position += Vector3.right * cameraSpeed * Time.deltaTime;  // other places cameraThis.transform.position
        }
        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > cameraLeftBound)
        {
            transform.position += Vector3.left * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow) && transform.position.y < cameraUpperBound)
        {
            transform.position += Vector3.up * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow) && transform.position.y > cameraLowerBound)
        {
            transform.position += Vector3.down * cameraSpeed * Time.deltaTime;
        }
    }


    //*** move Camera by Touch - called from inputTouchManager - delta is the movement of finger in last frame
    public void cameraMoveTouch(Vector2 delta)
    {
        float positionX = delta.x * touchSensitivity * Time.deltaTime;
        float positionY = delta.y * touchSensitivity * Time.deltaTime;

        transform.position += new Vector3(positionX, positionY, 0);

        // set the boundaries
        Vector3 limitedCameraPosition = cameraThis.transform.position;
        limitedCameraPosition.x = Mathf.Clamp(limitedCameraPosition.x, cameraLeftBound, cameraRightBound);
        limitedCameraPosition.y = Mathf.Clamp(limitedCameraPosition.y, cameraLowerBound, cameraUpperBound);
        cameraThis.transform.position = limitedCameraPosition;

    }


    //*** zoom camera with scroller - min max zoom
    public void cameraZoomMouse()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            this.cameraSize += scroll * zoomSpeed;

            if (this.cameraSize < this.zoomMin)
            {
                this.cameraSize = zoomMin;
                cameraThis.orthographicSize = this.cameraSize;
            }
            else if (this.cameraSize > this.zoomMax)
            {
                this.cameraSize = zoomMax;
                cameraThis.orthographicSize = this.cameraSize;
            }
            else
            {
                cameraThis.orthographicSize = this.cameraSize;
            }
        }
    }


    //*** zoom camera with Touch - min max zoom / zoom aimes the area between fingers
    public void cameraZoomTouch(Touch touchOne, Touch touchTwo)
    {
        Vector2 cameraViewsize = new Vector2(cameraThis.pixelWidth, cameraThis.pixelHeight);  // the viewsize

        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;  // the position of touch in the previous frame
        Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

        float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;  // distance of touches now and before
        float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

        float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;  // the change of the distance

        // this makes the zoom center between fingers
        cameraThis.transform.position += cameraThis.transform.TransformDirection((touchOnePrevPos + touchTwoPrevPos - cameraViewsize) * cameraThis.orthographicSize / cameraViewsize.y);

        cameraThis.orthographicSize += deltaMagDiff * orthoZoomSpeed;  // set camera size
        cameraThis.orthographicSize = Mathf.Clamp(cameraThis.orthographicSize, zoomMin, zoomMax);

        // this makes the zoom center between fingers
        cameraThis.transform.position -= cameraThis.transform.TransformDirection((touchOne.position + touchTwo.position - cameraViewsize) * cameraThis.orthographicSize / cameraViewsize.y);
    }


    //*** calculate the max zoom level and movement boundaries of the camera
    private void calculateCameraBoundaries()
    {
        zoomMax = Mathf.Max(8, (Grid.tilesPerColumn + 10) / 2);  // set zoomMax to the default 8 or larger

        cameraUpperBound = Grid.tilesPerColumn - 2;  // the boundaries of camera movement - not to leave the grid
        cameraRightBound = Grid.tilesPerRow - 2;
    }


    //*** gives back grid center from left bottom
    private Vector2 GridCentrum(Vector2 leftBottom, float offset, int rows, int columns)  
    {
        Vector2 center = new Vector2();

        center.x = leftBottom.x + (offset * (rows - 1)) / 2;
        center.y = leftBottom.y + (offset * (columns - 1)) / 2;

        return center;
    }


    //*** center the camera to the grid on screen
    private void CenterCamera()
    {
        GameObject grid = GameObject.Find("grid");
        if (grid != null)
        {
            Grid gridScript = (Grid)grid.GetComponent(typeof(Grid));
            Vector2 centrum = GridCentrum(leftBottom, 1.0f, Grid.tilesPerRow, Grid.tilesPerColumn);
            cameraThis.transform.position = new Vector3(centrum.x, centrum.y + 1, -10);  // centrum.y + 1 => center grid on empty area under the head
        }
    }


	// Use this for initialization
	void Start ()
    {
        cameraThis = Camera.main;
        CenterCamera();
        calculateCameraBoundaries();
    }
	
	// Update is called once per frame
	void Update () {

    }
}
