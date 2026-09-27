using UnityEngine;

[System.Serializable]
public class DeepSeekResponse 
{
    public string id;
    public string object_name;
    public long created;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
    public int index;
}