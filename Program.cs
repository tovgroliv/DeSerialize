using System.IO;
using System.Text;

namespace DeSerialize
{
	class Program
	{
		static void Main(string[] args)
		{
			ListRand list = new ListRand();

			// TODO заполнить список (мб рандом?)

			string path = "dump.txt";

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			File.Create(path);

			using (StreamWriter s = new StreamWriter(path))
			{
				list.Serialize(s);
			}

			using (StreamReader s = new StreamReader(path))
			{
				list.Deserialize(s);
			}
		}
	}

	class ListNode
	{
		public ListNode Prev;
		public ListNode Next;
		public ListNode Rand;
		public string Data;
	}


	class ListRand
	{
		public ListNode Head;
		public ListNode Tail;
		public int Count;

		public void Serialize(StreamWriter s)
		{
			ListNode activeHead = Head;
			ListNode activeTail = Tail;

			s.WriteLine(Count);

			for (int i = 0; i < Count; i++)
			{
				ListNode active = activeTail;

				if (i % 2 == 0)
				{
					active = activeHead;
				}

				string output = active.Data;

				if (active.Rand != null)
				{
					bool delta = true;
					int n = 0;

					FindDelta(ref delta, ref n, active);

					output = $"{output} | {delta} | {n}";
				}

				s.WriteLine(output);

				if (i % 2 == 0)
				{
					activeHead = active.Next;
				}
				else
				{
					activeTail = active.Prev;
				}
			}
		}

		public void Deserialize(StreamReader s)
		{
			
		}

		private void FindDelta(ref bool delta, ref int n, ListNode active)
		{
			ListNode find = active.Rand;

			ListNode activeHead = Head;
			ListNode activeTail = Tail;
			ListNode activeLocalHead = active;
			ListNode activeLocalTail = active;

			for (int i = 0; i < Count; i += 4)
			{
				if (find == activeHead)
				{
					delta = false;
					n = i / 4;
					return;
				}
				if (find == activeTail)
				{
					delta = false;
					n = -i / 4;
					return;
				}
				if (find == activeLocalHead)
				{
					delta = true;
					n = i / 4;
					return;
				}
				if (find == activeLocalTail)
				{
					delta = true;
					n = -i / 4;
					return;
				}

				activeHead = activeHead.Next;
				activeTail = activeTail.Prev;
				activeLocalHead = activeLocalHead.Next;
				activeLocalTail = activeLocalTail.Prev;
			}
		}
	}

}
