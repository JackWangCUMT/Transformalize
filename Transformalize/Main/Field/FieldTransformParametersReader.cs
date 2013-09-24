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

using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main
{
    public class FieldTransformParametersReader : ITransformParametersReader
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Parameters Read(TransformConfigurationElement transform)
        {
            var parameters = new Parameters();

            if (transform.Parameter != string.Empty && transform.Parameter != "*")
            {
                transform.Parameters.Insert(new ParameterConfigurationElement
                                                {
                                                    Field = transform.Parameter
                                                });
            }

            foreach (ParameterConfigurationElement p in transform.Parameters)
            {
                if (string.IsNullOrEmpty(p.Name))
                {
                    _log.Warn("Detected a {0} transform parameter without a name attribute.  Field parameters require names and values.", transform.Method);
                    return new Parameters();
                }

                //if (string.IsNullOrEmpty(p.Value))
                //{
                //    _log.Warn("The field {0} has a {1} transform parameter without a value attribute.  Field parameters require names and values.", _name, transform.Method);
                //    return new Parameters();
                //}

                var value = p.HasValue() ? p.Value : null;
                var alias = p.HasValue() ? p.Name : p.Field;
                parameters.Add(alias, p.Name, value, p.Type);
            }

            return parameters;
        }
    }
}