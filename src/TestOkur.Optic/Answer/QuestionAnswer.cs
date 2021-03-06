﻿namespace TestOkur.Optic.Answer
{
	using System.Linq;

	public class QuestionAnswer
	{
		internal const char Empty = ' ';
		private const string ValidAnswers = "ABCDE";

		public QuestionAnswer(int questionNo, char answer)
		{
			QuestionNo = questionNo;
			Answer = answer;
		}

		public QuestionAnswer()
		{
		}

		public int QuestionNo { get; set; }

		public char Answer { get; set; }

		public int SubjectId { get; set; }

		public string SubjectName { get; set; }

		public QuestionAnswerResult Result { get; set; }

		public char CorrectAnswer { get; set; }

		public void SetCorrectAnswer(AnswerKeyQuestionAnswer answerKeyQuestionAnswer)
		{
			if (answerKeyQuestionAnswer == null)
			{
				return;
			}

			AssignProperties(answerKeyQuestionAnswer);

			if (ProcessCancelAction(answerKeyQuestionAnswer))
			{
				return;
			}

			SetResult();
		}

		public override string ToString() => Answer.ToString();

		private void SetResult()
		{
			if (CorrectAnswer == Empty || CorrectAnswer == '\0')
			{
				return;
			}

			if (Answer == Empty)
			{
				Result = QuestionAnswerResult.Empty;
				return;
			}

			if (!ValidAnswers.Contains(Answer))
			{
				Result = QuestionAnswerResult.Invalid;
				return;
			}

			Result = CorrectAnswer == Answer ? QuestionAnswerResult.Correct : QuestionAnswerResult.Wrong;
		}

		private bool ProcessCancelAction(AnswerKeyQuestionAnswer answerKeyQuestionAnswer)
		{
			switch (answerKeyQuestionAnswer.QuestionAnswerCancelAction)
			{
				case QuestionAnswerCancelAction.CorrectForAll:
					Result = QuestionAnswerResult.Correct;
					return true;
				case QuestionAnswerCancelAction.EmptyForAll:
					Result = QuestionAnswerResult.Empty;
					return true;
				default:
					return false;
			}
		}

		private void AssignProperties(AnswerKeyQuestionAnswer answerKeyQuestionAnswer)
		{
			SubjectId = answerKeyQuestionAnswer.SubjectId;
			SubjectName = answerKeyQuestionAnswer.SubjectName;
			CorrectAnswer = answerKeyQuestionAnswer.Answer;
		}
	}
}
