using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {

    #region Singleton
    public static MessageUI instance = null;
    #endregion

    private struct Message
    {
        public string text;
        public Color color;

        public Message(string t, Color c)
        {
            text = t;
            color = c;
        }
    }

    private Queue<Message> messages;
    private Text[] messageLines;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There should only be one instance of MessageUI in the scene!");
            return;
        }

        instance = this;

        messages = new Queue<Message>();
        messageLines = GetComponentsInChildren<Text>();

        for (int i = 0; i < messageLines.Length; i++)
        {
            messages.Enqueue(new Message("", Color.white));
        }
    }

    public void Log(string message, Color color)
    {
        messages.Enqueue(new Message(message, color));

        if (messages.Count > messageLines.Length)
            messages.Dequeue();

        UpdateUI(); // not updating right away always
    }

    private void UpdateUI()
    {
        Message[] messageArr = messages.ToArray();
        for (int i = 0; i < messageLines.Length; i++)
        {
            messageLines[messageArr.Length - i - 1].text = messageArr[i].text;
            Color c = messageArr[i].color;
            c.a = Mathf.Clamp((float)i / (float)(messageLines.Length), 0.1f, 1f);
            messageLines[messageArr.Length - i - 1].color = c;
        }
    }
}
