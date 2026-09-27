using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class RequestData 
{
    public string model;

    public List<Message> messages;
}


[System.Serializable]
public class Message
{
    public string role;
    public string content;
}