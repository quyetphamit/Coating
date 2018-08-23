using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoatingSupport.Entities
{
    public class Position
    {
        public int step { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public string z1 { get; set; }
        public string z2 { get; set; }
        public int speed { get; set; }
        public int delay { get; set; }
        public int up { get; set; }
        public string spray { get; set; }
        public string area { get; set; } = "1";
        public static Position FromCsv(string line)
        {
            var col = line.Split(',');
            Position result = new Position()
            {
                step = Convert.ToInt32(col[0]),
                x = Convert.ToInt32(col[1]),
                y = Convert.ToInt32(col[2]),
                z = Convert.ToInt32(col[3]),
                z1 = col[4],
                z2 = col[5],
                delay = Convert.ToInt32(col[6]),
                speed = Convert.ToInt32(col[7]),
                spray = col[8],
                up = Convert.ToInt32(col[9]),
                area = col[10]
            };
            return result;
        }
    }
}
