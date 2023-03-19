﻿using System.IO;
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

		private void FindDelta(ref int delta, ref int n, ListNode active, ListNode activeReverse)
		{
			ListNode find = active.Rand;

			ListNode activeHead = Head;
			ListNode activeTail = Tail;
			ListNode activeLocalHead = active;
			ListNode activeLocalTail = active;
			ListNode activeReverseHead = activeReverse;
			ListNode activeReverseTail = activeReverse;

			for (int i = 0; i < Count; i += 6)
			{
				if (find == activeHead)
				{
					delta = 0;
					n = i / 4;
					return;
				}
				if (find == activeTail)
				{
					delta = 0;
					n = -i / 4;
					return;
				}
				if (find == activeLocalHead)
				{
					delta = 1;
					n = i / 4;
					return;
				}
				if (find == activeLocalTail)
				{
					delta = 1;
					n = -i / 4;
					return;
				}
				if (find == activeReverseHead)
				{
					delta = 2;
					n = i / 4;
					return;
				}
				if (find == activeReverseTail)
				{
					delta = 2;
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
