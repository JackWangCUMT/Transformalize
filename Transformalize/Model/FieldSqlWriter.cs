using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Model {


    public class FieldSqlWriter {

        const string ROW_VERSION_KEY = "RowVersion";
        const string TIME_KEY_KEY = "TimeKey";
        const string LOAD_DATE_KEY = "LoadDate";
        private SortedDictionary<string, string> _output;
        private Dictionary<string, IField> _original;

        public FieldSqlWriter(IField field) {
            StartWithField(field);
        }

        public FieldSqlWriter Reload(IField field) {
            StartWithField(field);
            return this;
        }

        public FieldSqlWriter(IEnumerable<IField> fields) {
            StartWithFields(fields);
        }

        public FieldSqlWriter Reload(IEnumerable<IField> fields) {
            StartWithFields(fields);
            return this;
        }

        public FieldSqlWriter(IDictionary<string, IField> fields) {
            StartWithDictionary(fields);
        }

        public FieldSqlWriter Reload(IDictionary<string, IField> fields) {
            StartWithDictionary(fields);
            return this;
        }

        public FieldSqlWriter(params IDictionary<string, IField>[] fields) {
            StartWithDictionaries(fields);
        }

        public FieldSqlWriter Reload(params IDictionary<string, IField>[] fields) {
            if (fields.Length == 0)
                StartWithDictionary(_original);
            else
                StartWithDictionaries(fields);
            return this;
        }

        private void StartWithField(IField field) {
            _original = new Dictionary<string, IField> { { field.Alias, field } };
            _output = new SortedDictionary<string, string> { { field.Alias, string.Empty } };
        }

        private void StartWithFields(IEnumerable<IField> fields) {
            var expanded = fields.ToArray();
            _original = expanded.ToArray().ToDictionary(f => f.Alias, f => f);
            _output = new SortedDictionary<string, string>(expanded.ToDictionary(f => f.Alias, f => string.Empty));
        }

        private void StartWithDictionary(IDictionary<string, IField> fields) {
            _original = fields.ToDictionary(f => f.Key, f => f.Value);
            _output = new SortedDictionary<string, string>(fields.ToDictionary(f => f.Key, f => string.Empty));
        }

        private void StartWithDictionaries(params IDictionary<string, IField>[] fields) {
            _original = new Dictionary<string, IField>();
            foreach (var dict in fields) {
                foreach (var key in dict.Keys) {
                    _original[key] = dict[key];
                }
            }
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        public string Write(string separator = ", ", bool flush = true) {
            var result = string.Join(separator, _output.Select(kv => kv.Value));
            if (flush)
                Flush();
            return result;
        }

        public IEnumerable<string> Values(bool flush = true) {
            return _output.Select(kv => kv.Value);
        }

        private void Flush() {
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        private IEnumerable<string> CopyOutputKeys() {
            var keys = new string[_output.Keys.Count];
            _output.Keys.CopyTo(keys, 0);
            return keys;
        }

        private static string SafeColumn(string name) {
            return string.Concat("[", name, "]");
        }

        public FieldSqlWriter Name() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = SafeColumn(_original[key].Name);
            }
            return this;
        }

        public FieldSqlWriter Alias() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = SafeColumn(_original[key].Alias);
            }
            return this;
        }

        public FieldSqlWriter IsNull() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                _output[key] = string.Concat("ISNULL(", _output[key], ", ", field.Quote, field.Default, field.Quote, ")");
            }
            return this;
        }

        public FieldSqlWriter ToAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType == Model.FieldType.Xml) {
                    _output[key] = string.Concat("[", field.Alias, "] = ", _output[key]);
                }
            }
            return this;
        }

        public FieldSqlWriter AsAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType == Model.FieldType.Xml) {
                    _output[key] = string.Concat(_output[key], " AS [", _original[key].Alias, "]");
                }
            }
            return this;
        }

        public FieldSqlWriter DataType() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(_output[key], " ", _original[key].SqlDataType);
            }
            return this;
        }

        public FieldSqlWriter Prepend(string prefix) {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(prefix, _output[key]);
            }
            return this;
        }

        private void Append(string suffix) {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(_output[key], suffix);
            }
        }

        public FieldSqlWriter Null() {
            Append(" NULL");
            return this;
        }

        public FieldSqlWriter NotNull() {
            Append(" NOT NULL");
            return this;
        }

        public FieldSqlWriter Asc() {
            Append(" ASC");
            return this;
        }

        public FieldSqlWriter Desc() {
            Append(" DESC");
            return this;
        }

        public FieldSqlWriter AppendIf(string suffix, FieldType fieldType) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType == fieldType)
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        public FieldSqlWriter AppendIfNot(string suffix, FieldType fieldType) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType != fieldType)
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        private static string XmlValue(IField field) {
            return string.Format("[{0}].value('({1})[{2}]', '{3}')", field.Parent, field.XPath, field.Index, field.SqlDataType);
        }

        public FieldSqlWriter XmlValue() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = XmlValue(_original[key]);
            }
            return this;
        }

        /// <summary>
        /// Presents the field for a select. 
        /// </summary>
        /// <returns>field's name for a regular field, and the XPath necessary for XML based fields.</returns>
        public FieldSqlWriter Select() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType == Model.FieldType.Xml)
                    _output[key] = XmlValue(field);
                else {
                    _output[key] = SafeColumn(field.Name);
                }
            }
            return this;
        }

        public FieldSqlWriter Output(bool answer = true) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Output != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Input(bool answer = true) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Input != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter HasReference(bool answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.HasReference() != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Xml(bool answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.InnerXml.Any() != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter FieldType(FieldType answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter FieldType(params FieldType[] answers) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (!answers.Any(a => a == field.FieldType))
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Set(string left, string right) {
            foreach (var key in CopyOutputKeys()) {
                var name = _output[key];
                _output[key] = string.Concat(left, ".", name, " = ", right, ".", name);
            }
            return this;
        }

        public FieldSqlWriter ExpandXml() {
            foreach (var key in CopyOutputKeys()) {

                var field = _original[key];
                if (!field.InnerXml.Any()) continue;

                foreach (var xml in field.InnerXml) {
                    _original[xml.Key] = xml.Value;
                    _output[xml.Key] = string.Empty;
                }

                _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter AddSystemFields() {
            var fields = new List<IField> {
                new Field() {Alias = ROW_VERSION_KEY, Type = "System.RowVersion", Output = true},
                new Field() {Alias = TIME_KEY_KEY, Type = "System.Int32", Output = true},
                new Field() {Alias = LOAD_DATE_KEY, Type = "System.DateTime", Output = true}
            };
            foreach (var field in fields) {
                _original[field.Alias] = field;
                _output[field.Alias] = string.Empty;
            }
            return this;
        }

        public override string ToString() {
            return Write();
        }

        public Dictionary<string, IField> Context() {
            var results = new Dictionary<string, IField>();
            foreach (var key in _output.Keys) {
                results[key] = _original[key];
            }
            return results;
        }

    }
}