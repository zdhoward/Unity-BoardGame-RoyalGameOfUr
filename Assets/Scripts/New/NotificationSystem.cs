using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance;

    [Header("Requirements")]
    [SerializeField] TextMeshProUGUI notificationPrefab;
    [SerializeField] Transform notificationSpawnPosition;

    [Header("Settings")]
    [SerializeField] float delayBetweenNotifications = 0.75f;
    [SerializeField] float amountToMoveY = 40f;
    [SerializeField] float timeToMoveY = 2f;

    Queue<string> notificationQueue = new Queue<string>();

    float timer = 0f;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Instance of NotificationSystem in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (timer <= 0)
        {
            if (DisplayNextNotification())
            {
                timer = delayBetweenNotifications;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void QueueNotification(string message)
    {
        notificationQueue.Enqueue(message);
    }

    bool DisplayNextNotification()
    {
        if (notificationQueue.Count > 0)
        {
            string message = notificationQueue.Dequeue();
            SpawnNotification(message);
            return true;
        }
        return false;
    }

    void SpawnNotification(string labelText)
    {
        Debug.Log(labelText);

        TextMeshProUGUI notificationLabel = Instantiate(notificationPrefab, notificationSpawnPosition);
        notificationLabel.text = labelText;

        RectTransform notificationLabelRectTransform = notificationLabel.GetComponent<RectTransform>();

        notificationLabelRectTransform.LeanMoveLocalY(notificationLabelRectTransform.anchoredPosition.y + amountToMoveY, timeToMoveY);

        Color alphaStart = notificationLabel.color;
        Color alphaEnd = notificationLabel.color;
        alphaEnd.a = 0f;
        LeanTween.value(notificationLabel.gameObject, a => notificationLabel.color = a, alphaStart, alphaEnd, timeToMoveY);

        Destroy(notificationLabel.gameObject, timeToMoveY);
    }
}
