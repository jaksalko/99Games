using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EditorSettingPopup : MonoBehaviour
{
    public InputField width;//x
    public InputField height;//z

    public Text warningText;
    Coroutine coroutine;

    

    public void EnterEditorMode()
    {
        int w = int.Parse(width.text);
        int h = int.Parse(height.text);
        

        if(w < 3 || w > 13 || h < 3 || h > 13)
        {
            StopAllCoroutines();
            StartCoroutine(Warning());
            return;
        }
        GameManager.instance.maxSize.x = w;
        GameManager.instance.maxSize.y = h;
        SceneManager.LoadScene("MapEditor");
    }
    public void Exit()
    {
        width.text = "";
        height.text = "";
        gameObject.SetActive(false);
    }

    IEnumerator Warning()
    {
        float alpha = 1;
        warningText.gameObject.SetActive(true);
        Color warningColor = warningText.color;

        while(alpha >= 0)
        {
            alpha -= Time.deltaTime;
            warningColor.a = alpha;
            warningText.color = warningColor;

            yield return new WaitForSeconds(Time.deltaTime);
        }
        warningText.gameObject.SetActive(false);
        warningColor.a = 1;
        warningText.color = warningColor;
        yield break;
    }
}
