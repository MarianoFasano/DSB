using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace cSharpeTest
{
	public class ThreadTest
	{
		public ThreadTest()
		{
		}

		//Delegate
		public event EventHandler ErrorEvent;
		public async void start()
        {
			//Task to simulate an error in DSB
			//await Task.Run(() => doTask());

			Thread t = new Thread(() => doTask());
			t.Start();

		}
		private void doTask()
        {
			//If the countdown is a multiple of number, a new message on top is generated
			Random random = new Random();
			int number = random.Next(5, 18);

			for (int i = 50; i > 5; i--)
			{
				if (i % number == 0)
				{
					errorEvent();
				}
				Thread.Sleep(300);
			}
		}

		private void errorEvent()
        {
			if(ErrorEvent != null)
            {
				//Event
				ErrorEvent(this, EventArgs.Empty);
            }
        }
	}
}

