using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeepSeekTalk : MonoBehaviour
{
    public InputField inputField;
    public Text responseText;

    public void SendText()
    {
        StartCoroutine(ConnetToDeepSeek());

        inputField.text = "";
        responseText.text = "DeepSeek 正在深度思考中...";
    }

    public void CloseDeepSeek()
    {
        //退出游戏
        Application.Quit();
    }

    public IEnumerator ConnetToDeepSeek()
    {
        using(UnityEngine.Networking.UnityWebRequest request = 
            new UnityEngine.Networking.UnityWebRequest("https://api.deepseek.com/chat/completions", "POST"))
        {
            //设置请求头
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + DeepSeekConfig.APIKey);

            //设置请求体
            var requestData = new RequestData
            {
                model = "deepseek-chat",
                messages = new List<Message>
                {
                    new Message{role = "user", content = inputField.text}
                }
            };
            //将请求体转换为字节数组
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(requestData));
            //设置上传处理程序
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            //设置下载处理程序
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            //发送请求
            yield return request.SendWebRequest();

            try
            {
                //检查请求是否成功
                if(request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    //请求失败
                    Debug.LogError("连接失败: " + request.error);
                }
                else
                {
                    //解析JSON
                    var response = JsonUtility.FromJson<DeepSeekResponse>(request.downloadHandler.text);
                    //获取响应文本
                    responseText.text = response.choices[0].message.content;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("处理请求时出错: " + e.Message);
            }
        }

        
    }

}
