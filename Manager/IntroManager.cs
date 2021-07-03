using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class IntroManager : MonoBehaviour
{
    public float fader_speed;
    
    public Image[] intro_object;

    public Image fade;
   
    int now_intro_num = 0;
    bool isFadeEnd = false;

    void Awake()
    {
        StartCoroutine(SwitchIntroScene(first : true));
    }

    public void NextButton()
    {
        
        if(isFadeEnd)
        {
            if(now_intro_num == intro_object.Length-1)
            {
                StartCoroutine(Fader());
                isFadeEnd = false;
            }                
            else
                StartCoroutine(SwitchIntroScene(first : false));
        }
        
    }

    IEnumerator SwitchIntroScene(bool first)
    {
        isFadeEnd = false;
        if(!first)
        {
            yield return StartCoroutine(FadeOut());
            now_intro_num++;
        }        
        yield return StartCoroutine(FadeIn());
        isFadeEnd = true;

        yield break;
    }
    IEnumerator FadeIn()
    {
        float t = 0;
        Color color = intro_object[now_intro_num].color;
        while(t < 1)//fade in
        {
            t += Time.deltaTime * fader_speed;
            color.a = t;
            intro_object[now_intro_num].color = color;
            yield return null;
        }
        yield break;
    }
    IEnumerator FadeOut()
    {
        float t = 1;
        Color color = intro_object[now_intro_num].color;

        while(t > 0)//fade out
        {
            t -= Time.deltaTime * fader_speed;
            color.a = t;
            intro_object[now_intro_num].color = color;
            yield return null;
        }
        yield break;
    }

    IEnumerator Fader()
    {
        
        float t = 0;
        Color c = fade.color;

        while(t < 1)
        {
            t += Time.deltaTime * 0.7f;
            c.a = t;
            fade.color = c;
            yield return null;
        }
        
        SceneManager.LoadScene("MainScene");

        

        yield break;
        
    }

}
