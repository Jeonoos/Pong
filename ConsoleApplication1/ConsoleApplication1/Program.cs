﻿using System;
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
            int getaltest = int.Parse(Console.ReadLine());

        }

        public bool IsPerfectGetal(int getal) {
            int x = 0;
            for (int i = getal-1; i > 0; i--)
            {
                if (getal % i == 0)
                    x += i;
            }
            return (x == getal);
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
