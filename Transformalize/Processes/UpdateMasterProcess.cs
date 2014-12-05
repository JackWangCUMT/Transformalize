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

using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {

    public class UpdateMasterProcess : EtlProcess {
        private readonly Process _process;

        public UpdateMasterProcess(Process process)
            : base(process) {
            _process = process;
        }

        protected override void Initialize() {
            foreach (var entity in _process.Entities) {
                Register(new EntityUpdateMaster(_process, entity));
            }
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        TflLogger.Error(this.Process.Name, string.Empty, e.Message);
                        TflLogger.Debug(this.Process.Name, string.Empty, e.StackTrace);
                    }
                }
                throw new TransformalizeException(this.Process.Name, string.Empty, "Update Master Process failed for {0}", Process.Name);
            }

            base.PostProcessing();
        }
    }
}