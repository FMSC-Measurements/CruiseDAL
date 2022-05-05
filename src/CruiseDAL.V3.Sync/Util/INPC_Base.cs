using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync.Util
{
    public class INPC_Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public void SetProperty<tTarget>(ref tTarget target, tTarget value, [CallerMemberName] string propName = null)
        {
            if (object.Equals(target, value)) { return; }
            target = value;
            if (propName != null) { RaisePropertyChanged(propName); }
        }
    }
}
