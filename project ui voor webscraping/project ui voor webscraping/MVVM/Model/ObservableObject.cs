using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace project_ui_voor_webscraping.MVVM.Model
{
    internal class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
