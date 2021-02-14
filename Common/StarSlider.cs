using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StarSlider : MonoBehaviour
{
    public Slider slider;
    public Transform[] star_limits;

    float sldierWidth = 0f;
    List<int> star_nums = new List<int>();
    public List<TextMeshProUGUI> starText;

    float maxValue = 0f;
    // Start is called before the first frame update
    public Sprite star_fail;
    
    public void SetSliderValue(int value)
    {
        if(value == star_nums[0])
        {
            star_limits[0].GetComponent<Image>().sprite = star_fail;
        }
        else if(value == star_nums[1])
        {
            star_limits[1].GetComponent<Image>().sprite = star_fail;
        }
        else if(value == star_nums[2])
        {
            star_limits[2].GetComponent<Image>().sprite = star_fail;
        }
        slider.value = maxValue - value;
    }
    public void SetSlider(List<int> star_limit, int maxValue)
    {
        RectTransform rt = slider.GetComponent<RectTransform>();
        sldierWidth = rt.rect.width;
        this.maxValue = maxValue;
        slider.maxValue = maxValue;

        for(int i = 0 ; i < star_limit.Count ; i++)
        {
            star_nums.Add(star_limit[i]);
            starText[i].text = star_limit[i].ToString();
        }
        
        for(int i = 0 ; i < star_nums.Count ; i++)
        {
            float x = (float)star_nums[i]/maxValue*sldierWidth*(-1);
            Debug.Log(star_nums[i]+","+x);
            star_limits[i].localPosition = new Vector3(x+350,star_limits[i].localPosition.y,star_limits[i].localPosition.z);
        }
    }
    
}
