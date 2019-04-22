﻿namespace TestOkur.Optic.Form
{
	using System.Runtime.Serialization;

	[DataContract]
	public class StudentOrder
	{
		public StudentOrder(string name, int classroomOrder, int schoolOrder, int districtOrder, int generalOrder)
		{
			Name = name;
			ClassroomOrder = classroomOrder;
			SchoolOrder = schoolOrder;
			DistrictOrder = districtOrder;
			GeneralOrder = generalOrder;
		}

		[DataMember]
		public string Name { get; private set; }

		[DataMember]
		public int ClassroomOrder { get; private set; }

		[DataMember]
		public int SchoolOrder { get; private set; }

		[DataMember]
		public int DistrictOrder { get; private set; }

		[DataMember]
		public int GeneralOrder { get; private set; }
	}
}