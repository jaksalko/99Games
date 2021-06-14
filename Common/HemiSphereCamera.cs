using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
public class HemiSphereCamera : MonoBehaviour
{
    
    public Vector3 target;
    public float horizontal_interval;
    public float vertical_interval;

    public Transform camera_temp_transform;
    public Transform camera_before;
    public float sensitivity;
    public float distance;
    public bool touchstream = false;
    bool isStart = false;
    public int touchValue = 0;

    Vector2 temp;

    int vertical_count = 0;

    Vector2 start_point;
    Vector2 end_point;
    public void InitializeCamera(Vector3 center)
    {
        target = center;
    }

    // Start is called before the first frame update
    void Start()
    {
        //vertical_count = 90 / (int)vertical_interval;

        
        this.UpdateAsObservable()
            .Where(_ => !touchstream)

              .Where(_ => Input.touchCount >= 1)
              .Where(_ => Input.GetTouch(0).phase == TouchPhase.Began)
              .Select(count => Input.touchCount)
              .Subscribe(count => StartCoroutine(TouchStart()));

        this.UpdateAsObservable()
            .Where(_ => touchstream)
            .Where(_ => !isStart)
            .Where(_ => touchValue >= 2)
              .Where(_ => Input.GetTouch(0).phase == TouchPhase.Moved)
              .Select(pos => Input.GetTouch(0))
              .Subscribe(pos => { start_point = pos.position;  isStart = true; });


        this.UpdateAsObservable()
            .Where(_ => touchstream)

              .Where(_ => Input.touchCount >= 1)
              .Where(_ => Input.GetTouch(0).phase == TouchPhase.Ended)
              .Select(count => Input.touchCount)
              .Select(pos => Input.GetTouch(0))
              .Subscribe(pos => { end_point = pos.position; TouchEnd(); });
              

        
        
    }

    

    void CameraMove(Vector2 touchPosition)
    {
        Vector2 gap_ = touchPosition - temp;
        
        float x = gap_.x;
        float z = gap_.y;
        /*
        camera_transform.position = new Vector3(camera_transform.position.x + gap.x, camera_transform.position.y, camera_transform.position.z + gap.y);
        temp = touchPosition;*/
        temp = touchPosition;




        camera_before.RotateAround(target,
                                    camera_before.up,
                                    x * sensitivity);
        camera_before.RotateAround(target,
                                        camera_before.right,
                                        -z * sensitivity);
        
        if(camera_before.position.y < 5 || camera_before.position.y > 10)
        {
            Debug.Log("max");
            camera_before.position = camera_temp_transform.position;
            camera_before.rotation = camera_temp_transform.rotation;
        }
        else
        {
            camera_temp_transform.position = camera_before.position;
            camera_temp_transform.rotation = camera_before.rotation;
        }
        /*
        temp_pos = temp_pos - camera_temp_transform.position;
        temp_pos = temp_pos * gap;
        camera_transform.position -= temp_pos;
        camera_transform.rotation = camera_temp_transform.rotation;
        */
        /*
        camera_temp_transform.LookAt(target);

        if (camera_temp_transform.position.y < 1 || camera_temp_transform.position.y > distance)
        {
            camera_temp_transform.position = temp_position;
            camera_transform.rotation = temp_rotation;
        }
        else
        {
            camera_transform.position = camera_temp_transform.position;
            camera_transform.rotation = camera_temp_transform.rotation;
        }
        */

        //camera_transform.LookAt(target);
    }

    IEnumerator TouchStart()//스타트는 한번 호출될수 있지1
    {
        yield return new WaitForSeconds(0.1f);
        int value = Input.touchCount;
        if (value > 0)
        {
            temp = Input.GetTouch(0).position;
            Debug.Log("began! touch count : " + Input.touchCount);
            touchValue = value;
            touchstream = true;
        }

    }

    void TouchEnd()//End는 손가락 갯수에 따라 여러번 실행될 수 있음을 유의
    {

        touchstream = false;
        
        Debug.Log("end! touch count : ");

        if(isStart)
        {
            isStart = false;
            if (Vector2.Distance(start_point, end_point) <= 30)//민감도
            {
                return;
            }
            Vector2 normalized = (end_point - start_point).normalized;


            if (normalized.x < -0.5)
            {
                MoveHorizontal(true);
                //isMove = PlayerControl(3); //left
            }
            else if (normalized.x > 0.5)
            {
                MoveHorizontal(false);
                //isMove = PlayerControl(1); //right
            }
            else
            {
                if (normalized.y > 0)
                {
                    Debug.Log("up");
                    MoveVertical(true);
                    //isMove = PlayerControl(0); //up

                }
                else
                {
                    Debug.Log("down");
                    MoveVertical(false);
                    //isMove = PlayerControl(2); // down
                }

            }
        }

        

    }

    public void MoveHorizontal(bool left)
    {
        float dir = 1;
        if (!left)
            dir *= 1;

        transform.RotateAround(target, Vector3.up, horizontal_interval * dir);

    }

    public void MoveVertical(bool up)
    {
        float dir = 1;
        if (!up)
        {
            dir *= -1;

            if (vertical_count > -6)
            {
                vertical_count--;
                transform.RotateAround(target, transform.TransformDirection(Vector3.right), vertical_interval * dir);
            }
        }
        else
        {
            if(vertical_count < 0)
            {
                vertical_count++;
                transform.RotateAround(target, transform.TransformDirection(Vector3.right), vertical_interval * dir);

            }
        }
            

        

    }
}
