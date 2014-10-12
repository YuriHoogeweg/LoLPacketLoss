using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AveragePacketLoss
{
    public class Patch
    {
        public double PatchNumber;
        public DateTime StartDate;
        public DateTime EndDate;

        public Patch(double PatchNumber, DateTime StartDate, DateTime EndDate)
        {
            this.PatchNumber = PatchNumber;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
        }
    }
}
