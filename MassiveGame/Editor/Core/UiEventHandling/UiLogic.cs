using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MassiveGame.Editor.Core.UiEventHandling
{
    

    public class UiLogic
    {
        public UiLogic()
        {

        }

        public UiLogicResult DoLogic(ActionParameters args)
        {
            UiLogicResult result = null;

            if (args.Type == ActionType.Load)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                var dlgResult = openFileDialog.ShowDialog();
                if (dlgResult == DialogResult.OK || dlgResult == DialogResult.Yes)
                {
                    if (openFileDialog.CheckPathExists)
                    {
                        string loadFilePath = openFileDialog.FileName;
                        result = new UiLogicResult() { bHasCallbackResult = true, CallbackResult = loadFilePath, LogicActionType = args.Type };
                    }
                }
            }

            return result;
        }

    }
}
