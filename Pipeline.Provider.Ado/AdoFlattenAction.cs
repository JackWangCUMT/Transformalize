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
using System.Text;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {

    public class AdoFlattenAction : IAction {

        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoFlattenAction(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public ActionResponse Execute() {
            if (_output.Process.Entities.Sum(e => e.Inserts + e.Updates + e.Deletes) == 0) {
                return new ActionResponse(200, "nothing to flatten");
            }

            var model = new AdoSqlModel(_output, _cf);

            if (model.MasterEntity.Inserts > 0) {

                if (_output.Process.IsFirstRun()) {
                    return new AdoFlattenFirstRunAction(_output, _cf, model).Execute();
                }

                if (_cf.AdoProvider == AdoProvider.SqlCe) {
                    var insertAction = new AdoFlattenInsertBySelectAction(_output, _cf, model);
                    var insertResponse = insertAction.Execute();
                    insertResponse.Action = new Action {
                        Type = "internal",
                        Description = "Flatten Action",
                        ErrorMode = "abort"
                    };
                    if (insertResponse.Code != 200) {
                        return insertResponse;
                    }
                } else {
                    var insertAction = new AdoFlattenInsertByViewAction(_output, _cf, model);
                    var insertResponse = insertAction.Execute();
                    insertResponse.Action = new Action {
                        Type = "internal",
                        Description = "Flatten Action",
                        ErrorMode = "abort"
                    };
                    if (insertResponse.Code != 200) {
                        return insertResponse;
                    }
                }

            }

            switch (_cf.AdoProvider) {
                case AdoProvider.SqlCe:
                    // this provider does NOT support views, and does NOT support UPDATE ... SET ... FROM ... syntax

                    var masterAlias = Utility.GetExcelName(model.MasterEntity.Index);
                    var builder = new StringBuilder();
                    builder.AppendLine($"SELECT {_output.SqlStarFields(_cf)}");

                    foreach (var from in _output.SqlStarFroms(_cf)) {
                        builder.AppendLine(@from);
                    }

                    builder.AppendFormat("INNER JOIN {0} flat ON (flat.{1} = {2}.{3})", _cf.Enclose(_output.Process.Flat), model.EnclosedKeyLongName, masterAlias, model.EnclosedKeyShortName);

                    builder.AppendLine($" WHERE {masterAlias}.{model.Batch} > @Threshold; ");

                    return new AdoFlattenTwoPartUpdateAction(_output, _cf, model, builder.ToString()).Execute();
                case AdoProvider.SqLite:

                    // this provider supports views, but does NOT support UPDATE ... SET ... FROM ... syntax

                    var sql = $@"
                        SELECT s.{string.Join(",s.", model.Aliases)}
                        FROM {model.Master} m
                        INNER JOIN {model.Flat} f ON (f.{model.EnclosedKeyLongName} = m.{model.EnclosedKeyShortName})
                        INNER JOIN {model.Star} s ON (s.{model.EnclosedKeyLongName} = m.{model.EnclosedKeyShortName})
                        WHERE m.{model.Batch} > @Threshold;";
                    return new AdoFlattenTwoPartUpdateAction(_output, _cf, model, sql).Execute();
                default:
                    // these providers support views and UPDATE ... SET ... FROM ... syntax (server based)
                    return new AdoFlattenUpdateByViewAction(_output, _cf, model).Execute();
            }
        }

    }
}