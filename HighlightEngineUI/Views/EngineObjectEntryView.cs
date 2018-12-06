using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HighlightEngineUI.Models;

namespace HighlightEngineUI.Views
{
    public class EngineObjectEntryView
    {
        public ObservableCollection<EngineObjectEntry> EngineObjectEntries { set; get; }

        public void TestCreateEntries()
        {
            var entries = new ObservableCollection<EngineObjectEntry>
            {
                new EngineObjectEntry { EntryLabel = "Entry 1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon.png" },
                new EngineObjectEntry { EntryLabel = "Entry 2", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon.png" },
                new EngineObjectEntry { EntryLabel = "Entry 3", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon.png" }
            };
            EngineObjectEntries = entries;
        }

    }
}
