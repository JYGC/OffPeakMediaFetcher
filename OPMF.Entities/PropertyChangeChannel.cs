using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace OPMF.Entities
{
    public interface IPropertyChangeChannel : IChannel, INotifyPropertyChanged { }

    public class PropertyChangeChannel : Channel, IPropertyChangeChannel
    {
        private bool __blackListed;

        new public bool BlackListed
        {
            get
            {
                return __blackListed;
            }
            set
            {
                __blackListed = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyChangeChannel(IChannel channel)
        {
            SiteId = channel.SiteId;
            Url = channel.Url;
            Name = channel.Name;
            Description = channel.Description;
            BlackListed = channel.BlackListed;
            NotFound = channel.NotFound;
            LastCheckedOut = channel.LastCheckedOut;
            LastActivityDate = channel.LastActivityDate;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
