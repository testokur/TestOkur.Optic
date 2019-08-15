﻿namespace TestOkur.Optic.Form
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Serialization;
	using TestOkur.Optic.Answer;
	using TestOkur.Optic.Score;
	using static System.Math;

	[DataContract]
	public class StudentOpticalForm : OpticalForm
	{
		private const char DefaultBooklet = 'A';

		public StudentOpticalForm(char booklet)
			: this()
		{
			Booklet = char.IsWhiteSpace(booklet) ? DefaultBooklet : booklet;
		}

		public StudentOpticalForm()
		{
			Sections = new List<StudentOpticalFormSection>();
			Scores = new Dictionary<string, float>();
			Orders = new List<StudentOrder>();
		}

		[DataMember]
		public int StudentId { get; set; }

		[DataMember]
		public List<StudentOpticalFormSection> Sections { get; set; }

		[DataMember]
		public Dictionary<string, float> Scores { get; private set; }

		[DataMember]
		public Guid ScanSessionReportId { get; set; }

		[DataMember]
		public int StudentNumber { get; set; }

		[DataMember]
		public string StudentFirstName { get; set; }

		[DataMember]
		public string StudentLastName { get; set; }

		[DataMember]
		public string SchoolName { get; set; }

		[DataMember]
		public int ClassroomId { get; set; }

		[DataMember]
		public string Classroom { get; set; }

		[DataMember]
		public string CityName { get; set; }

		[DataMember]
		public int CityId { get; set; }

		[DataMember]
		public string DistrictName { get; set; }

		[DataMember]
		public int DistrictId { get; set; }

		[DataMember]
		public int GeneralAttendanceCount { get; private set; }

		[DataMember]
		public int CityAttendanceCount { get; private set; }

		[DataMember]
		public int DistrictAttendanceCount { get; private set; }

		[DataMember]
		public int SchoolAttendanceCount { get; private set; }

		[DataMember]
		public int ClassroomAttendanceCount { get; private set; }

		[DataMember]
		public List<StudentOrder> Orders { get; private set; }

		[DataMember]
		public int EmptyCount => Sections.Select(s => s.EmptyCount).Sum();

		[DataMember]
		public int WrongCount => Sections.Select(s => s.WrongCount).Sum();

		[DataMember]
		public int CorrectCount => Sections.Select(s => s.CorrectCount).Sum();

		[DataMember]
		public int QuestionCount => Sections.SelectMany(s => s.Answers).Count();

		[DataMember]
		public float Net => Sections.Select(s => s.Net).Sum();

		[DataMember]
		public float SuccessPercent => CalculateSuccessPercent();

		[DataMember]
		public float ClassroomAverageNet => Sections
			.SelectMany(s => s.Averages)
			.Where(a => a.Name == "NET")
			.Select(a => a.Classroom).Sum();

		[DataMember]
		public float SchoolAverageNet => Sections
			.SelectMany(s => s.Averages)
			.Where(a => a.Name == "NET")
			.Select(a => a.School).Sum();

		[DataMember]
		public float DistrictAverageNet => Sections
			.SelectMany(s => s.Averages)
			.Where(a => a.Name == "NET")
			.Select(a => a.District).Sum();

		[DataMember]
		public float CityAverageNet => Sections
			.SelectMany(s => s.Averages)
			.Where(a => a.Name == "NET")
			.Select(a => a.City).Sum();

		[DataMember]
		public float GeneralAverageNet => Sections
			.SelectMany(s => s.Averages)
			.Where(a => a.Name == "NET")
			.Select(a => a.General).Sum();

		[DataMember]
		public int ClassOrder => Orders?.FirstOrDefault(o => o.Name == "NET")?.ClassroomOrder ?? 0;

		[DataMember]
		public int SchoolOrder => Orders?.FirstOrDefault(o => o.Name == "NET")?.SchoolOrder ?? 0;

		[DataMember]
		public int DistrictOrder => Orders?.FirstOrDefault(o => o.Name == "NET")?.DistrictOrder ?? 0;

		[DataMember]
		public int CityOrder => Orders?.FirstOrDefault(o => o.Name == "NET")?.CityOrder ?? 0;

		[DataMember]
		public int GeneralOrder => Orders?.FirstOrDefault(o => o.Name == "NET")?.GeneralOrder ?? 0;

		[DataMember]
		public float Score => Scores.Any() ? Scores.First().Value : SuccessPercent;

		[DataMember]
		public float ClassScoreAverage { get; set; }

		[DataMember]
		public float SchoolScoreAverage { get; set; }

		[DataMember]
		public float DistrictScoreAverage { get; set; }

		[DataMember]
		public float CityScoreAverage { get; set; }

		[DataMember]
		public float GeneralScoreAverage { get; set; }

		[DataMember]
		public int ClassroomGrade { get; set; }

		public void UpdateCorrectAnswers(AnswerKeyOpticalForm answerKeyOpticalForm)
		{
			foreach (var answerKeyOpticalFormSection in answerKeyOpticalForm.Sections)
			{
				var section = Sections.FirstOrDefault(s => s.LessonName == answerKeyOpticalFormSection.LessonName);
				section.UpdateAnswers(answerKeyOpticalFormSection);
			}
		}

		public void SetFromScanOutput(ScanOutput scanOutput, AnswerKeyOpticalForm answerKeyOpticalForm)
		{
			foreach (var answerKeyOpticalFormSection in answerKeyOpticalForm
				.Sections
				.Where(s => s.FormPart == scanOutput.FormPart))
			{
				var studentOpticalFormSection = new StudentOpticalFormSection(answerKeyOpticalFormSection);

				for (var i = 0; i < answerKeyOpticalFormSection.MaxQuestionCount; i++)
				{
					var correctAnswer = answerKeyOpticalFormSection.Answers.ElementAtOrDefault(i);
					var questionAnswer = new QuestionAnswer(i + 1, scanOutput.Next());
					questionAnswer.SetCorrectAnswer(correctAnswer);
					studentOpticalFormSection.Answers.Add(questionAnswer);
				}

				Sections.Add(studentOpticalFormSection);
			}
		}

		public void AddSections(IEnumerable<StudentOpticalFormSection> sections)
		{
			Sections = Sections
				.Where(s => !sections.Select(se => se.LessonName)
					.Contains(s.LessonName))
				.ToList();

			foreach (var section in sections)
			{
				Sections.Add(section);
			}

			Sections = Sections
				.OrderBy(s => s.FormPart)
				.ThenBy(s => s.ListOrder)
				.ToList();
		}

		public void Evaluate(int incorrectEliminationRate, List<ScoreFormula> scoreFormulas)
		{
			EvaluateSections(incorrectEliminationRate);
			CalculateScore(scoreFormulas);
		}

		public bool ContainsSection(int lessonId) => Sections.Any(s => s.LessonId == lessonId);

		public void ClearOrders() => Orders.Clear();

		public void AddStudentOrder(StudentOrder item)
		{
			Orders.Add(item);
		}

		public void AddEmptySection(AnswerKeyOpticalFormSection answerKeyOpticalFormSection)
		{
			var section = new StudentOpticalFormSection(answerKeyOpticalFormSection)
			{
				Answers = answerKeyOpticalFormSection.Answers
					.Select(a => new QuestionAnswer(a.QuestionNo, QuestionAnswer.Empty))
					.ToList(),
			};
			Sections.Add(section);
		}

		public void SetAttendance(IReadOnlyCollection<StudentOpticalForm> forms)
		{
			GeneralAttendanceCount = forms.Count;
			CityAttendanceCount = forms.Count(f => f.CityId == CityId);
			DistrictAttendanceCount = forms.Count(f => f.DistrictId == DistrictId);
			ClassroomAttendanceCount = forms.Count(f => f.ClassroomId == ClassroomId);
			SchoolAttendanceCount = forms.Count(f => f.SchoolId == SchoolId);
		}

		public void SetAverages(IReadOnlyCollection<StudentOpticalForm> forms)
		{
			CityScoreAverage = forms.Where(f => f.CityId == CityId)
				.Average(f => f.Score);
			ClassScoreAverage = forms.Where(f => f.ClassroomId == ClassroomId)
				.Average(f => f.Score);
			DistrictScoreAverage = forms.Where(f => f.DistrictId == DistrictId)
				.Average(f => f.Score);
			GeneralScoreAverage = forms.Average(f => f.Score);
			SchoolScoreAverage = forms.Where(f => f.SchoolId == SchoolId)
				.Average(f => f.Score);
		}

		private void EvaluateSections(int incorrectEliminationRate)
		{
			foreach (var section in Sections)
			{
				section.Evaluate(incorrectEliminationRate);
			}
		}

		private float CalculateSuccessPercent()
		{
			if (QuestionCount == 0)
			{
				return 0;
			}

			var percent = Net * 100 / QuestionCount;

			return percent < 0 ? 0 : percent;
		}

		private void CalculateScore(List<ScoreFormula> scoreFormulas)
		{
			if (scoreFormulas == null)
			{
				return;
			}

			Scores.Clear();
			foreach (var formula in scoreFormulas)
			{
				var score = formula.BasePoint +
							formula.Coefficients
								.Select(c => c.Coefficient * Sections.FirstOrDefault(s => s.LessonName == c.Lesson)?.Net ?? 0)
								.Sum();
				Scores.Add(formula.ScoreName.ToUpperInvariant(), (float)Round(score * 100) / 100);
			}
		}
	}
}
