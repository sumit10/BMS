using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BMS.Model;
namespace BMS.Model
{
    class application
    {
        public static ObservableCollection<ledgerbalance> tb;
        public application()
        {
            tb = new ledgermodel().gettrialbalance();
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                