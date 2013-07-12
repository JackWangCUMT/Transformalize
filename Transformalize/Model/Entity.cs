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

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class Entity : WithLoggingMixin, IDisposable {

        public string Schema { get; set; }
        public string ProcessName { get; set; }
        public string Name { get; set; }
        public Connection InputConnection { get; set; }
        public Connection OutputConnection { get; set; }
        public Field Version;
        public Dictionary<string, Field> PrimaryKey { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
        public Dictionary<string, Field> Xml { get; set; }
        public Dictionary<string, Field> All { get; set; }
        public Dictionary<string, Relationship> Joins { get; set; }
        public long RecordsAffected { get; set; }
        public object Begin { get; set; }
        public object End { get; set; }
        public int TflBatchId { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }

        public IEnumerable<Relationship> RelationshipToMaster { get; set; }

        public Entity() {
            Name = string.Empty;
            Schema = string.Empty;
            PrimaryKey = new Dictionary<string, Field>();
            Fields = new Dictionary<string, Field>();
            All = new Dictionary<string, Field>();
            Joins = new Dictionary<string, Relationship>();
        }

        public string FirstKey() {
            return PrimaryKey.First().Key;
        }

        public bool IsMaster() {
            return PrimaryKey.Any(kv => kv.Value.FieldType == FieldType.MasterKey);
        }

        public void Dispose() {
            foreach (var pair in All) {
                if (pair.Value.Transforms == null) continue;
                foreach (var t in pair.Value.Transforms) {
                    t.Dispose();
                }
            }
        }

        public string OutputName() {
            return string.Concat(ProcessName, Name);
        }
        
        public bool HasForeignKeys() {
            return Fields.Any(f => f.Value.FieldType.Equals(FieldType.ForeignKey));
        }
    }
}