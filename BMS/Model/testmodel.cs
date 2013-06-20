using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BMS.Model
{
    class testmodel : INotifyPropertyChanged
    {
        string _lid;
        String _lname,_group;
        //public testmodel(string lid,string lname,string group)
        //{
        //    this._lid = lid;
        //    this._lname = lname;
        //    this._group = group;
        //}
        public string ledgernumder 
        {
            get { return _lid;}
            set
            {
                _lid = value;
             // OnPropertyChanged("ledger id");
            }
        }
        public string lname
        {
            get { return lname; }
            set
            {
                _lname = value;
           //     OnPropertyChanged("l name");
            }
        }
        public string lgroup
        {
            get { return _group; }
            set
            {
                _group = value;
            //    OnPropertyChanged("group");
            }
        }
        public override string ToString()
        {
            return String.Format("{0} {1}", lname,lgroup);
        }

        private void OnPropertyChanged(string p)
        {
            throw new NotImplementedException();
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanged Members

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        #endregion
    }
}
