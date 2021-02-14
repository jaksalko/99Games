using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class JsonAdapter : MonoBehaviour
{
    
   
    public IEnumerator API_GET(string url , Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2+url);
//        Debug.Log(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            callback(null);
        }
        else
        {
           

            if(www.responseCode != 200)
            {
                Debug.Log("response code : " + www.responseCode);
                callback(null);
               
            }
            else
            {
                Debug.Log("GET WebRequset : " + www.downloadHandler.text);
                callback(www.downloadHandler.text);
               
            }
            //GET.Invoke(JsonHelper.fixJson(www.downloadHandler.text));


            // Or retrieve results as binary data
            /*byte[] results = www.downloadHandler.data;

           


            string fixdata = JsonHelper.fixJson(www.downloadHandler.text);
            JsonData[] datas = JsonHelper.FromJson<JsonData>(fixdata);
           
            Debug.Log(datas.Length);*/
            yield break;

        }
    }
    
    public IEnumerator API_POST(string url , string bodyJsonString , Action<string> callback)
    {
        Debug.Log(bodyJsonString);
        var req = new UnityWebRequest(PrivateData.ec2 + url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if(req.isHttpError || req.isNetworkError )
        {
            Debug.Log(req.error);
            callback(null);
        }
        else if(req.responseCode != 200)
        {
            Debug.Log("response code : " + req.responseCode);
            callback(null);
            
        }
        else
        {
            Debug.Log("GET WebRequset : " + req.downloadHandler.text);
            callback(req.downloadHandler.text);
        }
       
        Debug.Log("Status Code: " + req.responseCode);

        yield break;
    }

}





public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
//        Debug.Log(wrapper.Items.Length);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    public static string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }


}