﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sakuno.KanColle.Amatsukaze
{
    public abstract class BindableObject : IBindable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
