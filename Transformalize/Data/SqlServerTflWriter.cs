/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class SqlServerTflWriter : WithLoggingMixin, ITflWriter {

        private readonly Process _process;

        public SqlServerTflWriter(ref Process process) {
            _process = process;
        }

        private static void Execute(string sql, string connectionString) {
            using (var cn = new SqlConnection(connectionString)) {
                cn.Open();
                var command = new SqlCommand(sql, cn);
                command.ExecuteNonQuery();
            }
        }

        public string CreateSql() {
            return @"
                CREATE TABLE TflBatch(
                    TflBatchId INT NOT NULL,
					ProcessName NVARCHAR(100) NOT NULL,
	                EntityName NVARCHAR(100) NOT NULL,
	                BinaryVersion BINARY(8) NULL,
	                DateTimeVersion DATETIME NULL,
                    Int64Version BIGINT NULL,
                    Int32Version INT NULL,
                    Int16Version SMALLINT NULL,
                    ByteVersion TINYINT NULL,
	                LastProcessedDate DATETIME NOT NULL,
                    Rows BIGINT NOT NULL,
					CONSTRAINT Pk_TflBatch_TflBatchId PRIMARY KEY (
						TflBatchId
					)
                );

                CREATE INDEX Ix_TflBatch_ProcessName_EntityName__TflBatchId ON TflBatch (
                    ProcessName ASC,
                    EntityName ASC
                ) INCLUDE (TflBatchId);
            ";
        }

        public void Reset() {
            var sql = string.Format("DELETE FROM TflBatch WHERE ProcessName = '{0}';", _process.Name);
            Execute(sql, _process.MasterEntity.OutputConnection.ConnectionString);
        }

        public void Initialize() {
            var cs = _process.MasterEntity.OutputConnection.ConnectionString;

            Execute(SqlTemplates.TruncateTable("TflBatch"), cs);
            Info("{0} | Truncated TflBatch.", _process.Name);

            Execute(SqlTemplates.DropTable("TflBatch"), cs);
            Info("{0} | Dropped TflBatch.", _process.Name);

            Execute(CreateSql(), cs);
            Info("{0} | Created TflBatch.", _process.Name);
        }

    }
}
