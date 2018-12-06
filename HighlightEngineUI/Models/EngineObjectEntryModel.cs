using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HighlightEngineUI.Models
{
    public class EngineObjectEntryModel { }

    public class EngineObjectEntry : INotifyPropertyChanged
    {
        private string m_entryLabel;
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
