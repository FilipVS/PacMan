using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setnicka.AuxiliaryClasses
{
    /// <summary>
    /// Event args that contain the extra information about an Action
    /// </summary>
    public class ActionEventArgs : EventArgs
    {
        public ActionEventArgs(Action action)
        {
            Action = action;
        }

        public Action Action { get; }
    }
}
