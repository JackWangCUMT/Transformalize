using Transformalize.Libs.NanoXml;

namespace Transformalize.Test {
    public class TflTemplate : TflNode {
        public TflTemplate(NanoXmlNode node)
            : base(node) {
            Key("name");

            Property("content-type", "raw");
            Property("file", string.Empty, true, true);
            Property("cache", false);
            Property("enabled", true);
            Property("conditional", false);
            Property("engine", "razor");

            Class<TflParameter>("parameters");
            Class<TflAction>("actions");
        }
    }
}