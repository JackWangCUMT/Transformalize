using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Transform
{
    public class ToLocalTimeOperation : AbstractOperation {
        private readonly string _inKey;
        private readonly string _outKey;

        public ToLocalTimeOperation(string inKey, string outKey)
        {
            _inKey = inKey;
            _outKey = outKey;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                row[_outKey] = ((DateTime)row[_inKey]).ToLocalTime();
                yield return row;
            }

        }
    }
}