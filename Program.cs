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

			using (FileStream fs = File.Create(path))
			{
				list.Serialize(fs);
			}

			using (FileStream fs = File.OpenRead(path))
			{
				list.Deserialize(fs);
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

		public void Serialize(FileStream s)
		{
			ListNode activeHead = Head;
			ListNode activeTail = Tail;

			AddText(s, $"{Count}\n");

			for (int i = 0; i < Count; i++)
			{
				ListNode active = activeTail;

				if (i % 2 == 0)
				{
					active = activeHead;
				}

				string output = active.Data;

				if (active.Rand == null)
				{
					output += "\n";
				}
				else
				{
					bool delta = true;
					int n = 0;

					FindDelta(ref delta, ref n, active);

					output = $"{output} | {delta} | {n}\n";
				}

				AddText(s, output);

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

		public void Deserialize(FileStream s)
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

		private void AddText(FileStream fs, string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			fs.Write(info, 0, info.Length);
		}
	}

}
