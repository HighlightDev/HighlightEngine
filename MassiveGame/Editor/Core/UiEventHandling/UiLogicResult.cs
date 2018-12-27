using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Editor.Core.UiEventHandling
{
    public class UiLogicResult
    {
        public ActionType LogicActionType { set; get; }

        // Indicates that some result should be returned back to ui element
        public bool bHasCallbackResult { set; get; } = false;

        public string CallbackResult { set; get; } = string.Empty;
    }
}
