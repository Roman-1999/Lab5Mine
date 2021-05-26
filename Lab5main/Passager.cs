using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lab5main
{
    class Passager
    {

        Random random = new Random();

        public int tripsNumber;
        Mine mine;
        public string name;

            public Passager(string name,Mine mine)
        {
            this.name = name;
            this.mine = mine;
            tripsNumber = 1;
        }
        public void Run()
        {
            for (int i = 0; i < tripsNumber; i++) {
                TakeRide();
            }
        }
        public void TakeRide() {
            Console.WriteLine($"Пасажир {name}, купив бiлет на проїзд");
            mine.passagerInTurn++;
            mine.Load(this);
            Thread.Sleep(1000);
            mine.Unload(this);
        }
    }
}
