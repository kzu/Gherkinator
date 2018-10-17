using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Gherkinator
{
    public class AdditionalTextContent : AdditionalText
    {
        string content;

        public AdditionalTextContent(string path, string content)
        {
            Path = path;
            this.content = content;
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
            => SourceText.From(content);
    }
}
