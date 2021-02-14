using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StagePopup : UIScript
{
    public Image starImage;
    public Image stageImage;

    public Text titleText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI snowText;
    public TextMeshProUGUI moveText;

    public Image fader;
    public GameObject heartAnimation;
    public float fadeOutTime;
    
    int stage_num;

    public void SetPopup(string stage_string, int stage_num, Sprite star_sprite)
    {
        this.stage_num = stage_num;
        stageText.text = stage_string;
        starImage.sprite = star_sprite;

        //stage_num으로 csv에서 정보를 획득하기
        Map map = CSVManager.instance.GetMap(stage_num);
        Debug.Log(map.mapsizeW);
        titleText.text = map.map_title;
        moveText.text = map.star_limit[2].ToString();
        snowText.text = map.total_snow.ToString();

    }

    public void ExitButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void PlayButtonClicked()
    {
        gameManager.nowLevel = stage_num;
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        float t = 0;
        Color c = fader.color;

        heartAnimation.SetActive(true);
        fader.gameObject.SetActive(true);

        while (t <= 1)
        {
            t += Time.deltaTime / fadeOutTime;
            c.a = t;
            fader.color = c;

            yield return null;
        }

        Load_Island(gameManager.nowLevel);
        yield break;
    }
}
