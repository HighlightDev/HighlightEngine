using MassiveGame.Settings;
using System;
using System.Collections.ObjectModel;
using WpfControlLibrary1.Models;

namespace MassiveGame.EngineEditor.Core.UiContent
{
    public class EngineObjectEntryView
    {
        public ObservableCollection<EngineObjectEntry> EngineObjectEntries { set; get; }

        public void TestCreateEntries()
        {
            string currentDir = Environment.CurrentDirectory;
            Int32 index = currentDir.IndexOf("HighlightEngine");
            string newPath = currentDir.Substring(0, index);
            var entries = new ObservableCollection<EngineObjectEntry>
            {
                 new EngineObjectEntry {
                    ModelVisible = System.Windows.Visibility.Visible,
                    AlbedoVisible = System.Windows.Visibility.Visible,
                    NormalMapVisible = System.Windows.Visibility.Visible,
                    SpecularMapVisible = System.Windows.Visibility.Collapsed,
                    EntryLabel = "Static Mesh", NormalMapLabel = "Normal Map",
                    SpecularMapLabel = "Specular Map", AlbedoTextureLabel = "Albedo texture",
                    ModelLabel = "Model",
                    IconURI = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
                , new EngineObjectEntry {
                     ModelVisible = System.Windows.Visibility.Visible,
                     AlbedoVisible = System.Windows.Visibility.Collapsed,
                     NormalMapVisible = System.Windows.Visibility.Visible,
                     SpecularMapVisible = System.Windows.Visibility.Visible,
                     EntryLabel = "Dynamic Mesh", NormalMapLabel = "Normal Map",
                     SpecularMapLabel = "Specular Map", AlbedoTextureLabel = "Albedo texture",
                     ModelLabel = "Model2",
                     IconURI = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
                , new EngineObjectEntry {
                     ModelVisible = System.Windows.Visibility.Visible,
                     AlbedoVisible = System.Windows.Visibility.Visible,
                     NormalMapVisible = System.Windows.Visibility.Collapsed,
                     SpecularMapVisible = System.Windows.Visibility.Visible,
                     EntryLabel = "Skeletal Mesh", NormalMapLabel = "Normal Map",
                     SpecularMapLabel = "Specular Map", AlbedoTextureLabel = "Albedo texture",
                     ModelLabel = "Model3",
                     IconURI = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
                , new EngineObjectEntry {
                     ModelVisible = System.Windows.Visibility.Visible,
                     AlbedoVisible = System.Windows.Visibility.Collapsed,
                     NormalMapVisible = System.Windows.Visibility.Collapsed,
                     SpecularMapVisible = System.Windows.Visibility.Visible,
                     EntryLabel = "Skybox", NormalMapLabel = "Normal Map",
                     SpecularMapLabel = "Specular Map", AlbedoTextureLabel = "Albedo texture",
                     ModelLabel = "Model4",
                     IconURI = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
                , new EngineObjectEntry {
                     ModelVisible = System.Windows.Visibility.Visible,
                     AlbedoVisible = System.Windows.Visibility.Collapsed,
                     NormalMapVisible = System.Windows.Visibility.Collapsed,
                     SpecularMapVisible = System.Windows.Visibility.Collapsed,
                     EntryLabel = "Light", NormalMapLabel = "Normal Map",
                     SpecularMapLabel = "Specular Map", AlbedoTextureLabel = "Albedo texture",
                     ModelLabel = "Model5",
                     IconURI = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
            };
            EngineObjectEntries = entries;
        }

    }
}
