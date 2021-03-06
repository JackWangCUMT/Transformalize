#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Provider.File;

namespace Transformalize.Ioc.Autofac.Modules {
    public class DirectoryModule : Module {
        private readonly Process _process;

        public DirectoryModule() { }

        public DirectoryModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // enitity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "directory")) {

                // no input version detector for now
                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(entity.Key);

                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    return new DirectoryReader(input, rowFactory);
                }).Named<IRead>(entity.Key);

                if (entity.Delete) {
                    builder.Register<IReadInputKeysAndHashCodes>((ctx) => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                        return new DirectoryReader(input, rowFactory);
                    }).Named<IReadInputKeysAndHashCodes>(entity.Key);
                }

            }

            if (_process.Output().Provider == "directory") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {
                    // todo
                }
            }
        }
    }
}