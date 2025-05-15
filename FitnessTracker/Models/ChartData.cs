using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Models
{
    public class ChartData
    {
        public object XValue { get; set; }
        public double YValue { get; set; }

        public ChartData(object x, double y)
        {
            XValue = x;
            YValue = y;
        }
    }

}
