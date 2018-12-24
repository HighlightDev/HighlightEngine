using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.ImageResources
{
    public class DefaultImageModel : INotifyPropertyChanged
    {
        private string m_headerImageSrc;

        public DefaultImageModel(string imageSrc)
        {
            this.m_headerImageSrc = imageSrc;
        }

        public string ImageSrc
        {
            get { return m_headerImageSrc; }
            set
            {
                if (value != m_headerImageSrc)
                {
                    m_headerImageSrc = value;
                    RaisePropertyChanged("ImageSrc");
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
