using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SettingPopup : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public AudioMixer sfxMixer;
    public AudioMixer bgmMixer;


    private void OnEnable()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGM", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1);
    }

    public void SetBGMSound(float value)
    {
        Debug.Log(value);
        PlayerPrefs.SetFloat("BGM", value);
        float bgmValue = Mathf.Log10(value) * 20;
        bgmMixer.SetFloat("bgmValue", bgmValue);
    }
    public void SetSFXSound(float value)
    {
        Debug.Log(sfxSlider.value);
        PlayerPrefs.SetFloat("SFX", value);
        float sfxValue = Mathf.Log10(value) * 20;
        sfxMixer.SetFloat("sfxValue", sfxValue);
    }
    public void PushOn()
    {

    }
    public void PushOff()
    {

    }
    public void ReViewIntro()
    {

    }
    public void ViewMaker()
    {

    }
    public void FacebookLoginForGuest()
    {

    }
    public void SignOutForFacebookAuth()
    {

    }
    public void DeleteAccount()
    {

    }
    public void Exit()
    {
        gameObject.SetActive(false);
        if(GameController.instance != null)
        {
            GameController.instance.SetPlaying(true);
        }
    }
}
