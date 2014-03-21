using System.Configuration;

namespace Transformalize.Configuration {
    public class IoElementCollection : ConfigurationElementCollection {

        public IoConfigurationElement this[int index] {
            get { return BaseGet(index) as IoConfigurationElement; }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public override bool IsReadOnly() {
            return false;
        }

        protected override ConfigurationElement CreateNewElement() {
            return new IoConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var io = ((IoConfigurationElement)element);
            return string.Concat(io.Name, io.Connection);
        }

        public void Add(IoConfigurationElement output) {
            BaseAdd(output);
        }
    }
}