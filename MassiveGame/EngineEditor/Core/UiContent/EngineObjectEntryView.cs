using MassiveGame.Settings;
using System.Collections.ObjectModel;
using WpfControlLibrary1.Models;

namespace MassiveGame.EngineEditor.Core.UiContent
{
    public class EngineObjectEntryView
    {
        public ObservableCollection<EngineObjectEntry> EngineObjectEntries { set; get; }

        public void TestCreateEntries()
        {
            var entries = new ObservableCollection<EngineObjectEntry>
            {
                new EngineObjectEntry { EntryLabel = "Entry 1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 2", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 3", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 4", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 5", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 6", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 7", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 8", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 9", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 10",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 11",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 12",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 13",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 14",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 15",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" }
            };
            EngineObjectEntries = entries;
        }

    }
}
