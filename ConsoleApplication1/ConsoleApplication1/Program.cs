using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {


        }

        public float RemainderAfterDivision(int x, int y) {
            int remainder = 0;
            while(x > y){
                x -= y;
                remainder++;
            }
            return remainder;
        }
    }
}
