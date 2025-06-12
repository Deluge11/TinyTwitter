using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    class Massages
    {
        public int ChatID { get; set; }
        public List<Msg> massagesList { get; set; }

        public Massages(int cid)
        {
            ChatID = cid;
            massagesList = new List<Msg>();
        }

        public void AddMsg(Msg msg)
        {
            massagesList.Add(msg);
        }


    }
}
