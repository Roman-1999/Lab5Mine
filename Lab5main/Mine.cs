using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lab5main
{
    abstract class Mine
    {
        public int capicity { get; set; }
        public int passagerIn { get; set; }

        public int passagerInTurn = 0;

        public abstract void Load(Passager passager);
        public abstract void Unload(Passager passager);
        public abstract void Run();
    }

    class SemaphoreMine : Mine {

        Semaphore semaphoreMainCapicity;
        Semaphore semaphoreMainUnload;
        Mutex mutex = new Mutex();

        public SemaphoreMine(int capicity) {
            this.capicity = capicity;
            passagerIn = 0;
            semaphoreMainCapicity = new Semaphore(0, capicity);
            semaphoreMainUnload = new Semaphore(0, capicity);
        }

        public override void Run() {
            do {
                semaphoreMainCapicity.Release(capicity);
                Console.WriteLine("Вагонетка заходить на нове коло");
                while(passagerIn != capicity) {
                    Thread.Sleep(1000);
                }
                mutex.WaitOne();
                Console.WriteLine("Вагонетка заповнена i вирушає");
                
                Thread.Sleep(5000);
                
                Console.WriteLine("Вагонетка завершила поїздку");
                semaphoreMainUnload.Release(capicity);
                mutex.ReleaseMutex();
                while (passagerIn>0)
                {
                    Console.WriteLine($"Вагонетка очікує розвантаження {passagerIn}");;
                    Thread.Sleep(1000);
                }
            }
            while(passagerInTurn>=capicity);
            Console.WriteLine("Вагонетка завершила день, пасажирiв недостатньо");
        }

        public override void Load(Passager passager)
        {
            Console.WriteLine($" {passager.name} пробує сiсти");
            semaphoreMainCapicity.WaitOne();
            mutex.WaitOne();

            if (passagerIn < capicity)
            {
                passagerIn++;
                passagerInTurn--;
                Console.WriteLine($" {passager.name} сiв");

            }
            else {
                Console.WriteLine($"{passager.name}, Вагонетка заповнена");
            }
            mutex.ReleaseMutex();
        }

        public override void Unload(Passager passager)
        {
            semaphoreMainUnload.WaitOne();
            mutex.WaitOne();
            Console.WriteLine($" {passager.name} виходить");

            passagerIn--;
            mutex.ReleaseMutex();
        }

    }

    class MonitorMine : Mine {

        private object workBlock = new object();
        private object passangerLoadBlock = new object();
        private object passangerUnloadBlock = new object();
        private int passangerUn = 0;
        private int passangerOn = 0;

        public MonitorMine(int capicity)
        {
            this.capicity = capicity;
            passagerIn = 0;
        }

        public override void Run()
        {
            do
            {
                lock (passangerLoadBlock) {
                    for (int i = 0; i < capicity; i++) {
                        passangerOn++;
                        Monitor.Pulse(passangerLoadBlock);
                    }
                }
                Console.WriteLine("Вагонетка заходить на нове коло");
                while (passagerIn != capicity)
                {
                    Thread.Sleep(1000);
                }
                Monitor.Enter(workBlock);
                Console.WriteLine("Вагонетка заповнена i вирушає");

                Thread.Sleep(5000);

                Console.WriteLine("Вагонетка завершила поїздку");
                lock (passangerUnloadBlock)
                {
                    for (int i = 0; i < capicity; i++)
                    {
                        passangerUn++;
                        Monitor.Pulse(passangerUnloadBlock);
                    }
                }
                Monitor.Exit(workBlock);
                while (passagerIn > 0)
                {
                    Console.WriteLine($"Вагонетка очікує розвантаження {passagerIn}"); ;
                    Thread.Sleep(1000);
                }
            }
            while (passagerInTurn >= capicity);
            Console.WriteLine("Вагонетка завершила день, пасажирiв недостатньо");
        }

        public override void Load(Passager passager)
        {
            Console.WriteLine($" {passager.name} пробує сiсти");

            lock (passangerLoadBlock)
            {
                while (passangerOn <= 0)
                {
                    Monitor.Wait(passangerLoadBlock);
                }
                Monitor.Enter(workBlock);
                if (passagerIn < capicity)
                {
                    passagerIn++;
                    passagerInTurn--;
                    Console.WriteLine($" {passager.name} сiв");
                }
                else
                {
                    Console.WriteLine($"{passager.name}, Вагонетка заповнена");
                }
                passangerOn--;
                Monitor.Exit(workBlock);
                
            }

        }

        public override void Unload(Passager passager)
        {

            Monitor.Enter(workBlock);
            
            lock (passangerUnloadBlock)
            {
                while (passangerUn <= 0)
                {
                    Monitor.Wait(passangerUnloadBlock);
                }
                Console.WriteLine($" {passager.name} виходить");
                passangerUn--;
            }

            passagerIn--;
            Monitor.Exit(workBlock);
        }
    }
}
