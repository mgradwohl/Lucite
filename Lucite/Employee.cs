using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.ObjectModel;

namespace Lucite
{
    public class Employee
    {
        private string name;
        private DateTime awarddate;
        private DateTime nextaward;
        private DateTime startdate;
        private int award = -1;

        private string anniversary;
        private bool fanniversary = false;

        public string Name //0
        {
            get { return name; }
            set { name = value; }
        }


        public DateTime StartDateTime //1
        {
            get { return startdate; }
            set { startdate = value; }
        }

        public string StartDate //2
        {
            get { return startdate.ToShortDateString(); }
            set { startdate = DateTime.Parse(value); }
        }

        public DateTime AwardDateTime //3
        {
            get { return awarddate;  }
            set { awarddate = value; }
        }

        public string AwardDate //4
        {
            get
            {
                if (this.fanniversary == true)
                {
                    return awarddate.ToShortDateString();
                }
                else
                {
                    return string.Empty;
                };
            }
            
            set { awarddate = DateTime.Parse(value);  }
        }

        public string NextAward //5
        {
            get { return nextaward.ToShortDateString(); }
            set { nextaward = DateTime.Parse(value); }
        }

        public DateTime NextAwardDateTime //6
        {
            get { return nextaward; }
            set { nextaward = value; }
        }

        public bool AwardFlag //7
        {
            get { return fanniversary; }
            set { fanniversary = value; }
        }

        public string Anniversary //8
        {
            get { return anniversary; }
            set { anniversary = value; }
        }

        public int Award //9
        {
            get { return award; }
            set { award = value; }
        }
    }

    public class Employees : ObservableCollection<Employee>
    {
    }

}
