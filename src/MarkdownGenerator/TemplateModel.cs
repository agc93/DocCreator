using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownGenerator
{
    public class TemplateModel
    {
        public string Title { get; set; }
        public string Markdown { get; set; }
        public Theme Theme { get; set; }
    }
}
