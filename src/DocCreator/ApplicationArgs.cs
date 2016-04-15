using MarkdownGenerator;

namespace DocCreator
{
    class ApplicationArgs
    {
        public string OutputDirectory { get; set; }
        public string InputFile { get; set; }
        public Theme Theme { get; set; }
        public string Title { get; set; }
        public bool RewriteLinks { get; internal set; }
    }
}
