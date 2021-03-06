﻿namespace TestOkur.Optic.Form
{
	using System.Collections.Generic;
	using System.Linq;
	using TestOkur.Optic.Answer;
	using TestOkur.Optic.Score;

	public class AnswerKeyOpticalForm : OpticalForm
	{
		public AnswerKeyOpticalForm(char booklet, List<ScoreFormula> scoreFormulas)
			: this()
		{
			Booklet = booklet;
			ScoreFormulas = scoreFormulas;
		}

		public AnswerKeyOpticalForm()
		{
			Sections = new List<AnswerKeyOpticalFormSection>();
			ScoreFormulas = new List<ScoreFormula>();
		}

		public AnswerKeyOpticalForm(AnswerKeyOpticalForm form, char booklet)
			: base(form, booklet)
		{
			IncorrectEliminationRate = form.IncorrectEliminationRate;
			Sections = new List<AnswerKeyOpticalFormSection>();
			ScoreFormulas = form.ScoreFormulas;
		}

		public bool Empty => Sections.SelectMany(s => s.Answers).All(a => a.QuestionNo == default);

		public int IncorrectEliminationRate { get; set; }

		public List<AnswerKeyOpticalFormSection> Sections { get; set; }

		public IReadOnlyList<AnswerKeyQuestionAnswer> Answers => Sections.SelectMany(s => s.Answers).ToList();

		public List<ScoreFormula> ScoreFormulas { get; set; }

		public bool SharedExam { get; set; }

		public void AddSection(AnswerKeyOpticalFormSection section)
		{
			Sections.Add(section);
		}

		public void AddAnswer(
			AnswerKeyOpticalFormSection section,
			int questionNo,
			int questionNoBookletB,
			int questionNoBookletC,
			int questionNoBookletD,
			char answer,
			int subjectId,
			string subject,
			QuestionAnswerCancelAction questionAnswerCancelAction)
		{
			if (!Sections.Contains(section))
			{
				Sections.Add(new AnswerKeyOpticalFormSection(section));
			}

			Sections
				.First(s => s.LessonName == section.LessonName)
				.AddAnswer(new AnswerKeyQuestionAnswer(questionNo, questionNoBookletB, questionNoBookletC, questionNoBookletD, answer, subjectId, subject, questionAnswerCancelAction));
		}

		public void AddAnswer(AnswerKeyOpticalFormSection section, int questionNo, AnswerKeyQuestionAnswer questionAnswer)
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
			var formDictionary = CreateFormsForAllBooklets();

			foreach (var section in Sections)
			{
				foreach (var answer in section.Answers)
				{
					formDictionary['A'].AddAnswer(section, answer.QuestionNo, answer.QuestionNoBookletB, answer.QuestionNoBookletC, answer.QuestionNoBookletD, answer.Answer, answer.SubjectId, answer.SubjectName, answer.QuestionAnswerCancelAction);
					formDictionary['B'].AddAnswer(section, answer.QuestionNoBookletB, answer);
					formDictionary['C'].AddAnswer(section, answer.QuestionNoBookletC, answer);
					formDictionary['D'].AddAnswer(section, answer.QuestionNoBookletD, answer);
				}
			}

			return formDictionary.Values.Where(a => !a.Empty).ToList();
		}

		private Dictionary<char, AnswerKeyOpticalForm> CreateFormsForAllBooklets()
		{
			var formDictionary = new Dictionary<char, AnswerKeyOpticalForm>()
			{
				{ 'A', new AnswerKeyOpticalForm(this, 'A') },
				{ 'B', new AnswerKeyOpticalForm(this, 'B') },
				{ 'C', new AnswerKeyOpticalForm(this, 'C') },
				{ 'D', new AnswerKeyOpticalForm(this, 'D') },
			};
			return formDictionary;
		}
	}
}
