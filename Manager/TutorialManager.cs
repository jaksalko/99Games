using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class Progress
{
    public Image target_image;
    public Text target_text;
    public string content;
    public Sprite emotion;
}

[Serializable]
public class Tutorial_Progress : Progress
{
    public Tutorial_Progress(Image target_image,Sprite emotion, Text target_text, string content)
    {
        this.target_image = target_image;
        this.target_text = target_text;
        this.content = content;
        this.emotion = emotion;
    }
}

[Serializable]
public class TutorialStageProgress : Progress
{
    
    public TutorialStageProgress(Text target_text, string content)
    {
        this.target_text = target_text;
        this.content = content;
    }



}

public class TutorialManager : MonoBehaviour
{
    public Transform baseCanvas;
    public Transform mainCanvas;

    public Transform tutorialCanvas;

    public Image left_image;
    
    public Image right_image;
    public Text left_text;
    public Sprite left_smile;
    public Sprite left_embar;
    public Sprite left_cute;

    public Text right_text;
    public Sprite right_idle;
    public Sprite right_angry;
    public Sprite right_shed;

    public Transform creditPanel;
    public Transform[] levelPanel;
    public Transform startButton;

    public Transform[] islandButton; //tutorial island
    public Transform[] level_island;//level scene
    public Transform[] level_star_gage;//level scene
    public Transform[] level_level_list;//level scene

    public Text rpText;
    public Text bpText;

    public Player bpPlayer;
    public Player rpPlayer;
    public UiController uiController;
    public Camera inGameCamera;
    XMLManager xmlManager = XMLManager.ins;


    #region Scenario

    private string[] left_script_credit;


    string[] right_script_credit;

    string[] left_script_gamestart;

    string[] right_script_gamestart;

    string[] left_script_end_tutorial_1;

    string[] left_script_end_tutorial_2;
    string[] right_script_end_tutorial_1;

    string[] right_script_end_tutorial_2;


    string[] bp_tutorial_stage_3;
    string[] bp_tutorial_stage_2;

    string[] bp_tutorial_stage_1;

    string[] bp_tutorial_stage_4;

    string[] bp_tutorial_stage_5;

    string[] rp_tutorial_stage_3;

    string[] rp_tutorial_stage_2;
    string[] rp_tutorial_stage_1;

    string[] rp_tutorial_stage_4;

    string[] rp_tutorial_stage_5;

    string[] bp_icecream;

    string[] bp_parfait;

    string[] bp_cracker;

    string[] rp_icecream;

    string[] rp_parfait;

    string[] rp_cracker;
    #endregion

    bool trigger = false; // false is left turn
    public List<Progress> progresses = new List<Progress>();

    // Start is called before the first frame update
    void Awake()
    {
        left_script_credit = new string[]
        {"안녕! 나는 핑이라고 해.",//laugh
    "네가 우리와 함께 디저트 월드를"+Environment.NewLine+"청소하는 친구구나! 이름이….",//laugh
    "펭! 먼저 인사하는 게 어딨어!"+Environment.NewLine+"같이 하기로 했잖아!",//embar
    "반, 반가워 "+PlayerPrefs.GetString("nickname","pingpengboong")+"!",
    "잠깐만, 잠깐만! "+PlayerPrefs.GetString("nickname","pingpengboong")+"한테 설명 먼저 하고!",//cute
    "너무 급해, 펭!",
    "끙….",//embar
    PlayerPrefs.GetString("nickname","pingpengboong")+"! 여긴 로비 구역이야.",//cute
    "우리가 담당한 섬들을 여기서 선택할 수 있어.",
    "아직이야 펭! 너무 급하다니깐!",//cute
    "여기 위를 보면, 우리의 주식인 ‘붕’이랑",
    "생명력인 ‘하트’를 얼만큼 소지했는지 볼 수 있어!",
    "우리는 ‘하트’가 있어야 일 할 수 있다는 걸 잊지마!"};

        right_script_credit = new string[]
        {"나는 펭.",//idle
    PlayerPrefs.GetString("nickname","pingpengboong")+". 반가워.",//idle
    "빨리 일을 시작하도록 할까.",//idle
    "네가 너무 느린 거겠지, 핑.",//shed
    "그 밖에 다양한 기능도 있지."+Environment.NewLine+"이제 일을 하러 가볼….",//idle
    "(붕…. 많이 벌고 싶다.)"};//shed

        left_script_gamestart = new string[]
        {"벌써 시작하는 거야?",
    "맞다, 맞다! "+PlayerPrefs.GetString("nickname","pingpengboong")+"은 처음이니까!",
    "그럼, 바로 시작해볼까!"

    };

        right_script_gamestart = new string[]
            {"제일 중요한 걸 잊었잖아.",
    "이건 PLAY 버튼."+Environment.NewLine+"일을 시작하고 싶으면 여기를 눌러.",
    PlayerPrefs.GetString("nickname","pingpengboong")+". 튜토리얼 섬으로 가자.",
    "어떻게 하는지 가르쳐 줄게."
        };

        left_script_end_tutorial_1 = new string[]
            {"후, 힘들었다.",
    PlayerPrefs.GetString("nickname","pingpengboong")+", 어때? 할 만한 것 같아?",
    "지금 조금 힘들더라도, 금방 적응 될 거야!",
    "걱정마, 난 훈련된 마쉬멜로우 펭귄인 걸!",
    PlayerPrefs.GetString("nickname","pingpengboong")+". 다른 섬으로 떠날 준비는 되었지?",
    "섬과 섬 사이를 이동하는 법을 알려줄게!",
    "가운데 보이는 섬을 눌러봐!"};

        left_script_end_tutorial_2 = new string[]
            {"처음 보는 공간이지? 앞으로 익숙해질 거야.",
    "우리가 담당한 섬들을 여기서 볼 수 있어!",
    "대신 그만큼 붕을 많이 받을 수 있어!",
    "역시 펭은 똑똑하구나!",
    "그럼 나는 다른 걸 설명해줄게.",
    "우리가 청소할 때 얼마나 움직였는지 기억 나?",
    "얼마나 많이 움직였는지 계산해서…",
    "그, 그치만. 설탕 눈이 너무 폭신해서…",
    "큼큼, 어쨌든. 별을 획득하면 좋은 점을 알려줄게.",
    "여기 보여? 별 게이지!",
    "별을 획득하면 별 게이지가 올라.",
    "그리고 별을 많이 획득할수록.... 더 특별한 보상을 받을 수 있다구!",
    "헤헤, 응. "+PlayerPrefs.GetString("nickname","pingpengboong")+", 설명은 여기까지야.",
    "잘 할 수 있을 거야. 앞으로도 잘 부탁해!"};

        right_script_end_tutorial_1 = new string[]
            {"조금은 긴장하도록 해, 핑.",
    "이제부터는 정말 시작이니까."};

        right_script_end_tutorial_2 = new string[]
            {"으엑…. 너무 많잖아.",
    PlayerPrefs.GetString("nickname","pingpengboong")+". 한 번만 설명할 거야. 잘 들어.",
    "위에서는 우리가 청소할 섬을 선택할 수 있고",
    "아래에서는 스테이지를 선택할 수 있어.",
    "스테이지마다 별을 획득할 수 있지.",
    "적게 움직일 수록 많은 별을 획득할 수 있어.",
    "그러니까, 청소 중에 딴 생각은 안돼. 핑.",
    "펭, 되게 신나보이네."};


        bp_tutorial_stage_3 = new string[]
            {"여긴 아까랑 똑같은 구역이네?",
    "엥? 뭐가? 완전 똑같은데.",
    "괜찮아! 우리가 힘을 합하면…."};

        bp_tutorial_stage_2 = new string[]
            {"펭! 보고 싶었어, 아까는 못 만나는 줄 알았다니깐.",
    "아! 그랬지!",
    "응! "+PlayerPrefs.GetString("nickname","pingpengboong")+", 이번에도 잘 부탁해!",
    "우리는 서로를 장애물처럼 이용할 수 있어!",
    "부딪히면 바로 앞에서 멈추지!",
    "왜애애앵. 나는 펭이 좋은걸!"};

        bp_tutorial_stage_1 = new string[]
            {"첫 번째 튜토리얼이야!",
    "어디보자…. ‘이동 조작’을 알아보는 튜토리얼이래!",
    "아니! 헤헤, 펭이 설명해 줄 거니까!",
    "으엑! 우리 못 만나는 거야?",
    "둘 다 청소해야 하는 거였어?",
    "(뜨끔), "+PlayerPrefs.GetString("nickname","pingpengboong")+"! 나를 움직여 줘! 청소를 시작하자!",
    "손가락으로 화면을 스와이프하면 움직일 수 있어!"};

        bp_tutorial_stage_4 = new string[]
            {"와, 펭! 이번엔 2층이야! 어떻게 올라가지?",
    "날아서 올라갈 수는 없으려나?",
    "흐에엥, 내 동심을 지켜주란 말이야, 펭!",
    "어떻게?",
    "업어주는 거야, 펭?"};

        bp_tutorial_stage_5 = new string[]
            {"펭! 어디있어? 안 보여!",
    "거기서 뭐해? 어서 내려와!",
    "정말! 어떻게 하지?",
    "업고 움직일 수도 있구나!",
    "대단해, 펭!",
    "실수해도 괜찮아, "+PlayerPrefs.GetString("nickname","pingpengboong")+"!",
    "일시정지를 하고 처음부터 다시 시작할 수 있어!"};

        rp_tutorial_stage_3 = new string[]
            {"바보야, 다르잖아.",
    "아까랑 다른 곳에서 시작하잖아.",
    "저기 가운데 튀어나온 곳이 까다롭겠는걸.",
    "맞아. "+PlayerPrefs.GetString("nickname","pingpengboong")+", 잘 부탁해.",
    "우리를 잘 부딪혀서 해결해줘."
        };

        rp_tutorial_stage_2 = new string[]
            {"진정해 핑. 어차피 일 끝나고 같이 가잖아.",
    "이 구역은 간단하네. 빨리 끝나겠어.",
    "너무 들러붙지는 말자는 거지.",
    "이러니까 말이야. 어서 일을 시작하자, "+PlayerPrefs.GetString("nickname","pingpengboong")+"."};

        rp_tutorial_stage_1 = new string[]
            {"무슨 말인지 이해는 했니, 핑.",
    "여기를 봐.",
    "그래. 각자의 구역은 알아서 청소하는 거야.",
    "그래. 너 설마 나 혼자 일 시키려는 거는 아니었겠지?",
    "다른 펭귄을 움직이고 싶으면, 교체 버튼을 눌러줘.",
    "대신 한 번에 한 마리씩 움직일 수 있어."};

        rp_tutorial_stage_4 = new string[]
            {"저기에 경사가 있네. 저기로 미끄러져 올라가자.",
    "우리는 펭귄이야. 그것도 마쉬멜로우 함량 98%.",
    "단언컨데, 날 수 없지.",
    "날 수 없는 대신 우리는 서로를 업을 수 있어.",
    "마쉬멜로우니까.",
    "네가 2층으로 올라갔다가….",
    "내 위로 미끄러져 올라오면 내가 너를 업을게.",
    "…. 빨리 업히기나 해."};

        rp_tutorial_stage_5 = new string[]
            {"여기, 2층에 있어.",
    "바보야. 반대편에 치워야 하는 2층 언덕이 있잖아.",
    "….",
    "나를 업고 가야겠어, 핑.",
    "….",
    "잘 부탁해 "+PlayerPrefs.GetString("nickname","pingpengboong")+".",
    "떨어지면 다시 올라갈 수 없으니까."};

        bp_icecream = new string[]
            {"음, 달콤한 냄새. 베리랑 아이스크림이 가득하네.",
    "여기가 피크닉 명소라던데.",
    "뭐라고 펭?",
    "응, 헤헤.",
    "여기는 마쉬멜로우 빌리지에서 배웠던 걸 활용해서",
    "쉽게 치울 수 있을 것 같아!",
    "응, 얼른 해보자! "+PlayerPrefs.GetString("nickname","pingpengboong")+"!"};

        bp_parfait = new string[]
            {"와! 휴양지다! 파르페 섬에 꼭 와보고 싶었는데!",
    "맞다, 그렇지!",
    "그래도 파르페 섬의 명물, 파르페는 먹어 보고 싶은데!",
    "헉! 그러면 어떻게 해? 파르페 못 먹는 거야?",
    "순서에 맞게?",
    "너무 어렵다. 나는 암기가 제일 싫어!",
    "그렇구나! "+PlayerPrefs.GetString("nickname","pingpengboong")+", 청소를 시작해 볼까!"};

        bp_cracker = new string[]
            {"펭! 지금 그쪽으로 갈게!",
    "뭐, 뭐라고?!",
    "어디가 위험한지는 어떻게 알아?",
    "다행이다.",
    "대신 바닥이 무너지면 더 이상 지나갈 수 없는 거네.",
    "알았지, "+PlayerPrefs.GetString("nickname","pingpengboong")+"! 조심해서 다니자!"};

        rp_icecream = new string[]
            {"하지만 우리는 일 하러 온 거야. 핑.",
    "(달콤해… 좋다….)",
    "일 하자고, 일.",
    "얼른 끝내고 싶어.",
    "아이스크림 때문에 온 몸이 끈적해지기 전에 말이야.",
    "(아이스크림 먹고싶다…)"};

        rp_parfait = new string[]
            {"정신 차려, 핑.",
    "우린 여기 일하러 온 거잖아.",
    "그렇지 않아도 파르페 섬에서 연락이 왔어.",
    "눈보라 때문에 재료들이 죄다 설탕 결정에 갇혀버렸대.",
    "순서에 맞게 제대로 모으면 된다는데.",
    "그 전까지는 장애물같이 단단해서 모을 수 없지만.",
    "컵 ▶ 아이스크림 ▶ 시럽 ▶ 과일 순서야.",
    "걱정하지 마. 위에 순서를 제대로 표시해줄 테니까."};

        rp_cracker = new string[]
            {"조심해! 바닥이 무너질 것 같아.",
    "휴…. 크래커 섬은 워낙 오래된 곳이라",
    "바닥이 무너질 위험이 커. 조심히 다니자.",
    "바닥을 잘 보고 다녀야지.",
    "크래커로 된 바닥 모양을 잘 봐.",
    "빗금 모양에 따라 3번까지 지나갈 수 있어.",
    "금이 많이 간 크래커는 한 번만 지나가도 무너져버려.",
    "무너지더라도 걱정 마, 떨어지진 않으니까."};


        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if(PlayerPrefs.GetInt("tutorial",0) == 0)
            {
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_credit[0]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_credit[0]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_credit[1]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_credit[1]));
                progresses.Add(new Tutorial_Progress(left_image,left_embar,left_text,left_script_credit[2]));
                progresses.Add(new Tutorial_Progress(left_image,left_embar,left_text,left_script_credit[3]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_credit[2]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[4]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[5]));
                progresses.Add(new Tutorial_Progress(right_image,right_shed,right_text,right_script_credit[3]));
                progresses.Add(new Tutorial_Progress(left_image,left_embar,left_text,left_script_credit[6]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[7]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[8]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_credit[4]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[9]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[10]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[11]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_credit[12]));
                progresses.Add(new Tutorial_Progress(right_image,right_shed,right_text,right_script_credit[5]));

                progresses.Add(new Tutorial_Progress(right_image,right_shed,right_text,right_script_gamestart[0]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_gamestart[1]));
                progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_gamestart[0]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_gamestart[2]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_gamestart[3]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_gamestart[1]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_gamestart[2]));
            }
            if(PlayerPrefs.GetInt("tutorial",0) == 1)
            {
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[0]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[1]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[2]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_1[0]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_1[1]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[3]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[4]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[5]));
                progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_1[6]));
            }
            

        }
        if(SceneManager.GetActiveScene().name == "LevelScene" && PlayerPrefs.GetInt("tutorial",0) == 1)
        {
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[0]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[1]));
                progresses.Add(new Tutorial_Progress(right_image,right_shed,right_text,right_script_end_tutorial_2[0]));
            progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_end_tutorial_2[2]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[1]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[2]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[3]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[3]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[4]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[5]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[6]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[4]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[5]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[6]));
            progresses.Add(new Tutorial_Progress(left_image,left_embar,left_text,left_script_end_tutorial_2[7]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[8]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[9]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[10]));
            progresses.Add(new Tutorial_Progress(left_image,left_smile,left_text,left_script_end_tutorial_2[11]));
                progresses.Add(new Tutorial_Progress(right_image,right_idle,right_text,right_script_end_tutorial_2[7]));
            progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_end_tutorial_2[12]));
            progresses.Add(new Tutorial_Progress(left_image,left_cute,left_text,left_script_end_tutorial_2[13]));
        }
        if(SceneManager.GetActiveScene().name == "Tutorial_Island")
        {
            if(GameManager.instance.nowLevel == 0)
            {
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[0]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[1]));

                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[0]));

                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[2]));

                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[1]));

                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[3]));

                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[2]));

                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[4]));

                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[3]));

                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[5]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_1[6]));

                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[4]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_1[5]));
            }
            if(GameManager.instance.nowLevel == 1)
            {
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_2[0]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_2[1]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[2]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[3]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[4]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_2[2]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_2[5]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_2[3]));
            }
            if(GameManager.instance.nowLevel == 2)
            {
                    progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_3[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_3[0]));
                    progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_3[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_3[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_3[2]));
                    progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_3[2]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_3[3]));
                progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_3[4]));
            }
            if(GameManager.instance.nowLevel == 3)
            {
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_4[0]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[0]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_4[1]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[1]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[2]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_4[2]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[3]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[4]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_4[3]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[5]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[6]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_4[4]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_4[7]));
            }
            if(GameManager.instance.nowLevel == 4)
            {
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[0]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[0]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[1]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[1]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[2]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[2]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[3]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[3]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[4]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[4]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[5]));
                    progresses.Add(new TutorialStageProgress(rpText,rp_tutorial_stage_5[6]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[5]));
                progresses.Add(new TutorialStageProgress(bpText,bp_tutorial_stage_5[6]));
            }
        }
        if(SceneManager.GetActiveScene().name == "Icecream_Island")
        {
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[0]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[1]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[2]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[2]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[3]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[4]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[5]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[3]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[4]));
            progresses.Add(new TutorialStageProgress(bpText,bp_icecream[6]));
                progresses.Add(new TutorialStageProgress(rpText,rp_icecream[5]));          
        }
        if(SceneManager.GetActiveScene().name == "Beach_Island")
        {
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[1]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[1]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[2]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[2]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[3]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[3]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[4]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[5]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[4]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[6]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[5]));
                progresses.Add(new TutorialStageProgress(rpText,rp_parfait[7]));
            progresses.Add(new TutorialStageProgress(bpText,bp_parfait[6]));
        }
        if(SceneManager.GetActiveScene().name == "Cracker_Island")
        {
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[0]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[0]));
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[1]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[2]));
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[2]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[3]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[4]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[5]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[6]));
                progresses.Add(new TutorialStageProgress(rpText,rp_cracker[7]));
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[3]));
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[4]));
            progresses.Add(new TutorialStageProgress(bpText,bp_cracker[5]));
        }
        if(SceneManager.GetActiveScene().name == "Cottoncandy_Island")
        {
            //아직 없음.
        }

        

        
    }

    void Start()
    {
        xmlManager = XMLManager.ins;

        if(SceneManager.GetActiveScene().name == "MainScene")
        {
            if(PlayerPrefs.GetInt("tutorial",0) == 0)
            {
                StartCoroutine(Tutorial_All());
            }
            if(PlayerPrefs.GetInt("tutorial",0) == 1)
            {
                StartCoroutine(TutorialEndPhase_1());//섬 클릭해서 들어가기
            }
            
        }
        else if(SceneManager.GetActiveScene().name == "LevelScene")
        {
            if(PlayerPrefs.GetInt("tutorial",0) == 1)
                StartCoroutine(TutorialEndPhase_2());//끝나면 2
        }
        else//인게임
        {
                StartCoroutine(StageTutorial());
        }
    }


    // Update is called once per frame
    int islandNum()
    {
        int stage = AWSManager.instance.userInfo.stage_current;

        for (int i = 0; i < IslandData.island_last.Length; i++)
        {
            if (stage <= IslandData.island_last[i])
            {
                Debug.Log("island num : " + IslandData.island_last[i]);
                return i;
            }
        }

        return -1;
    }

    public void NextProgressClicked()
    {
        trigger = true;
    }

    void MoveUI(Transform target, Transform to)
    {
        target.SetParent(to);
    }

    IEnumerator TutorialEndPhase_1()
    {
        int progress_count = progresses.Count;
        int now_count = 0;

        while(now_count != progress_count)
        {
            if(now_count == 8)
            {
                MoveUI(islandButton[islandNum()],tutorialCanvas);
                islandButton[islandNum()].gameObject.GetComponent<Button>().interactable = false;
            }
                

            yield return StartCoroutine(Texting(progresses[now_count]));
            yield return StartCoroutine(Waiting());
            progresses[now_count].target_text.transform.parent.gameObject.SetActive(false);
            now_count++;
        }
        islandButton[islandNum()].gameObject.GetComponent<Button>().interactable = true;
        left_image.gameObject.SetActive(false);
        right_image.gameObject.SetActive(false);
    }
    IEnumerator TutorialEndPhase_2()
    {
        int progress_count = progresses.Count;
        int now_count = 0;

        while(now_count != progress_count)
        {
            if(now_count == 5)//섬
            {
                MoveUI(level_island[islandNum()],tutorialCanvas);
            }        
            if(now_count == 6)//스테이지
            {
                left_image.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(-300,550,0);
                right_image.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(300,550,0);
                MoveUI(level_island[islandNum()],levelPanel[islandNum()]);
                MoveUI(level_level_list[islandNum()],tutorialCanvas);

            }
            if(now_count == 7)//스테이지
            {
                left_image.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(-300,-550,0);
                right_image.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(300,-550,0);
                MoveUI(level_level_list[islandNum()],levelPanel[islandNum()]);
            }
            if(now_count == 16)//
            {
                MoveUI(level_star_gage[islandNum()],tutorialCanvas);
            }
            if(now_count == 18)//
            {
                MoveUI(level_star_gage[islandNum()],levelPanel[islandNum()]);
            }
                
            
            
            yield return StartCoroutine(Texting(progresses[now_count]));
            yield return StartCoroutine(Waiting());
            progresses[now_count].target_text.transform.parent.gameObject.SetActive(false);
            now_count++;
        }
        PlayerPrefs.SetInt("tutorial",IslandData.tutorial+1);
        gameObject.SetActive(false);
    }

    IEnumerator StageTutorial()
    {
        int progress_count = progresses.Count;
        int now_count = 0;

        while(now_count != progress_count)
        {
            if(progresses[now_count].target_text == rpText)
            {
                Debug.Log("rp said : ");
                uiController.ChangeCharacter(2);
            }
            else
            {
                Debug.Log("bp said : ");
                uiController.ChangeCharacter(1);
            }
            yield return StartCoroutine(TextingStage(progresses[now_count]));
            yield return StartCoroutine(Waiting());
            progresses[now_count].target_text.transform.parent.gameObject.SetActive(false);
            now_count++;
        }

        this.gameObject.SetActive(false);
        yield break;
            
    }

    IEnumerator Tutorial_All()
    {
        int progress_count = progresses.Count;
        int now_count = 0;

        while(now_count != progress_count)
        {
            if(now_count == 15)
                MoveUI(creditPanel,tutorialCanvas);
            if(now_count == 18)
                MoveUI(creditPanel,baseCanvas);
            if(now_count == 20)
            {
                startButton.GetComponent<Button>().interactable = false;
                MoveUI(startButton,tutorialCanvas);
            }
                
            
            
            yield return StartCoroutine(Texting(progresses[now_count]));
            yield return StartCoroutine(Waiting());
            progresses[now_count].target_text.transform.parent.gameObject.SetActive(false);
            now_count++;
        }

        startButton.GetComponent<Button>().interactable = true;
        //무ㅜ조건 튜토리얼로 가게해야함.
        left_image.gameObject.SetActive(false);
        right_image.gameObject.SetActive(false);

        yield break;
        
    }


    IEnumerator Waiting()
    {
        float wait = 0.2f;

        while(!trigger)
        {

            yield return new WaitForSeconds(wait);
        }

        yield break;

    }

    IEnumerator Texting(Progress progress)
    {
        trigger = false;
        progress.target_text.text = "";
        progress.target_text.transform.parent.gameObject.SetActive(true);
        progress.target_image.sprite = progress.emotion;
        //target_image.gameObject.SetActive(true);
        float wait = 0.1f;
        int count = 0;
        while(count != progress.content.Length)
        {
            if(trigger)
            {
                progress.target_text.text = "";
                for(int i = 0 ; i < progress.content.Length ; i++)
                {
                    progress.target_text.text += progress.content[i];
                }
                trigger = false;
                break;
            }
            progress.target_text.text += progress.content[count];
            count++;
            yield return new WaitForSeconds(wait);
        }
        



        yield break;

    }

    IEnumerator TextingStage(Progress progress)
    {
        trigger = false;
        progress.target_text.text = "";
        progress.target_text.transform.parent.gameObject.SetActive(true);
        //progress.target_text.transform.parent.transform.position = inGameCamera.WorldToScreenPoint(bpPlayer.position + new Vector3(0, 0.8f, 0));
        float wait = 0.1f;
        int count = 0;
        while(count != progress.content.Length)
        {
            if(trigger)
            {
                progress.target_text.text = "";
                for(int i = 0 ; i < progress.content.Length ; i++)
                {
                    progress.target_text.text += progress.content[i];
                }
                trigger = false;
                break;
            }
            progress.target_text.text += progress.content[count];
            count++;
            yield return new WaitForSeconds(wait);
        }
        



        yield break;

    }

}
