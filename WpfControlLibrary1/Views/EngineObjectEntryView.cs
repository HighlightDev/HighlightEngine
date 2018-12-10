using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfControlLibrary1.Models;

namespace WpfControlLibrary1.Views
{
    public class EngineObjectEntryView
    {
        public ObservableCollection<EngineObjectEntry> EngineObjectEntries { set; get; }

        public void TestCreateEntries()
        {
            var entries = new ObservableCollection<EngineObjectEntry>
            {
                new EngineObjectEntry { EntryLabel = "Entry 1", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 2", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 3", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 4", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 5", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 6", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 7", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 8", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 9", AlbedoTextureLabel = "Albedo texture",ModelLabel = "Model1",IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 10", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 11", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 12", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 13", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 14", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" },
                new EngineObjectEntry { EntryLabel = "Entry 15", AlbedoTextureLabel = "Albedo texture", ModelLabel = "Model1", IconURI = "C:\\Users\\dzinovev\\Desktop\\Универчик\\project\\HighlightEngine\\HighlightEngineUI\\Images\\icon2.png" }
            };
            EngineObjectEntries = entries;
        }

    }
}
