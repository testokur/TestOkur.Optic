﻿namespace TestOkur.Optic.Form
{
	using System.Collections.Generic;
	using System.Linq;

	public class ScanOutput
	{
		private int _index;

		public ScanOutput(IEnumerable<char> answers, int formPart, char booklet)
		{
			Answers = answers.ToArray();
			FormPart = formPart;
		}

		public char[] Answers { get; }

		public int FormPart { get; }

		public char Next()
		{
			return _index == Answers.Length ? ' ' : Answers[_index++];
		}
	}
}