using MuseJet.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseJet.GUI.Events
{
    public enum ChangeType
    {
        Add,
        Edit,
        Delete,
    }

    public class StationStateChangeEventArgs : EventArgs
    {
        public ChangeType Type { get; set; }
        public Station ChangedStation { get; set; }
    }
}
