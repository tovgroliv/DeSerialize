using System;
using System.IO;

namespace DeSerialize
{
	/// <summary>
	/// Основной класс программы
	/// </summary>
	class Program
	{
		/// <summary>
		/// Стартовая функция выполнения программы.
		/// </summary>
		/// <param name="args">Входные аргументы</param>
		static void Main(string[] args)
		{
			for (int i = 0; i < 1000; i++)
			{
				Test(i);
			}

			Console.ReadKey();
		}

		/// <summary>
		/// Проведение тестирования работы списка.
		/// </summary>
		/// <param name="number">Порядковый номер теста</param>
		static void Test(int number)
		{
			Console.Write($"Test #{number} - ");

			ListRand list = new ListRand();
			Random rand = new Random();

			for (int i = 0; i < rand.Next(5, 20); i++)
			{
				list.Push(i.ToString());
			}

			list.Randomize();
			string first = list.ToString();

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
			string second = list.ToString();

			if (first == second)
			{
				Console.WriteLine("true");
			}
			else
			{
				Console.WriteLine("false");
				Console.WriteLine(first);
				Console.WriteLine(second);
			}
		}
	}

	/// <summary>
	/// Элемент списка
	/// </summary>
	class ListNode
	{
		/// <summary>
		/// Предыдущий элемент.
		/// </summary>
		public ListNode Prev;
		/// <summary>
		/// Следующий элемент.
		/// </summary>
		public ListNode Next;
		/// <summary>
		/// Случайный элемент.
		/// </summary>
		public ListNode Rand;
		/// <summary>
		/// Данные элемента.
		/// </summary>
		public string Data;
	}

	/// <summary>
	/// Список
	/// </summary>
	class ListRand
	{
		/// <summary>
		/// Первый элемент списка.
		/// </summary>
		public ListNode Head;
		/// <summary>
		/// Последний элемент списка.
		/// </summary>
		public ListNode Tail;
		/// <summary>
		/// Количество элементов в списке.
		/// </summary>
		public int Count;

		/// <summary>
		/// Перечисление местоположения указателя активного элемента.
		/// </summary>
		private enum Active
		{
			/// <summary>
			/// Начало списка.
			/// </summary>
			Head = 0,
			/// <summary>
			/// Текущий элемент.
			/// </summary>
			Local = 1,
			/// <summary>
			/// Обратный элемент текущему (равно удален, только с другого конца).
			/// </summary>
			Reverse = 2,
			/// <summary>
			/// Конец списка.
			/// </summary>
			Tail = 3
		}

		/// <summary>
		/// Преобразование списка в строку.
		/// Формат data или data|delta|n,
		///		где data - данные элемента списка (string),
		///			delta - точка начало отсчета (Active),
		///			n - сдвиг от точки начала отсчета (int).
		/// </summary>
		/// <returns>Представление списка в строке.</returns>
		public override string ToString()
		{
			string output = "";
			ListNode active = Head;
			for (int i = 0; i < Count; i++)
			{
				string rand = "";
				if (active.Rand != null)
				{
					rand = $"({active.Rand.Data})";
				}
				output += $"{active.Data}{rand} ";
				active = active.Next;
			}

			return output;
		}

		/// <summary>
		/// Добавить элемент в конец списка.
		/// Используется для тестового представления данных.
		/// </summary>
		/// <param name="data">Данные элемента списка.</param>
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

		/// <summary>
		/// Добавлен случайных указателей в список.
		/// Используется для тестового представления данных.
		/// </summary>
		public void Randomize()
		{
			ListNode active = Head;
			Random random = new Random();

			for (int i = 0; i < Count; i++)
			{
				if (random.Next(0, 2) == 1)
				{
					int id = random.Next(0, Count);

					ListNode rand = Head;
					for (int j = 0; j < id; j++)
					{
						rand = rand.Next;
					}
					active.Rand = rand;
				}

				active = active.Next;
			}
		}

		/// <summary>
		/// Сериализация списка.
		/// </summary>
		/// <param name="s">Поток в который записывается список</param>
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

		/// <summary>
		/// Десериализация списка.
		/// </summary>
		/// <param name="s">Поток из которого читается список</param>
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

				active.Data = param[0];

				if (i % 2 == 0)
				{
					if (i == Count - 2)
					{
						activeHead.Next = activeTail;
						activeTail.Prev = activeHead;
					}
					else if (active.Next == null)
					{
						active.Next = new ListNode();
						active.Next.Prev = active;
					}
					activeHead = active.Next;
				}
				else
				{
					if (i == Count - 2)
					{
						activeHead.Next = activeTail;
						activeTail.Prev = activeHead;
					}
					else if (active.Prev == null)
					{
						active.Prev = new ListNode();
						active.Prev.Next = active;
					}
					activeTail = active.Prev;
				}
			}
		}

		/// <summary>
		/// Поиск и установка рандомного указателя.
		/// </summary>
		/// <param name="delta">Точка начала отсчета</param>
		/// <param name="n">Смещение от начала отсчета</param>
		/// <param name="activeLocal">Текущий элемент</param>
		/// <param name="activeReverse">Обратный элемент текущему</param>
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

		/// <summary>
		/// Нахождение смещения до рандомного указателя.
		/// </summary>
		/// <param name="delta">Изменяемое значение точки начала отсчета</param>
		/// <param name="n">Изменяемое значение смещения от точки начала отсчета</param>
		/// <param name="activeLocal">Текущий элемент</param>
		/// <param name="activeReverse">Обратный элемент текущему</param>
		private void FindDelta(ref int delta, ref int n, ListNode activeLocal, ListNode activeReverse)
		{
			ListNode find = activeLocal.Rand;

			ListNode activeHead = Head;
			ListNode activeTail = Tail;
			ListNode activeLocalHead = activeLocal;
			ListNode activeLocalTail = activeLocal;
			ListNode activeReverseHead = activeReverse;
			ListNode activeReverseTail = activeReverse;

			int i = 0;

			while (activeHead != null ||
				activeTail != null ||
				activeLocalHead != null ||
				activeLocalTail != null ||
				activeReverseHead != null ||
				activeReverseTail != null)
			{
				if (find == activeHead)
				{
					delta = (int)Active.Head;
					n = i;
					return;
				}
				if (find == activeTail)
				{
					delta = (int)Active.Tail;
					n = -i;
					return;
				}
				if (find == activeLocalHead)
				{
					delta = (int)Active.Local;
					n = i;
					return;
				}
				if (find == activeLocalTail)
				{
					delta = (int)Active.Local;
					n = -i;
					return;
				}
				if (find == activeReverseHead)
				{
					delta = (int)Active.Reverse;
					n = i;
					return;
				}
				if (find == activeReverseTail)
				{
					delta = (int)Active.Reverse;
					n = -i;
					return;
				}

				i++;

				if (activeHead != null)
				{
					activeHead = activeHead.Next;
				}
				if (activeTail != null)
				{
					activeTail = activeTail.Prev;
				}
				if (activeLocalHead != null)
				{
					activeLocalHead = activeLocalHead.Next;
				}
				if (activeLocalTail != null)
				{
					activeLocalTail = activeLocalTail.Prev;
				}
				if (activeReverseHead != null)
				{
					activeReverseHead = activeReverseHead.Next;
				}
				if (activeReverseTail != null)
				{
					activeReverseTail = activeReverseTail.Prev;
				}
			}
		}
	}
}
