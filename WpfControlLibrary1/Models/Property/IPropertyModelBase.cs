using System.ComponentModel;

namespace WpfControlLibrary1.Models.Property
{
    public class IPropertyModelBase : INotifyPropertyChanged
    {
        private string m_templateType;

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
