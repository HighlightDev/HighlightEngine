using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfControlLibrary1.EventHandlerCore;

namespace MassiveGame.Editor.Core.UiEventHandling
{
    public class UiEventHandleFactory
    {
        UiLogic m_uiLogic = null;

        public UiEventHandleFactory()
        {
            m_uiLogic = new UiLogic();
        }

        // this method should be called within main thread (UI creator)
        public void DoProcessEvent(EventData eventData)
        {
            string actionType = eventData.AdditionalInfo;
            if (actionType != null)
            {
                ActionType resultActionType;
                switch (actionType)
                {
                    case "LoadMesh":
                    case "LoadAlbedo":
                    case "LoadNormalMap":
                    case "LoadSpecularMap":
                    default:
                        resultActionType = ActionType.Load; break;
                }

                UiLogicResult logicResult = m_uiLogic.DoLogic(new EntityActionParameters(resultActionType));
                if (logicResult != null)
                {
                    SendBackToUiElement(logicResult, eventData);
                }
            }
        }

        public void SendBackToUiElement(UiLogicResult logicResult, EventData eventData)
        {
            switch (eventData.SenderInputType)
            {
                case InputUiType.TextBlock:
                    {
                        TextBlock sender = eventData.Sender as TextBlock;
                        string fullPath = logicResult.CallbackResult;
                        char[] charArray = fullPath.ToCharArray();
                        Array.Reverse(charArray);
                        string reversed = new string(charArray);
                        Int32 index = reversed.IndexOf("\\");
                        string fileName = fullPath.Substring(logicResult.CallbackResult.Length - index);
                        sender.Text = fileName;
                        break;
                    }
            }
        }
    }
}
