#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }

        public SqlServerEntityRecordsExist() {
            EntityExists = new SqlServerEntityExists();
        }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            if (EntityExists.Exists(connection, entity)) {

                using (var cn = connection.GetConnection()) {
                    cn.Open();
                    var sql = $@"SELECT TOP(1) [{entity.PrimaryKey.First().Alias}] FROM [{entity.OutputName()}];";
                    var cmd = cn.CreateCommand();
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader()) {
                        return reader.Read();
                    }
                }
            }
            return false;
        }
    }
}