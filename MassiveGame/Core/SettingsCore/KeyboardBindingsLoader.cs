using MassiveGame.Core.DebugCore;
using MassiveGame.Core.GameCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MassiveGame.Core.SettingsCore
{
    public class KeyboardBindingsLoader
    {
        private string KeyboardBindingsContent = null;

        public KeyboardBindingsLoader()
        {
            string pathToIni = ProjectFolders.IniPath + "keybindings.ini";

            try
            {
                using (StreamReader reader = new StreamReader(pathToIni))
                    KeyboardBindingsContent = reader.ReadToEnd();
            }
            catch (FileLoadException ex)
            {
                Log.AddToFileStreamLog(ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #region core

        private List<KeyValuePair<Keys, ActionTypeBinding>> GetBindings()
        {
            string[] bindingCodeLines = KeyboardBindingsContent.Split(';');
            Dictionary<Keys, ActionTypeBinding> resultBindings = new Dictionary<Keys, ActionTypeBinding>();

            try
            {
                for (Int32 i = 0; i < bindingCodeLines.Length; i++)
                {
                    if (bindingCodeLines[i] != string.Empty)
                    {
                        string[] keyValue = bindingCodeLines[i].Split(':');
                        ActionTypeBinding actionType = (ActionTypeBinding)Enum.Parse(typeof(ActionTypeBinding), keyValue[0]);
                        Keys key = (Keys)char.ToUpper(keyValue[1].ToCharArray()[1]);
                        resultBindings.Add(key, actionType);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                // wrong argument in keybinding.ini
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultBindings.ToList();
        }


        #endregion

#if DEBUG
        public void SetKeyboardBindings()
        {
            // player controller bindings
            var playerController = GameWorld.GetWorldInstance().GetLevel().PlayerController;
            GetBindings().ForEach((key_value_pair) => playerController.SetBindingKeyboardKey(key_value_pair.Key, key_value_pair.Value));
        }
#endif

    }
}
