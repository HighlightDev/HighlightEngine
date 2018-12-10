using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfControlLibrary1.Models
{
    public class EngineObjectEntryModel { }

    public class EngineObjectEntry : INotifyPropertyChanged
    {
        private string m_entryLabel;
        private string m_modelLabel;
        private string m_albedoTextureLabel;
        private string m_iconURI;

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
