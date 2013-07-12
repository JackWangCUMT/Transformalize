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

using Transformalize.Data;

namespace Transformalize.Model {
    public class Connection {
        private readonly IConnectionChecker _connectionChecker;

        public Connection(IConnectionChecker connectionChecker) {
            _connectionChecker = connectionChecker;
        }

        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int Year { get; set; }
        public int OutputBatchSize { get; set; }
        public int InputBatchSize { get; set; }
        public bool IsReady() {
            return _connectionChecker.Check(ConnectionString);
        }
    }
}