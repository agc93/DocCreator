using System;
using System.IO;
using MarkdownGenerator;

namespace DocCreator
{
    static class Extensions
    {
        internal static TemplateModel ToModel(this ApplicationArgs args, string markdown = null)
        {
            return new TemplateModel
            {
                Theme = args.Theme,
                Markdown = markdown ?? string.Empty,
                Title = args.Title
            };
        }

        
    }
}