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
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public abstract class BaseTransform : ITransform {

        private const StringComparison Sc = StringComparison.OrdinalIgnoreCase;
        private Field _singleInput;
        private string _received;
        private readonly HashSet<string> _errors = new HashSet<string>();
        private readonly HashSet<string> _warnings = new HashSet<string>();

        public IContext Context { get; }

        // this **must** be implemented
        public abstract IRow Transform(IRow row);

        // this *may* be implemented
        public virtual IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows.Select(Transform);
        }



        public string Returns
        {
            get { return Context.Transform.Returns; }
            set { Context.Transform.Returns = value; }
        }

        public void Error(string error) {
            _errors.Add(error);
        }

        public void Warn(string warning) {
            _warnings.Add(warning);
        }

        public IEnumerable<string> Errors() {
            return _errors;
        }

        public IEnumerable<string> Warnings() {
            return _warnings;
        }

        protected BaseTransform(IContext context, string returns) {
            Context = context;
            Returns = returns;
        }

        public uint RowCount { get; set; }

        protected virtual void Increment() {
            RowCount++;
            if (RowCount % Context.Entity.LogInterval == 0) {
                Context.Info(RowCount.ToString());
            }
        }

        /// <summary>
        /// A transformer's input can be entity fields, process fields, or the field the transform is in.
        /// </summary>
        /// <returns></returns>
        List<Field> ParametersToFields() {
            return Context.Process.ParametersToFields(Context.Transform.Parameters, Context.Field);
        }

        public Field SingleInput() {
            return _singleInput ?? (_singleInput = ParametersToFields().First());
        }

        /// <summary>
        /// Only used with producers, see Transform.Producers()
        /// </summary>
        /// <returns></returns>
        public Field SingleInputForMultipleOutput() {
            if (Context.Transform.Parameter != string.Empty) {
                return Context.Entity == null
                    ? Context.Process.GetAllFields().First(f => f.Alias.Equals(Context.Transform.Parameter, Sc) || f.Name.Equals(Context.Transform.Parameter, Sc))
                    : Context.Entity.GetAllFields().First(f => f.Alias.Equals(Context.Transform.Parameter, Sc) || f.Name.Equals(Context.Transform.Parameter, Sc));
            }
            return Context.Field;
        }

        public Field[] MultipleInput() {
            return ParametersToFields().ToArray();
        }

        public Field[] MultipleOutput() {
            return ParametersToFields().ToArray();
        }

        public string Received() {
            if (_received != null)
                return _received;

            var index = Context.Field.Transforms.IndexOf(Context.Transform);
            if (index <= 0)
                return SingleInput().Type;

            var previous = Context.Field.Transforms[index - 1];

            _received = previous.Returns ?? _singleInput.Type;

            return _received;
        }

        public bool IsLast() {
            var count = Context.Field.Transforms.Count;
            if (count == 1)
                return true;
            var index = Context.Field.Transforms.IndexOf(Context.Transform);
            return index == count - 1;
        }

        public virtual void Dispose() {
        }
    }

}