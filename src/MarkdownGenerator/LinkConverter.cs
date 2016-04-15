using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownGenerator
{
    public static class LinkConverter
    {
        public static string RewriteLinks(string content)
        {
            var rgx = new Regex(@"\[[\w\s]+\]\((\w+.md)\)", RegexOptions.IgnoreCase|RegexOptions.Multiline);
            var match = rgx.Match(content);
            if (match.Success)
            {
                var replace = rgx.Replace(content, m => m.Value.Replace(m.Groups[1].Value, m.Groups[1].Value.Replace(".md", ".html")));
                return replace;
            }
            return content;
        }
    }
}
