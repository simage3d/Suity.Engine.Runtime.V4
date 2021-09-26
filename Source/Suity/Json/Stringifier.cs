using System.Text;

namespace ComputerBeacon.Json
{
    class Stringifier {

        const string newline = "\r\n";
        const string indent = "\t";

        public static void stringify(JsonObject jo, StringBuilder sb, int depth, bool niceFormat) {
            sb.Append('{');

            ++depth;

            bool firstValue = false;
            foreach (var kvp in jo) {
                if (firstValue) sb.Append(',');
                if(niceFormat) appendIndent(sb, depth);

                writeEscapedString(sb, kvp.Key);
                sb.Append(':');
                if (kvp.Value is JsonObject) stringify(kvp.Value as JsonObject, sb, depth + 1, niceFormat);
                else if (kvp.Value is JsonArray) stringify(kvp.Value as JsonArray, sb, depth + 1, niceFormat);
                else writeValue(sb, kvp.Value);

                firstValue = true;
            }

            if (niceFormat) appendIndent(sb, --depth);
            sb.Append('}');
        }

        public static void stringify(JsonArray ja, StringBuilder sb, int depth, bool niceFormat) {
            sb.Append('[');

            ++depth;

            for (int i = 0; i < ja.Count; i++) {
                if (i > 0) sb.Append(',');

                if (niceFormat) appendIndent(sb, depth);

                if (ja[i] is JsonObject) stringify(ja[i] as JsonObject, sb, depth + 1, niceFormat);
                else if (ja[i] is JsonArray) stringify(ja[i] as JsonArray, sb, depth + 1, niceFormat);
                else writeValue(sb, ja[i]);
            }

            if (niceFormat) appendIndent(sb, --depth);
            sb.Append(']');
        }

        static void appendIndent(StringBuilder sb, int depth) {
            sb.Append(newline);
            while (depth --> 0) sb.Append(indent);
        }

        public static void writeEscapedString(StringBuilder sb, string s) {
            sb.Append('"');
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                switch (c) {
                    case '"':
                        sb.Append("\\\"");
                        continue;
                    case '\\':
                        sb.Append("\\\\");
                        continue;
                    case '\n':
                        sb.Append("\\n");
                        continue;
                    case '\r':
                        sb.Append("\\r");
                        continue;
                    case '\t':
                        sb.Append("\\t");
                        continue;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }

        static void writeValue(StringBuilder sb, object o) {
            if (o == null) sb.Append("null");
            else if (o is string) writeEscapedString(sb, o as string);
            else if (o is bool) sb.Append((bool)o ? "true" : "false");
            else sb.Append(o.ToString());
        }
    }
}
