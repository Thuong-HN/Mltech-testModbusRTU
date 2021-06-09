using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace How_to_create_HMI_Control_Real_Time
{
    public class Register : INotifyPropertyChanged
    {
        private ushort _Address;
        private ushort _Value;

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ushort Address
        {
            get
            {
                return _Address;
            }

            set
            {
                if (_Address != value)
                {
                    _Address = value;
                    NotifyPropertyChanged("Address");
                }
            }
        }

        public ushort Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
