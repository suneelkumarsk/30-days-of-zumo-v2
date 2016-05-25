using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ClientApp.ViewModels
{
    public class Base : INotifyPropertyChanged
    {
        #region Properties
        private string propTitle = string.Empty;
        public string Title
        {
            get { return propTitle; }
            set { SetProperty(ref propTitle, value, "Title"); }
        }

        private string propSubTitle = string.Empty;
        public string Subtitle
        {
            get { return propSubTitle; }
            set { SetProperty(ref propSubTitle, value, "Subtitle"); }
        }

        private string propIcon;
        public string Icon
        {
            get { return propIcon; }
            set { SetProperty(ref propIcon, value, "Icon"); }
        }

        private bool propIsBusy;
        public bool IsBusy
        {
            get { return propIsBusy; }
            set { SetProperty(ref propIsBusy, value, "IsBusy"); }
        }
        #endregion

        #region INotifyPropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T store, T value, string propName, Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(store, value))
                return;
            store = value;

            if (onChanged != null)
                onChanged();
            OnPropertyChanged(propName);
        }

        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
