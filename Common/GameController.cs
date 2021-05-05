using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    XMLManager xmlManager = XMLManager.ins;
    AWSManager awsManager = AWSManager.instance;
    CSVManager csvManager = CSVManager.instance;
    [Header("Controller Field")]
    public CameraController cameraController;
    public UiController ui;

    [Header("Game Component Field")]
    public MapLoader mapLoader;
    public Player player1;
    public Player player2;
    public Player nowPlayer;

    Map map;
    public Map GetMap() { return map; }
    public int snow_total, snow_remain;
    public int moveCount; 

    [Header ("For Achievement Datas")]
    int dropCount; // 몇번 떨어졌는지
    int crashCount; // 몇번 부딪혔는지 (캐릭터)
    int carryCount; // 몇번 업었는지
    int resetCount; // 몇번 되돌렸는지
    int crackCount; // 몇번 크래커를 부셨는지
    int cloudCount; // 몇번 솜사탕을 탔는지

    [SerializeField]
    int parfaitOrder;
    public static int ParfaitOrder
    {
        get => instance.parfaitOrder;
        set => instance.parfaitOrder = value;
    }

    public bool customMode;
    public bool editorMode;




    private bool isPlaying;
    public static bool Playing
    {
        get => instance.isPlaying;
    }

    public float startTime, endTime;



    SoundManager soundManager;
    GameManager gameManager = GameManager.instance;


    JsonAdapter jsonAdapter = JsonAdapter.instance;
    

    [SerializeField]
    Vector2 down;
    [SerializeField]
    Vector2 up;
    [SerializeField]
    bool click;

    InputHandler handler;
    public MoveCommand moveCommand;

    public GameObject tutorialManager;

    int star = 3;

    private void Awake()
    {
        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);


        handler = new InputHandler();
        

    }

    void Start()
    {
        xmlManager = XMLManager.ins;
        gameManager = GameManager.instance;
        soundManager = SoundManager.instance;
        awsManager = AWSManager.instance;
        jsonAdapter = JsonAdapter.instance;
        csvManager = CSVManager.instance;
        
        SwipeStream();
        GameSetting();
    }
    
    void SwipeStream()
    {

#if UNITY_EDITOR
        var mouseDownStream = this.UpdateAsObservable()
                .Where(_ => !EventSystem.current.IsPointerOverGameObject()
                    && !click
                    && Input.GetMouseButtonDown(0))

                .Select(_ => Input.mousePosition)
                .Subscribe(_ => { down = _; click = true; });

        var mouseUpStream = this.UpdateAsObservable()
            .Where(_ => click && Input.GetMouseButtonUp(0))

            .Select(_ => Input.mousePosition)
            .Subscribe(_ => { up = _; MakeMoveCommand(); click = false; });

#elif UNITY_ANDROID || UNITY_IOS
        var touchDownStream = this.UpdateAsObservable()
          
            .Where(_ => !click)
            .Where(_ => Input.touchCount > 0)
            .Where(_ => !EventSystem.current.IsPointerOverGameObject(0))
            .Where(_ => Input.GetTouch(0).phase == TouchPhase.Began)
            .Select(_ => Input.GetTouch(0))
            .Subscribe(_ => { down = _.position; click = true; } );

        var touchUpStream = this.UpdateAsObservable()
         
            .Where(_ => click)
            .Where(_ => Input.touchCount > 0)
            .Where(_ => Input.GetTouch(0).phase == TouchPhase.Ended)
            .Select(_ => Input.mousePosition)
            .Subscribe(_ => { up = _; MakeMoveCommand(); click = false; });
#endif

    }
    void MakeMoveCommand()
    {
        int dir = NormalizeSwipe();

        //make command;
        if (!nowPlayer.Moving() && isPlaying && dir != -1)
        {

            moveCommand = new MoveCommand(nowPlayer, map, dir);

            handler.ExecuteCommand(moveCommand);
            moveCount++;
            if(moveCount < map.star_limit[0])
            {
                star = 3;
            }
            else if(moveCount < map.star_limit[1])
            {
                star = 2;
            }
            else if(moveCount < map.star_limit[2])
            {
                star = 1;
            }
            else
            {
                star = 0;
            }

            ui.SetMoveCountText(moveCount,map.star_limit[2]);
            ui.revertButton.interactable = true;
        }

    }

    public void UndoCommand()
    {
        if (moveCount < map.star_limit[0])
        {
            star = 3;
        }
        else if (moveCount < map.star_limit[1])
        {
            star = 2;
        }
        else if (moveCount < map.star_limit[2])
        {
            star = 1;
        }
        else
        {
            star = 0;
        }
        ui.revertButton.interactable = false;
        ui.SetMoveCountText(moveCount, map.star_limit[2]);
    }

   


    int NormalizeSwipe()
    {
        if (Vector2.Distance(up, down) <= 30)//민감도
        {
            return -1;
        }
        Vector2 normalized = (up - down).normalized;


        if (normalized.x < -0.5)
        {
            return 3;
            //isMove = PlayerControl(3); //left
        }
        else if (normalized.x > 0.5)
        {
            return 1;
            //isMove = PlayerControl(1); //right
        }
        else
        {
            if (normalized.y > 0)
            {
                return 0;
                //isMove = PlayerControl(0); //up

            }
            else
            {
                return 2;
                //isMove = PlayerControl(2); // down
            }

        }


    }
    void GameSetting()
    {
        // map 생성
        if (customMode)
        {
            map = mapLoader.CustomPlayMap();
        }
        else if (editorMode)
        {
            map = mapLoader.EditorMap();
        }
        else
            map = mapLoader.GenerateMap(gameManager.nowLevel);//map 생성



        //데이터 초기화 (Remain / Total / MoveCount)
        snow_total = RemainCheck();
        moveCount = 0;
        // player.FindPlayer 가 실행되면 자동으로 2개가 사라짐 이 전까지는 remain == total
        // 실행위치는 GameStart CameraController에 의해서 실행됨.


        //character 위치에 맵 데이터가 노멀블럭으로 되어있는데 캐릭터 데이터로 전환 ? **데이터가 캐릭터면 바꿀필요 없음
        //체크 true로 변경
        //snow_remain 변경?
        int AposX = (int)map.startPositionA.x;
        int AposZ = (int)map.startPositionA.z;

        int BposX = (int)map.startPositionB.x;
        int BposZ = (int)map.startPositionB.z;

        map.UpdateCheckArray(width: AposX, height: AposZ, true);//character position check true
        map.UpdateCheckArray(width: BposX, height: BposZ, true);//character position check true

        snow_remain = RemainCheck();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i].GetComponent<Player>();
            player.enabled = true;

            if (player1 == null)
            {
                player1 = player;
                nowPlayer = player;
            }
            else
            {
                player2 = player;
                player1.other = player;
                player2.other = player1;
            }
        }



            //캐릭터 배치 (active)
            player1.SetPosition(
           startpos: map.startPositionA);

        player2.SetPosition(
            startpos: map.startPositionB);



        

        //카메라 활성화
        cameraController.gameObject.SetActive(true);

        
    }


    public int RemainCheck()//남은 눈 체크 --> 이동
    {
        int remain = 0;

        for (int i = 0; i < map.mapsizeH; i++)
        {
            for (int j = 0; j < map.mapsizeW; j++)
            {
                if (!map.check[i, j])
                {
                    remain++;
                }
            }
        }

        if (remain == 0) GameEnd(true);
        else
        {
            snow_remain = remain;
            ui.SetRemainText(snow_remain, snow_total);
            if(moveCount == map.star_limit[2]) GameEnd(false);
            
            
        }

        return remain;
    }





    public void SetPlaying(bool play)
    {
        isPlaying = play;
    }

    public void GameStart()//called by cameracontroller.cs after mapscanning...
    {

        //if(!simulating)
        ui.SetRemainText(remain: snow_remain, total: snow_total);
        ui.SetSlider(map.star_limit, map.star_limit[2]);
        ui.SetMoveCountText(moveCount, map.star_limit[2]);
        
        nowPlayer = player1;
        nowPlayer.isActive = true;


        isPlaying = true;
        startTime = Time.time;

        if (map.parfait)
        {
            ui.mission_parfait.SetActive(true);
        }
        else
        {
            ui.mission_default.SetActive(true);
        }
        ui.inGame.SetActive(true);

        

        //if tutorial mode active tutorial!
        if(PlayerPrefs.GetInt("tutorial",0) == 0)
        {
            tutorialManager.gameObject.SetActive(true);
        }
        
        if(awsManager.userInfo.stage_current == csvManager.islandData.tutorial+1 && gameManager.nowLevel == csvManager.islandData.tutorial+1)//ice 1
        {
            tutorialManager.gameObject.SetActive(true);
        }
        else if(awsManager.userInfo.stage_current == csvManager.islandData.icecream+1 && gameManager.nowLevel == csvManager.islandData.icecream+1)//beach 1
        {
            tutorialManager.gameObject.SetActive(true);
        }
        else if(awsManager.userInfo.stage_current == csvManager.islandData.beach+1 && gameManager.nowLevel == csvManager.islandData.beach+1)//cracker 1
        {
            tutorialManager.gameObject.SetActive(true);
        }
        else if(awsManager.userInfo.stage_current == csvManager.islandData.cracker+1 && gameManager.nowLevel == csvManager.islandData.cracker+1)//cotton 1
        {
            tutorialManager.gameObject.SetActive(true);
        }

        
            
    }

    #region JSON
    /*
    public void UserUpdate(int cash, int stage)//cash : +cash , stage : nowStage
    {
        UserData user = new UserData(gameManager.user.id, cash, change_heart: 0, stage);
        var json = JsonUtility.ToJson(user);
        StartCoroutine(jsonAdapter.API_POST("account/update", json, callback => {

            gameManager.user.cash += cash;
            gameManager.user.stage = stage;

        }));

    }

    public void StageClear(int stage_num, int step)//max update
    {
        StageData stage = new StageData(gameManager.user.id, stage_num, step);

        var json = JsonUtility.ToJson(stage);
        StartCoroutine(jsonAdapter.API_POST("stage/insert", json, callback => { }));
    }

    public void StageClear(int step)//update step
    {
        StageData stage = new StageData(gameManager.user.id, gameManager.nowLevel, step);

        var json = JsonUtility.ToJson(stage);
        StartCoroutine(jsonAdapter.API_POST("stage/update", json, callback => { }));
    }
    */
    #endregion


    public void GameEnd(bool isSuccess)
    {
        soundManager.Mute();

        UserHistory userHistory = awsManager.userHistory;
        UserInfo userInfo = awsManager.userInfo;

        isPlaying = false;
        endTime = Time.time;
        Debug.Log("Game End... PlayTime : " + (endTime - startTime));

        ui.GameEnd(isSuccess,star,snow_remain,moveCount, customMode, editorMode);
       

        if (customMode)
        {
            

        }
        else if (editorMode)//mapLoader.editorMap 생성하기
        {
            
        }
        else//stage mode
        {
            int level = awsManager.userInfo.stage_current;
            int nowLevel = gameManager.nowLevel;//input level (select stage or play btn)

            if(isSuccess)
            {
                userHistory.stage_clear++;
                

                if (nowLevel == csvManager.islandData.tutorial)
                {
                    PlayerPrefs.SetInt("tutorial",1);
                    
                }

                

                if (nowLevel == level)//지금 스테이지 레벨 == 유저의 도전해야할 레벨
                {
                    Debug.Log("high level");
                    UserStage newStageClear = new UserStage(userInfo.nickname, userInfo.stage_current, star, moveCount);

                    awsManager.userStage.Add(newStageClear);
                    jsonAdapter.CreateUserStage(newStageClear, WebCallback);

                    userInfo.stage_current++;
                    userInfo.boong += 200 + csvManager.islandData.Island_Num(nowLevel) * 50;

                }
                else
                {
                    if (awsManager.userStage[nowLevel].stage_star < star)
                    {
                        awsManager.userStage[nowLevel].stage_star = star;
                    }
                    if (awsManager.userStage[nowLevel].stage_move > moveCount)
                    {
                        awsManager.userStage[nowLevel].stage_move = moveCount;
                    }

                    
                    jsonAdapter.UpdateData(awsManager.userStage[nowLevel], "userStage", WebCallback);
                    
                }
            }
            else
            {
                userHistory.stage_fail++;

                //실패 시
            }

            //xmlManager.SaveItems();
            jsonAdapter.UpdateData(userInfo, "userInfo", WebCallback);
            jsonAdapter.UpdateData(userHistory, "userHistory",WebCallback);


            
        }


    }

    void WebCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                //
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }








}
