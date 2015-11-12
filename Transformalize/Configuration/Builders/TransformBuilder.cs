using System;
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Configuration.Builders {

    public class TransformBuilder {

        private readonly FieldBuilder _fieldBuilder;
        private readonly BranchBuilder _branchBuilder;
        private readonly TflTransform _transform;

        public TransformBuilder(FieldBuilder fieldBuilder, TflTransform transform) {
            _fieldBuilder = fieldBuilder;
            _transform = transform;
        }

        public TransformBuilder(BranchBuilder branchBuilder, TflTransform transform) {
            _branchBuilder = branchBuilder;
            _transform = transform;
        }

        public FieldBuilder Field(string name) {

            return _fieldBuilder == null ? _branchBuilder.Field(name) : _fieldBuilder.Field(name);
        }

        public TransformBuilder Method(string method) {
            _transform.Method = method;
            return this;
        }

        public TransformBuilder XPath(string xPath) {
            _transform.Xpath = xPath;
            return this;
        }

        public TransformBuilder Units(string units) {
            _transform.Units = units;
            return this;
        }

        public TransformBuilder Value(string value) {
            _transform.Value = value;
            return this;
        }

        public TransformBuilder Pattern(string pattern) {
            _transform.Pattern = pattern;
            return this;
        }

        public TransformBuilder Replacement(string value) {
            _transform.Replacement = value;
            return this;
        }

        public TransformBuilder OldValue(string value) {
            _transform.OldValue = value;
            return this;
        }

        public TransformBuilder NewValue(string value) {
            _transform.NewValue = value;
            return this;
        }

        public TransformBuilder TrimChars(string trimChars) {
            _transform.TrimChars = trimChars;
            return this;
        }

        public TransformBuilder Index(int index) {
            _transform.Index = index;
            return this;
        }

        public TransformBuilder Interval(int interval) {
            _transform.Interval = interval;
            return this;
        }

        public TransformBuilder Count(int count) {
            _transform.Count = count;
            return this;
        }

        public TransformBuilder StartIndex(int startIndex) {
            _transform.StartIndex = startIndex;
            return this;
        }

        public TransformBuilder Length(int length) {
            _transform.Length = length;
            return this;
        }

        public TransformBuilder Elipse(string elipse) {
            _transform.Elipse = elipse;
            return this;
        }

        public TransformBuilder TotalWidth(int totalWidth) {
            _transform.TotalWidth = totalWidth;
            return this;
        }

        public TransformBuilder PaddingChar(char paddingChar) {
            _transform.PaddingChar = paddingChar;
            return this;
        }

        public TransformBuilder Map(string map) {
            _transform.Map = map;
            return this;
        }

        public TransformBuilder Root(string root) {
            _transform.Root = root;
            return this;
        }

        public TransformBuilder Script(string script) {
            _transform.Script = script;
            return this;
        }

        public TransformBuilder Template(string template) {
            _transform.Template = template;
            return this;
        }

        public TransformBuilder Format(string format) {
            _transform.Format = format;
            return this;
        }

        public TransformBuilder Encoding(string encoding) {
            _transform.Encoding = encoding;
            return this;
        }

        public TransformBuilder Separator(string separator) {
            _transform.Separator = separator;
            return this;
        }

        public TransformBuilder FromTimeZone(string timeZone) {
            _transform.FromTimeZone = timeZone;
            return this;
        }

        public TransformBuilder ToTimeZone(string timeZone) {
            _transform.ToTimeZone = timeZone;
            return this;
        }

        public TransformBuilder Model(string model) {
            _transform.Model = model;
            return this;
        }

        public TransformBuilder Type(string type) {
            _transform.Type = type;
            return this;
        }

        public TransformBuilder Characters(string characters) {
            _transform.Characters = characters;
            return this;
        }

        public TransformBuilder ContainsCharacters(string containsCharacters) {
            _transform.ContainsCharacters = containsCharacters;
            return this;
        }

        public TransformBuilder Negated(bool negated) {
            _transform.Negated = negated;
            return this;
        }

        public TransformBuilder IgnoreEmpty(bool ignore = true) {
            _transform.IgnoreEmpty = ignore;
            return this;
        }

        public TransformBuilder LowerBound(object lowerBound) {
            _transform.LowerBound = lowerBound;
            return this;
        }

        public TransformBuilder LowerBoundType(string lowerBoundType) {
            _transform.LowerBoundType = lowerBoundType;
            return this;
        }

        public TransformBuilder LowerUnit(string lowerUnit) {
            _transform.LowerUnit = lowerUnit;
            return this;
        }

        public TransformBuilder UpperBound(object upperBound) {
            _transform.UpperBound = upperBound;
            return this;
        }

        public TransformBuilder UpperBoundType(string upperBoundType) {
            _transform.UpperBoundType = upperBoundType;
            return this;
        }

        public TransformBuilder UpperUnit(string upperUnit) {
            _transform.UpperUnit = upperUnit;
            return this;
        }

        public TransformBuilder Domain(string domain) {
            _transform.Domain = domain;
            return this;
        }

        public TransformBuilder Left(string name) {
            _transform.Left = name;
            return this;
        }

        public TransformBuilder Operator(string comparisonOperator) {
            _transform.Operator = comparisonOperator;
            return this;
        }

        public TransformBuilder Right(string fieldOrValue) {
            _transform.Right = fieldOrValue;
            return this;
        }

        public TransformBuilder Then(string fieldOrValue) {
            _transform.Then = fieldOrValue;
            return this;
        }

        public TransformBuilder Else(string fieldOrValue) {
            _transform.Else = fieldOrValue;
            return this;
        }

        public TransformBuilder FromLat(string coordinate) {
            _transform.FromLat = coordinate;
            return this;
        }

        public TransformBuilder FromLong(string coordinate) {
            _transform.FromLong = coordinate;
            return this;
        }

        public TransformBuilder ToLat(string coordinate) {
            _transform.ToLat = coordinate;
            return this;
        }

        public TransformBuilder ToLong(string coordinate) {
            _transform.ToLong = coordinate;
            return this;
        }

        public TflProcess Process() {
            return _fieldBuilder == null ? _branchBuilder.Process() : _fieldBuilder.Process();
        }

        public TransformBuilder Transform(string method = "") {
            return _fieldBuilder == null ? _branchBuilder.Transform(method) : _fieldBuilder.Transform(method);
        }

        public EntityBuilder Entity(string name) {
            return _fieldBuilder == null ? _branchBuilder.Entity(name) : _fieldBuilder.Entity(name);
        }

        public FieldBuilder CalculatedField(string name) {
            return _fieldBuilder == null ? _branchBuilder.CalculatedField(name) : _fieldBuilder.CalculatedField(name);
        }

        public TransformBuilder ToString(string format) {
            _transform.Method = "toString";
            _transform.Format = format;
            return this;
        }

        public TransformBuilder Parameter(object value, string type) {
            var parameter = new TflParameter { Type = type, Value = value.ToString()}.WithDefaults();
            _transform.Parameters.Add(parameter);
            return this;
        }

        public TransformBuilder Parameter(string entity, string field) {
            return InternalParameter(entity, field);
        }

        public TransformBuilder Parameter(string field) {
            return InternalParameter(string.Empty, field);
        }

        private TransformBuilder InternalParameter(string entity, string field) {
            if (field.Equals("*")) {
                _transform.Parameter = field;
            } else {
                var parameter = new TflParameter{
                    Entity = entity,
                    Field = field
                }.WithDefaults();
                _transform.Parameters.Add(parameter);
            }
            return this;
        }

        public BranchBuilder Branch(string name) {
            if (_fieldBuilder == null) {
                return _branchBuilder.Branch(name);
            }
            var branch = new TflBranch { Name = name}.WithDefaults();
            _transform.Branches.Add(branch);
            return new BranchBuilder(this, branch);
        }

        public RelationshipBuilder Relationship() {
            return _fieldBuilder == null ? _branchBuilder.Relationship() : _fieldBuilder.Relationship();
        }

        public TransformBuilder ExternalScript(string name) {
            var script = new TflNameReference() { Name = name };
            _transform.Scripts.Add(script);
            return this;
        }
    }
}