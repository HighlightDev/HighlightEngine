using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfControlLibrary1.Models
{
    public class EngineObjectEntryModel { }

    public class EngineObjectEntry : INotifyPropertyChanged
    {
        private string m_entryLabel;
        private string m_modelLabel;
        private string m_albedoTextureLabel;
        private string m_normalMapLabel;
        private string m_specularMapLabel;
        private string m_iconURI;

        private Visibility m_modelVisible;
        private Visibility m_albedoVisible;
        private Visibility m_normalMapVisible;
        private Visibility m_specularMapVisible;

        public Visibility ModelVisible
        {
            get { return m_modelVisible; }
            set
            {
                if (value != m_modelVisible)
                {
                    m_modelVisible = value;
                    RaisePropertyChanged("ModelVisible");
                }
            }
        }

        public Visibility AlbedoVisible
        {
            get { return m_albedoVisible; }
            set
            {
                if (value != m_albedoVisible)
                {
                    m_albedoVisible = value;
                    RaisePropertyChanged("AlbedoVisible");
                }
            }
        }

        public Visibility NormalMapVisible
        {
            get { return m_normalMapVisible; }
            set
            {
                if (value != m_normalMapVisible)
                {
                    m_normalMapVisible = value;
                    RaisePropertyChanged("NormalMapVisible");
                }
            }
        }

        public Visibility SpecularMapVisible
        {
            get { return m_specularMapVisible; }
            set
            {
                if (value != m_specularMapVisible)
                {
                    m_specularMapVisible = value;
                    RaisePropertyChanged("SpecularMapVisible");
                }
            }
        }

        public string EntryLabel
        {
            get { return m_entryLabel; }
            set
            {
                if (value != m_entryLabel)
                {
                    m_entryLabel = value;
                    RaisePropertyChanged("EntryLabel");
                }
            }
        }

        public string ModelLabel
        {
            get { return m_modelLabel; }
            set
            {
                if (value != m_modelLabel)
                {
                    m_modelLabel = value;
                    RaisePropertyChanged("ModelLabel");
                }
            }
        }

        public string AlbedoTextureLabel
        {
            get { return m_albedoTextureLabel; }
            set
            {
                if (value != m_albedoTextureLabel)
                {
                    m_albedoTextureLabel = value;
                    RaisePropertyChanged("AlbedoTextureLabel");
                }
            }
        }

        public string NormalMapLabel
        {
            get { return m_normalMapLabel; }
            set
            {
                if (value != m_normalMapLabel)
                {
                    m_normalMapLabel = value;
                    RaisePropertyChanged("NormalMapLabel");
                }
            }
        }

        public string SpecularMapLabel
        {
            get { return m_specularMapLabel; }
            set
            {
                if (value != m_specularMapLabel)
                {
                    m_specularMapLabel = value;
                    RaisePropertyChanged("SpecularMapLabel");
                }
            }
        }

        public string IconURI
        {
            get { return m_iconURI; }
            set
            {
                if (value != m_iconURI)
                {
                    m_iconURI = value;
                    RaisePropertyChanged("IconURI");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
