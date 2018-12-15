using System.ComponentModel;

namespace WpfControlLibrary1.Models
{
    public class IEntityGroupsModelBase : INotifyPropertyChanged
    {
        private string m_LabelMain;
        private string m_iconUriMain;
        private string m_templateType;

        public string LabelMain
        {
            get { return m_LabelMain; }
            set
            {
                if (value != m_LabelMain)
                {
                    m_LabelMain = value;
                    RaisePropertyChanged("LabelMain");
                }
            }
        }

        public string IconMain
        {
            get { return m_iconUriMain; }
            set
            {
                if (value != m_iconUriMain)
                {
                    m_iconUriMain = value;
                    RaisePropertyChanged("IconMain");
                }
            }
        }

        public string TemplateType
        {
            get { return m_templateType; }
            set
            {
                if (value != m_templateType)
                {
                    m_templateType = value;
                    RaisePropertyChanged("TemplateType");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
