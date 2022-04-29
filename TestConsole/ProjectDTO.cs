using Json.Orm;
using Json.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
	public class ProjectDTO : JsonOrmModel
	{
		public override string GetStoredProcedureName => "[get].[Project]";

		[TableType(1)]
		public int ProjectId { get; set; }

		[TableType(2)]
		public string ProjectIdentity { get; set; }

		[TableType(3)]
		public string Title { get; set; }

		[TableType(4)]
		public bool ClassGS { get; set; }

		[TableType(5)]
		public bool ClassSC { get; set; }

		[TableType(6)]
		public bool ClassSS { get; set; }

		[TableType(7)]
		public bool ClassUnknown { get; set; }

		[TableType(8)]
		public bool Gt150k { get; set; }

		[TableType(9)]
		public bool MaintFunds { get; set; }

		[TableType(10)]
		public bool Na50Funded { get; set; }

		[TableType(11)]
		public bool DimitryList { get; set; }

		[TableType(12)]
		public int FundsAuth { get; set; }

		[TableType(13)]
		public int Tpc { get; set; }

		[TableType(14)]
		public int ComplexityOfProject { get; set; }

		[TableType(15)]
		public int ConsequenceOfFailure { get; set; }

		[TableType(16)]
		public int DeconflictingSchedule { get; set; }

		[TableType(17)]
		public int DifficultyScore { get; set; }

		[TableType(18)]
		public int Location { get; set; }

		[TableType(19)]
		public int ProbabilityOfFailure { get; set; }

		[TableType(20)]
		public int ProcurementComplexity { get; set; }

		[TableType(21)]
		public int Resources { get; set; }

		[TableType(22)]
		public int RiskScore { get; set; }

		[TableType(23)]
		public string IdeaDate { get; set; }

		[TableType(24)]
		public string CDPProjectNumber { get; set; }

		[TableType(25)]
		public string G2Code { get; set; }

		[TableType(26)]
		public string Advocate { get; set; }

		[TableType(27)]
		public string ConstMgr { get; set; }

		[TableType(28)]
		public string DiscoveryLead { get; set; }

		[TableType(29)]
		public string FrdLead { get; set; }

		[TableType(30)]
		public string IdeaRequestor { get; set; }

		[TableType(31)]
		public string ExecutionEnd { get; set; }

		[TableType(32)]
		public string ExecutionGroup { get; set; }

		[TableType(33)]
		public string ExecutionStart { get; set; }

		[TableType(34)]
		public string Fynsp { get; set; }

		[TableType(35)]
		public string HoldPhase { get; set; }

		[TableType(36)]
		public string LowRigorAuthorization { get; set; }

		[TableType(37)]
		public string LowRigorReview { get; set; }

		[TableType(38)]
		public string MissionRisk { get; set; }

		[TableType(39)]
		public string PhaseGateReqd { get; set; }

		[TableType(40)]
		public string PlanningEngineer { get; set; }

		[TableType(41)]
		public string PlanningResponsibility { get; set; }

		[TableType(42)]
		public string PointOfContact { get; set; }

		[TableType(43)]
		public string PreplanSupport { get; set; }

		[TableType(44)]
		public string Priority { get; set; }

		[TableType(45)]
		public string ProcurementStrategist { get; set; }

		[TableType(46)]
		public string ProjectEngineer { get; set; }

		[TableType(47)]
		public string ProjectFiles { get; set; }

		[TableType(48)]
		public string ProjectSchedule { get; set; }

		[TableType(49)]
		public string Rev { get; set; }

		[TableType(50)]
		public string WbsProject { get; set; }

		[TableType(51)]
		public string WbsUpload { get; set; }

	}
}
