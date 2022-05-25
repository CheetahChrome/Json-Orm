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


	}
}
