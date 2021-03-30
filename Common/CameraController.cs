using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    public Transform camera_target;
    public float distance = 5.0f;
    public float height = 10.0f;
    public float rotateValue = 5.0f;

    
    public Transform camera_transform;
    public Camera main_camera;

    public Vector2 fovMinMax;
    bool zoom = true;

    public int cameraView = 0;
    Coroutine coroutine;

    public GameObject startPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
        //target = GameController.instance.nowPlayer;
        camera_transform = main_camera.transform;
        
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ChangeCameraView();//if input keycode , change cameraview
        //Debug.Log(main_camera.fieldOfView);
        switch (cameraView)//cameraView for change camera angle
        {
            case 0:
                StartLevel();
                break;
            case 1://is Character Move
                PlayerMove();
                break;
            case 2:
                MiniMapCamera();
                break;
            default:
                break;
            
        }

        
        
    }
    

    void PlayerMove()
    {
        camera_target = GameController.instance.nowPlayer.transform;
        //Debug.Log(camera_target.position);
        //float currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, 0, rotateValue * Time.deltaTime);
        //Debug.Log("current angle : " + currentAngle);//current angle is always 0
        //Quaternion rotateAngle = Quaternion.Euler(0, currentAngle, 0);

        //Debug.Log("x : " + (Vector3.forward * distance).x + "  y : " + (Vector3.forward * distance).y
        //    + "  z : " + (Vector3.forward * distance).z);

        transform.position = camera_target.position - (Vector3.forward * 5)
            + (Vector3.up * 10);
        
        camera_transform.position = transform.position;
        camera_transform.LookAt(camera_target.position);


        //_transform.position = target.position + (Vector3.up * height);
        //transform.LookAt(target);

    }

    void StartLevel()
    {
        
        

        if(coroutine == null)
        {

            camera_target = GameController.instance.mapLoader.transform;
            coroutine = StartCoroutine(MapScanning());
        }
            

    }

    void ChangeCameraView()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            cameraView = 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(coroutine == null && !zoom)
                coroutine = StartCoroutine(FadeIn());
            
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(coroutine == null && zoom)
                coroutine = StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeIn()
    {
        zoom = true;
        float t = 0;
        while(t <= 0.3f)
        {
            t += Time.deltaTime;
            float fov = Mathf.Lerp(fovMinMax.y, fovMinMax.x, t / 0.3f);
            main_camera.fieldOfView = fov;

            yield return new WaitForSeconds(Time.deltaTime);

            
        }

        coroutine = null;

    }
    IEnumerator FadeOut()
    {
        zoom = false;
        float t = 0;
        while (t <= 0.3f)
        {
            t += Time.deltaTime;
            float fov = Mathf.Lerp(fovMinMax.x, fovMinMax.y, t / 0.3f);
            main_camera.fieldOfView = fov;

            yield return new WaitForSeconds(Time.deltaTime);


        }

        coroutine = null;
    }
    IEnumerator MapScanning()//top down camera move
    {
        Map map = GameController.instance.GetMap();
        
        float t = 0;
        
        float height = map.mapsizeH;
        float width = map.mapsizeW;
        float now_height;

        //mapLoader scanning
        while (t <= height)
        {
            t += Time.deltaTime*5;

            now_height = Mathf.Lerp(height, 0, t / height);

            transform.position = new Vector3((width-1) / 2, 10, now_height);
            camera_transform.position = transform.position;
            yield return new WaitForSeconds(Time.deltaTime);
        }


        yield return new WaitForSeconds(0.5f);


        if (map.parfait)
        {
            t = 0;
            Vector3 start = transform.position;
            Vector3 end = GameController.instance.mapLoader.parfaitBlock[0].transform.position;//map.parfaitBlock[0].transform.position;

            //Look Parfait
            transform.position = end - (Vector3.forward * 2)
            + (Vector3.up * 4);
            camera_transform.position = transform.position; 
            camera_transform.LookAt(end);
            
            yield return new WaitForSeconds(1f);
            //for(int i = 0; i<4; i++)
            //{
            //    mapLoader.parfaitList[i].SetActive(true);
            //}
            yield return new WaitForSeconds(1.5f);

            //Look Player
            Vector3 now = transform.position;
            camera_target = GameController.instance.nowPlayer.transform;
            
           

            Vector3 player_pos = camera_target.position - (Vector3.forward * 5)
            + (Vector3.up * 10);
            
            while (t <= 0.3f)
            {
                //Debug.Log(t);
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(now, player_pos, t / 0.3f);
                camera_transform.position = transform.position;
                
                camera_transform.LookAt(Vector3.Lerp(end,camera_target.position,t/0.3f));

                yield return new WaitForSeconds(Time.deltaTime);
            }
            camera_transform.position = player_pos;
            camera_transform.LookAt(camera_target.position);
           
            yield return new WaitForSeconds(1f);
        }
        startPanel.SetActive(false);
        GameController.instance.GameStart();
        Debug.Log("end scan");
        cameraView = 1;
        coroutine = null;
        //GameController.instance.GameStart();
    }

    void MiniMapCamera()
    {
        camera_target = GameController.instance.mapLoader.minimapTarget;
        transform.position = camera_target.position
           + (Vector3.up * 10);

        camera_transform.position = transform.position;
        camera_transform.LookAt(camera_target.position);
    }

    //Ui button 
    public bool MiniMapView(bool mini)
    {
        
        if (mini)//true 면 원래상태로 복
        {
           
            mini = false;
            main_camera.fieldOfView = fovMinMax.x;
            cameraView = 1;
        }
        else
        {
            mini = true;
            main_camera.fieldOfView = fovMinMax.y;
            cameraView = 2;
        }

        return mini;
    }

    public void SkipMapScanning()
    {
        startPanel.SetActive(false);

        if(coroutine != null)
            StopCoroutine(coroutine);

        GameController.instance.GameStart();
        cameraView = 1;
        coroutine = null;
    }
}
