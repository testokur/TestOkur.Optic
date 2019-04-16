﻿namespace TestOkur.Optic.Form
{
	using System.Collections.Generic;
	using System.Linq;
	using TestOkur.Optic.Answer;
	using TestOkur.Optic.Score;

	public class AnswerKeyOpticalForm : OpticalForm
	{
		public AnswerKeyOpticalForm()
		{
			Sections = new List<AnswerKeyOpticalFormSection>();
		}

		private AnswerKeyOpticalForm(AnswerKeyOpticalForm form, char booklet)
			: base(form, booklet)
		{
			IncorrectEliminationRate = form.IncorrectEliminationRate;
			Sections = new List<AnswerKeyOpticalFormSection>();
		}

		public bool Empty => Sections.SelectMany(s => s.Answers).All(a => a.QuestionNo == default);

		public int IncorrectEliminationRate { get; set; }

		public List<AnswerKeyOpticalFormSection> Sections { get; set; }

		public List<AnswerKeyQuestionAnswer> Answers => Sections.SelectMany(s => s.Answers).ToList();

		public List<ScoreFormula> ScoreFormulas { get; set; }

		public void AddSection(AnswerKeyOpticalFormSection section)
		{
			Sections.Add(section);
		}

		public void AddAnswer(AnswerKeyOpticalFormSection section, int questionNo, QuestionAnswer questionAnswer)
		{
			if (!Sections.Contains(section))
			{
				Sections.Add(new AnswerKeyOpticalFormSection(section));
			}

			Sections
				.First(s => s.LessonName == section.LessonName)
				.AddAnswer(new AnswerKeyQuestionAnswer(questionNo, questionAnswer));
		}

		public List<AnswerKeyOpticalForm> Expand()
		{
			var formA = new AnswerKeyOpticalForm(this, 'A');
			var formB = new AnswerKeyOpticalForm(this, 'B');
			var formC = new AnswerKeyOpticalForm(this, 'C');
			var formD = new AnswerKeyOpticalForm(this, 'D');

			foreach (var section in Sections)
			{
				foreach (var answer in section.Answers)
				{
					formA.AddAnswer(section, answer.QuestionNo, answer);
					formB.AddAnswer(section, answer.QuestionNoBookletB, answer);
					formC.AddAnswer(section, answer.QuestionNoBookletC, answer);
					formD.AddAnswer(section, answer.QuestionNoBookletD, answer);
				}
			}

			var list = new List<AnswerKeyOpticalForm>()
			{
				formA,
				formB,
				formC,
				formD
			};

			return list.Where(a => !a.Empty).ToList();
		}
	}
}
