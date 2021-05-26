using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lab5main
{
    class Program
    {
        static void Main(string[] args)
        {
            int capicity = 2;
            int passagersCount = 10;
            if (capicity > passagersCount)
            {
                Console.WriteLine("Помилка, загальна вмiст вагонетки перевущує загальну кiлькiсть людей!");
            }
            else
            {
                Mine mine = new SemaphoreMine(capicity);
                //Mine mine = new MonitorMine(capicity);
                var tasks = new Task[passagersCount + 1];
                tasks[passagersCount] = new Task(mine.Run);
                for (int i = 0; i < passagersCount; i++) {
                    tasks[i] = new Task(new Passager("name" + i, mine).Run);
                }
                Array.ForEach(tasks, x => x.Start());
                Task.WaitAll(tasks.ToArray());
            }
        }
    }
}
