using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public string ValidationError { get; set; }

        public bool IsFormVisible { get; set; }

        public bool IsFormBusy { get; set; }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
