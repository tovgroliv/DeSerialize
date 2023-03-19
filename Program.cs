using System;
using System.IO;

namespace DeSerialize
{
	class Program
	{
		static void Main(string[] args)
		{
			ListRand list = new ListRand();

			for (int i = 0; i < 10; i++)
			{
				list.Push(i.ToString());
			}

			list.Print();

			string path = "dump.txt";

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			using (File.Create(path))
			{

			}

			using (StreamWriter s = new StreamWriter(path))
			{
				list.Serialize(s);
			}

			using (StreamReader s = new StreamReader(path))
			{
				list.Deserialize(s);
			}
			list.Print();

			Console.ReadKey();
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

		enum Active
		{
			Head = 0, Local = 1, Reverse = 2, Tail = 3
		}

		public void Print()
		{
			ListNode active = Head;
			for (int i = 0; i < Count; i++)
			{
				string rand = "";
				if (active.Rand != null)
				{
					rand = $"({active.Rand.Data})";
				}
				Console.Write($"{active.Data}{rand} ");
				active = active.Next;
			}
			Console.WriteLine();
		}

		public void Push(string data)
		{
			if (Count == 0)
			{
				Head = new ListNode();
				Head.Data = data;
				Tail = Head;
			}
			else
			{
				Tail.Next = new ListNode();
				Tail.Next.Prev = Tail;
				Tail = Tail.Next;
				Tail.Data = data;
			}

			Count++;
		}

		public void Serialize(StreamWriter s)
		{
			ListNode activeHead = Head;
			ListNode activeTail = Tail;

			s.WriteLine(Count);

			for (int i = 0; i < Count; i++)
			{
				ListNode active = activeTail;
				ListNode activeReverse = activeHead;

				if (i % 2 == 0)
				{
					active = activeHead;
					activeReverse = activeTail;
				}

				string output = active.Data;

				if (active.Rand != null)
				{
					int delta = 0;
					int n = 0;

					FindDelta(ref delta, ref n, active, activeReverse);

					output = $"{output}|{delta}|{n}";
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
			Count = int.Parse(s.ReadLine());
			Head = new ListNode();
			Tail = new ListNode();

			ListNode activeHead = Head;
			ListNode activeTail = Tail;

			for (int i = 0; i < Count; i++)
			{
				ListNode active = activeTail;
				ListNode activeReverse = activeHead;

				if (i % 2 == 0)
				{
					active = activeHead;
					activeReverse = activeTail;
				}

				string input = s.ReadLine();

				string[] param = input.Split('|');

				if (param.Length == 3)
				{
					SetRand(int.Parse(param[1]), int.Parse(param[2]), active, activeReverse);
				}
				else
				{
					active.Data = param[0];
				}

				if (i % 2 == 0)
				{
					if (active.Next == null)
					{
						active.Next = new ListNode();
						active.Next.Prev = active;
					}
					activeHead = active.Next;
				}
				else
				{
					if (active.Prev == null)
					{
						active.Prev = new ListNode();
						active.Prev.Next = active;
					}
					activeTail = active.Prev;
				}
			}

			activeHead.Prev.Next = activeTail.Next;
			activeTail.Next.Prev = activeHead.Prev;
		}

		private void SetRand(int delta, int n, ListNode activeLocal, ListNode activeReverse)
		{
			ListNode active = activeLocal;

			switch ((Active)delta)
			{
				case Active.Head: active = Head; break;
				case Active.Local: active = activeLocal; break;
				case Active.Reverse: active = activeReverse; break;
				case Active.Tail: active = Tail; break;
			}

			for (int i = 0; i < Math.Abs(n); i++)
			{
				if (n < 0)
				{
					if (active.Prev == null)
					{
						active.Prev = new ListNode();
						active.Prev.Next = active;
					}
					active = active.Prev;
				}
				else
				{
					if (active.Next == null)
					{
						active.Next = new ListNode();
						active.Next.Prev = active;
					}
					active = active.Next;
				}
			}

			activeLocal.Rand = active;
		}

		private void FindDelta(ref int delta, ref int n, ListNode activeLocal, ListNode activeReverse)
		{
			ListNode find = activeLocal.Rand;

			ListNode activeHead = Head;
			ListNode activeTail = Tail;
			ListNode activeLocalHead = activeLocal;
			ListNode activeLocalTail = activeLocal;
			ListNode activeReverseHead = activeReverse;
			ListNode activeReverseTail = activeReverse;

			for (int i = 0; i < Count; i += 6)
			{
				if (find == activeHead)
				{
					delta = (int)Active.Head;
					n = i / 4;
					return;
				}
				if (find == activeTail)
				{
					delta = (int)Active.Tail;
					n = -i / 4;
					return;
				}
				if (find == activeLocalHead)
				{
					delta = (int)Active.Local;
					n = i / 4;
					return;
				}
				if (find == activeLocalTail)
				{
					delta = (int)Active.Local;
					n = -i / 4;
					return;
				}
				if (find == activeReverseHead)
				{
					delta = (int)Active.Reverse;
					n = i / 4;
					return;
				}
				if (find == activeReverseTail)
				{
					delta = (int)Active.Reverse;
					n = -i / 4;
					return;
				}

				activeHead = activeHead.Next;
				activeTail = activeTail.Prev;
				activeLocalHead = activeLocalHead.Next;
				activeLocalTail = activeLocalTail.Prev;
				activeReverseHead = activeReverseHead.Next;
				activeReverseTail = activeReverseTail.Prev;
			}
		}
	}
}
