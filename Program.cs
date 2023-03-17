using System.IO;

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
		public int Count = 0;

		public void Serialize(FileStream s)
		{

		}

		public void Deserialize(FileStream s)
		{
		}
	}

}
