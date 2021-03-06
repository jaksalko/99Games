﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public enum State
{
	Idle,//no interaction
	Master,//in interaction and state is master
	Slave//in interaction and state is slave...
}

public struct PlayerProperty
{
	private Vector3Int _position;
	
	public Vector3Int Position
	{
		get => _position;
		set => _position = value;
	}

	private int _blockTemp;

	public int BlockTemp
	{
		get => _blockTemp;
		set => _blockTemp = value;
	}
	private int _direction;

	public int Direction
	{
		get => _direction;
		set => _direction = value;
	}
	private State _state;

	public State State
	{
		get => _state;
		set => _state = value;
	}
	private bool _isLock;

	public bool Lock
	{
		get => _isLock;
		set => _isLock = value;
	}
	
	private bool _onCloud;
	
	public bool Cloud
	{
		get => _onCloud;
		set => _onCloud = value;
	}
	private int _floor; public int Floor
	{
		get => _floor;
		set
		{
			if (value < 1 || value > 3) return;
			
			
			_floor = value;
			_position.y = value;
			Debug.Log(_floor);
			if (_floor == 1) _blockTemp = BlockNumber.character;
			else _blockTemp = BlockNumber.upperCharacter;

		}
	}
	 
	public PlayerProperty(Vector3Int pos , int dir , State state , bool isLock , bool onCloud , int temp)
	{ 
		_position = pos;
		_direction = dir;
		_state = state;
		_isLock = isLock;
		_onCloud = onCloud;
		_floor = pos.y;
		_blockTemp = temp;
		
		
	}
}
public class Player : Block , IMoveable
{
	[SerializeField]
	
    Vector3[] dir = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };


    [Header("Components")]
    [SerializeField]
    CharacterController cc;
    [SerializeField]
    CheckAnimationState stateMachine;
    [SerializeField]
    AudioSource playerAudio;
    [SerializeField]
    Animator animator;


    [Header("Character Infomation")]
    public float speed;   
    bool isPlayingParticle = false;

    public int actionnum;   
    public int getDirection = -1;//초기 입력된 방향 map.getdestination 에서 Cloud를 만나면 변경됨.(Player 가 사용하지않)
    public int direction = -1;//수정되는 방향값(targetposition에 의해)

    //public List<Tuple<Vector3, int>> targetPositions = new List<Tuple<Vector3, int>>();
    

    [Header("Character State")]
    [SerializeField]
    bool isMoving = false; public bool Moving() { return isMoving; }
    [SerializeField]
    public bool isActive = false;

   
    [SerializeField] 
    public State state { get; set; }    
    [SerializeField]
    public bool onCloud = false;
    public bool isLock = false;
    public bool stateChange = false;
    public int temp;

    [Header("Character Sound")]
	public AudioClip crashSound;
	public AudioClip departureSound;
	public AudioClip fallSound;
	public AudioClip slideSound;
    
	private bool isSlideSoundPlaying;

    [Header("Character Particle System")]
	public GameObject bumpParticle;
    
    
	
    
    [SerializeField]
    public Player other;


    void AnimationEnd()
    {
        //Debug.Log("Animation End...");
        animator.SetInteger("action", 0);
        actionnum = 0;
        
	}

    public override void Init(int block_num, int style)
    {
        base.Init(block_num, style);
    }


    void Start()
    {
        state = State.Idle;


        cc = GetComponent<CharacterController>();
        playerAudio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        stateMachine = animator.GetBehaviour<CheckAnimationState>();
        
        stateMachine.player = this;
        stateMachine.ActionEnd += AnimationEnd;

        
            

    }
    public void Move(Map map, int direction)//call by GameController Command
    {
       
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))//remove this
        //{
            //isLock = false;
        this.direction = getDirection = direction;

        if (state == State.Slave)
        {
            Debug.Log("slave move");
            state = State.Idle;
            other.state = State.Idle;
            transform.SetParent(null);

            /*if (other.upstair)
            {
                //thirdFloor = true;
            }*/

            playerAudio.Stop();

        }

        Debug.Log("move direction : " + direction);
        transform.rotation = Quaternion.Euler(new Vector3(0f, direction * 90, 0f));

        //CharacterMove(map.map);

        //map.SetBlockData((int)transform.position.x, (int)transform.position.z, temp);                
        //map.GetDestination(this, transform.position);

        
        isMoving = true;

        //}
    }

  


    public void SetPosition(Vector3 startpos)
    {
        transform.position = startpos;

        switch(startpos.y)
        {
            case 1:
                temp = BlockNumber.normal;
                break;
                
            case 2:
                temp = BlockNumber.upperNormal;
                break;
        }
        
    }

    

    void CharacterControllerMovement()
    {


        if (cc.isGrounded)  // 바닥에 붙어있으면 움직임
        {
            Debug.Log("dir : " + direction);
            cc.Move(speed * Time.deltaTime * dir[direction]);
        }
        else                // 바닥이 없으면 떨어짐 (여기다 쿵! 넣으면되는데 지금 잘 작동이 안 되서 넣으면 안 됨)
        {
            Debug.Log("is not grounded!!!!");
            cc.Move(speed*1.3f * Time.deltaTime * Vector3.down);
        }

        if (direction % 2 == 0)//vector.forward , vector.back ==> z 움직임 
        {
           

            int x = Mathf.RoundToInt(transform.position.x);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
            Debug.Log("X : " + transform.position.x);
        }
        else
        {
           

            int z = Mathf.RoundToInt(transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
            Debug.Log("Z : " + transform.position.z);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameController.Playing)
            return;
        
        if (isMoving)
        {
           /*
            CharacterControllerMovement();
            
            float distance = Vector3.Distance(transform.position, targetPositions[0].Item1);
            
            if (distance < 0.25f)//arrive condition
            {
                transform.position = targetPositions[0].Item1;
                Debug.Log(name + " : " + "Arrive... target position : " + targetPositions[0].Item1 + "  distance : " + distance);


                if (targetPositions.Count != 1)
                {
                    direction = targetPositions[0].Item2;
                    transform.rotation = Quaternion.Euler(new Vector3(0f, direction * 90, 0f));
                    targetPositions.RemoveAt(0);

                    return;
                }
                else//last target position
                {
                    direction = targetPositions[0].Item2;
                    transform.rotation = Quaternion.Euler(new Vector3(0f, direction * 90, 0f));
                    targetPositions.RemoveAt(0);
                }

                
                isMoving = false;

                animator.SetBool("move", false);
                
 
				

                GameController.instance.RemainCheck();

                

                if (state == State.Master)
                {
                    other.temp = (transform.position.y == 1) ? BlockNumber.character : BlockNumber.upperCharacter;

                    //change character?

                    //other == State.Slave
                    if(stateChange)
                    {
                        stateChange = false;
                        other.Move(GameController.instance.GetMap(), direction);
                    }
                        
                    //둘 다 Idle로 변경됨.
                }
                else//Idle 상태 - slave는 움직일때 풀림
                {
                    Vector3 otherPos = other.transform.position;
                    Vector3 myPos = transform.position;
                    //master 에서 other.move로 idle상태로 other이 움직이기 때문에 모든 move는 else로 들어오게 되어있음.
                    //Other에서 불리느냐 this에서 불리느냐의 차이
                    if(otherPos.x == myPos.x && otherPos.z == myPos.z)
                    {
                        if(myPos.y < otherPos.y)
                        {
                            state = State.Master;
                            other.state = State.Slave;

                            other.transform.SetParent(transform);
                        }
                        else
                        {
                            state = State.Slave;
                            other.state = State.Master;

                            transform.SetParent(other.transform);
                        }
                    }
                    
                }


				


			}
           */
        }
        

    }


    /*
    private void LateUpdate()
    {
        animator.SetBool("move", isMoving);
        
        if (isMoving)
        {
			if (actionnum == 5)//drop
            {
				if (!playerAudio.isPlaying)
				{
					playerAudio.loop = true;
					playerAudio.clip = slideSound;

					playerAudio.Play();
					isSlideSoundPlaying = true;
				}

				float distance = Vector3.Distance(transform.position, targetPositions[0].Item1 + new Vector3(0, 1, 0));
                if (distance < 1f)//거리가 1일때부터 드랍 애니메이션 실행 , 움직이고 있던상태에서 애니메이션 실행
                {
                    animator.SetInteger("action", actionnum);//actionnum = 5 drop...

					if (isSlideSoundPlaying)
					{
						playerAudio.Stop();
						isSlideSoundPlaying = false;
					}
					if (!playerAudio.isPlaying)
					{
						playerAudio.loop = false;
						playerAudio.clip = fallSound;

						playerAudio.Play();
					}
				}
            }
			else
			{
				if (!playerAudio.isPlaying)
				{
					playerAudio.loop = true;
					playerAudio.clip = slideSound;   

					playerAudio.Play();
					isSlideSoundPlaying = true;
				}
			}
            
        }
        else//isMoving == false
        {
			if(isSlideSoundPlaying)
			{
				playerAudio.Stop();
				isSlideSoundPlaying = false;
			}

			if (actionnum !=5)//이미 전에 실행해서 드랍만 예외처리
			    animator.SetInteger("action", actionnum);
            
			//이동 시 발생하는 particle control
			switch(actionnum)
			{
				case 3:
					if (!isPlayingParticle)
					{
//						Debug.Log("play crash particle");
						// crashParticle.Play();
						isPlayingParticle = true;
					}

					break;
				case 4:
					if (!isPlayingParticle)
					{
//						Debug.Log("play bump particle");
						bumpParticle.SetActive(true);
						Invoke("BumpParticleControl", 4.5f);
						//bumpParticle.Play();
						isPlayingParticle = true;
					}

					if (!playerAudio.isPlaying)
					{
						playerAudio.loop = false;
						playerAudio.clip = crashSound;

						playerAudio.Play();
					}
					break;
				default:
					break;
			}
			
		}
        

    }

    */

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Parfait"))
        {
            ParfaitBlock parfait = collider.GetComponent<ParfaitBlock>();
            if (parfait.sequence < GameController.ParfaitOrder)
            {
                
				Debug.Log("get parfait...");
				parfait.ActiveNextParfait();
            }
            else
            {
                Debug.Log("pass inactive parfait...");
            }
        }
    }

	private void BumpParticleControl()
	{
		bumpParticle.SetActive(false);
	}

   
}
