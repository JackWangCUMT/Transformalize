﻿/*
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

namespace Transformalize.Data
{
    public class SqlServerEntityExists : IEntityExists {

        private const string FORMAT = @"
IF EXISTS(
	SELECT *
	FROM INFORMATION_SCHEMA.TABLES 
	WHERE TABLE_SCHEMA = '{0}' 
	AND  TABLE_NAME = '{1}'
)	SELECT 1
ELSE
	SELECT 0;";

        public bool OutputExists(Entity entity) {
            using (var cn = new SqlConnection(entity.OutputConnection.ConnectionString)) {
                cn.Open();
                var sql = string.Format(FORMAT, entity.Schema, entity.OutputName());
                var cmd = new SqlCommand(sql, cn);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }

        public bool InputExists(Entity entity) {
            using (var cn = new SqlConnection(entity.InputConnection.ConnectionString)) {
                cn.Open();
                var sql = string.Format(FORMAT, entity.Schema, entity.Name);
                var cmd = new SqlCommand(sql, cn);
                return (int)cmd.ExecuteScalar() == 1;
            }
        }
    }
}