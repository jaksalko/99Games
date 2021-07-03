using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.EventSystems;
using System.Linq;


public enum GameMode
{
    StageMode,
    CustomMode,
    EditorMode,
    InfiniteMode
        
}

public class GameController : MonoBehaviour
{
    public class EditorClearRequest
    {
        public UserInfo userInfo;//붕과 별사탕 업데이트
        public UserHistory userHistory;//에디터 클리어 정보와 재화 획득 기록
        public UserStage userStage;//클리어 한 에디터 맵 추가
        public EditorMap editorMap;//플레이 한 에디터 맵 업데이트

        public EditorClearRequest(UserInfo info, UserHistory history, UserStage stage, EditorMap map)
        {
            userInfo = info;
            userHistory = history;
            userStage = stage;
            editorMap = map;
        }

    }

    public class StageClearRequest
    {
        public UserInfo userInfo;//붕과 별사탕 업데이트
        public UserHistory userHistory;//에디터 클리어 정보와 재화 획득 기록
        public UserStage userStage;//클리어 한 에디터 맵 추가


        public StageClearRequest(UserInfo info, UserHistory history, UserStage stage)
        {
            userInfo = info;
            userHistory = history;
            userStage = stage;

        }

    }

    

    public static GameController instance;
    
    XMLManager xmlManager = XMLManager.ins;
    AWSManager awsManager = AWSManager.instance;
    CSVManager csvManager = CSVManager.instance;
    SoundManager soundManager;
    GameManager gameManager = GameManager.instance;
    JsonAdapter jsonAdapter = JsonAdapter.instance;

    
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
    public bool isSuccess;
    UserInfo copyInfo;
    UserHistory copyHistory;
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


    public GameMode gameMode;
    
   
    
    public int unirx_dir = -1;



    private bool isPlaying;
    public static bool Playing
    {
        get => instance.isPlaying;
        set => instance.isPlaying = value;
    }

    public float startTime, endTime;



    

    [SerializeField]
    Vector2 down;
    [SerializeField]
    Vector2 up;
    [SerializeField]
    bool click;

    InputHandler handler;
    public MoveCommand moveCommand;
    public TutorialManager tutorialManager;

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

        this.ObserveEveryValueChanged(x => parfaitOrder)
            .Subscribe(x => ui.ParfaitDone(parfaitOrder));
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


        this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.UpArrow))
               .Subscribe(_ => { MakeMoveCommand(0); click = false; });

        this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.RightArrow))
               .Subscribe(_ => { MakeMoveCommand(1); click = false; });

        this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.DownArrow))
               .Subscribe(_ => { MakeMoveCommand(2); click = false; });

        this.UpdateAsObservable()
               .Where(_ => Input.GetKeyDown(KeyCode.LeftArrow))
               .Subscribe(_ => { MakeMoveCommand(3); click = false; });



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
    public void MakeMoveCommand(int unirx_dir)
    {
        

        //make command;
        if (!nowPlayer.Moving() && isPlaying && unirx_dir != -1)
        {
            List<Unit_Movement> movements = new List<Unit_Movement>();
            map.SetBlockData((int)nowPlayer.transform.position.x, (int)nowPlayer.transform.position.z, nowPlayer.temp);
            map.GetDestination(movements, nowPlayer, nowPlayer.transform.position, nowPlayer.transform.position);

            moveCommand = new MoveCommand(movements);

            handler.ExecuteCommand(moveCommand);
            moveCount++;
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

            ui.SetMoveCountText(moveCount, map.star_limit[2]);
            ui.revertButton.interactable = true;
        }

    }

    public void MakeMoveCommand()
    {
        unirx_dir = NormalizeSwipe();

        //make command;
        if (!nowPlayer.Moving() && isPlaying && unirx_dir != -1)
        {
            List<Unit_Movement> movements = new List<Unit_Movement>();
            map.SetBlockData((int)nowPlayer.transform.position.x, (int)nowPlayer.transform.position.z, nowPlayer.temp);
            map.GetDestination(movements,nowPlayer,nowPlayer.transform.position, nowPlayer.transform.position);

            moveCommand = new MoveCommand(movements);

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
        
        switch (gameMode)
        {
            case GameMode.StageMode:
                map = mapLoader.GenerateMap(gameManager.nowLevel);
                break;
            case GameMode.CustomMode:
                map = mapLoader.CustomPlayMap();
                break;
            case GameMode.EditorMode:
                map = mapLoader.EditorMap();
                break;
            case GameMode.InfiniteMode:
                map = mapLoader.InfiniteMap();
                break;
        }
       
            
        
        



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

    public void GameStart()//called by CameraController.cs after mapscanning...
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
        
        ui.inGame.SetActive(true);

        //Set Tutorial 
        if(SceneManager.GetActiveScene().buildIndex >= 6)
        {
            if (gameManager.nowLevel >= 0 && gameManager.nowLevel <= csvManager.islandData.tutorial)
            {
                tutorialManager.StartTutorial();
            }
            else
            {
                for (int i = 0; i < csvManager.islandData.island_start.Length; i++)
                {
                    if (csvManager.islandData.island_start[i] == gameManager.nowLevel)
                    {
                        tutorialManager.StartTutorial();
                        break;
                    }
                }
            }
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


    public void GameEnd(bool success)
    {
        isSuccess = success;
        soundManager.Mute();

        copyHistory = awsManager.userHistory.DeepCopy();
        copyInfo = awsManager.userInfo.DeepCopy();

        isPlaying = false;
        endTime = Time.time;
        Debug.Log("Game End... PlayTime : " + (endTime - startTime));

        
       
        
        if (gameMode == GameMode.CustomMode)
        {
            
            copyHistory.editor_clear++;
            
            if (gameManager.playCustomData.move > moveCount)
            {
                gameManager.playCustomData.move = moveCount;
            }
            //이미 클리어한 맵이라면 업데이트, 아니면 인서트
            UserStage newStageClear = new UserStage(copyInfo.nickname, gameManager.playCustomData.map_no, star, moveCount);
            EditorClearRequest editorClearRequest = new EditorClearRequest(copyInfo, copyHistory, newStageClear, gameManager.playCustomData);

            if (!awsManager.userEditorStage.Exists(x => x.stage_num == gameManager.playCustomData.map_no))
            {
                Debug.Log(gameManager.playCustomData.map_no);
                //최초 클리어 시
                //붕 별사탕 보상 획득
                copyInfo.boong += gameManager.playCustomData.level * 100;
                copyInfo.candy += gameManager.playCustomData.level;
                copyHistory.boong_get += gameManager.playCustomData.level * 100;

                //플레이 카운트 +1
                gameManager.playCustomData.play_count++;
                

                jsonAdapter.UpdateData(editorClearRequest,"newEditorStage", GameEndCallback);
            }
            else
            {
                //이미 클리어
                jsonAdapter.UpdateData(editorClearRequest,"updateEditorStage", GameEndCallback);


            }
            
           
            

        }
        else if (gameMode == GameMode.EditorMode)//mapLoader.editorMap 생성하기
        {
            awsManager.userHistory.editor_make++;
            jsonAdapter.UpdateData(awsManager.userHistory, "userHistory", GameEndCallback);
        }
        else//stage mode
        {
            int level = awsManager.userInfo.stage_current;
            int nowLevel = gameManager.nowLevel;//input level (select stage or play btn)

            if(isSuccess)
            {
                if (level >= csvManager.islandData.island_last[0] && PlayerPrefs.GetInt("tutorial", 0) == 0)
                {
                    PlayerPrefs.SetInt("tutorial", 1);

                }

                copyHistory.stage_clear++;
                

                if (nowLevel == level)//지금 스테이지 레벨 == 유저의 도전해야할 레벨
                {
                    Debug.Log("high level");
                    UserStage newStageClear = new UserStage(copyInfo.nickname, copyInfo.stage_current, star, moveCount);
                    copyInfo.stage_current++;
                    copyInfo.boong += 200 + csvManager.islandData.Island_Num(nowLevel) * 50;
                    copyHistory.boong_get = 200 + csvManager.islandData.Island_Num(nowLevel) * 50;
                    StageClearRequest stageClearRequest = new StageClearRequest(copyInfo, copyHistory, newStageClear);
                    jsonAdapter.UpdateData(stageClearRequest,"newStage", GameEndCallback);
                    

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


                    StageClearRequest stageClearRequest = new StageClearRequest(copyInfo, copyHistory, awsManager.userStage[nowLevel]);
                    jsonAdapter.UpdateData(stageClearRequest, "updateStage", GameEndCallback);

                }
            }
            else//fail stage
            {
                copyHistory.stage_fail++;
                jsonAdapter.UpdateData(copyHistory, "userHistory", GameEndCallback);
                //실패 시
            }

            


            
        }


    }
    
    void GameEndCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                jsonAdapter.GetAllUserData(awsManager.userInfo.nickname, UpdateUserDataCallback);

                if(gameMode == GameMode.CustomMode)
                {
                    CustomMapItem map = awsManager.editorMap.Find(x => x.itemdata.map_id == GameManager.instance.playCustomData.map_id);
                    map.itemdata = GameManager.instance.playCustomData;
                    map.play.text = map.itemdata.play_count.ToString();
                }
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }

    void UpdateUserDataCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                ui.GameEnd(isSuccess, star, snow_remain, moveCount, gameMode);
                if (gameMode == GameMode.CustomMode)
                    gameManager.retry = true;
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }








}
