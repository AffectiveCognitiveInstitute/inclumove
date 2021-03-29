using Aci.Unity.Events;
using Aci.Unity.UserInterface.ViewControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatHighlightViewController : MonoBehaviour, IAciEventHandler<BotMessageReceivedEvent>
{
    private IAciEventManager m_EventManager;

    [SerializeField]
    private RectTransform m_ContentContainer;

    [Zenject.Inject]
    private void Construct(IAciEventManager eventManager)
    {
        m_EventManager = eventManager;
    }

    private void OnEnable()
    {
        RegisterForEvents();
    }

    private void OnDisable()
    {
        UnregisterFromEvents();
    }

    void IAciEventHandler<BotMessageReceivedEvent>.OnEvent(BotMessageReceivedEvent arg)
    {
        for(int i = 0; i <  m_ContentContainer.childCount; ++i)
        {
            ChatCardViewController cardController = m_ContentContainer.GetChild(i).GetComponent<ChatCardViewController>();
            if (cardController == null)
                continue;
            cardController.SetHighlighted(i == m_ContentContainer.childCount - 1);
        }
    }

    public void RegisterForEvents()
    {
        m_EventManager.AddHandler(this);
    }

    public void UnregisterFromEvents()
    {
        m_EventManager.RemoveHandler(this);
    }
}
