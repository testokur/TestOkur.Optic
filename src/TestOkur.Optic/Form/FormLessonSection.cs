﻿namespace TestOkur.Optic.Form
{
	public abstract class FormLessonSection
	{
		public int LessonId { get; set; }

		public string LessonName { get; set; }

		public int MaxQuestionCount { get; set; }

		public int FormPart { get; set; }

		public int ListOrder { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (GetType() != obj.GetType())
			{
				return false;
			}

			return obj is FormLessonSection other &&
			       other.LessonName == LessonName;
		}

		public override int GetHashCode()
		{
			return LessonName != null ? LessonName.GetHashCode() : 0;
		}
	}
}
