using BeaverSoft.Texo.Core.Model.Text;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class TextModelTests
    {
        [Fact]
        public void SimpleStrongAndItalicSpan()
        {
            Span builded = SpanBuilder
                .Create()
                .Write("Lorem ")
                .Strong("ipsum")
                .Write(" ")
                .Italic("dolor")
                .Write(" sit amet.")
                .Span;

            Assert.Equal("Lorem **ipsum** *dolor* sit amet.", builded.ToString());
        }

        [Fact]
        public void ComplexMarkedInsertedStrongCodeSpan()
        {
            ISpanBuilder builder = new SpanBuilder().Write("Pellentesque ");

            using (var marked = builder.StartMarkedContext())
            {
                using (var inserted = marked.StartInsertedContext())
                {
                    inserted
                        .Write("habitant ")
                        .StartStrongContext()
                        .Write("morbi tristique")
                        .EndContext()
                        .Write(" senectus");
                }

                marked.Write(" et ");
                marked.Code("netus et malesuada");
            }

            builder.Write(" fames ac turpis egestas.");

            Assert.Equal("Pellentesque ==++habitant **morbi tristique** senectus++ et `netus et malesuada`== fames ac turpis egestas.", builder.Span.ToString());
        }

        [Fact]
        public void SimpleDocument()
        {
            Document document = new Document(
                new Header("Main Title"),
                new Section(
                    new Header(2, "Section Title"),
                    new Paragraph(
                        new Span(
                            new PlainText("Lorem ipsum "),
                            new Strong(
                                new Span(
                                    new PlainText("dolor sit amet, "),
                                    new Strikethrough("consectetur"),
                                    new PlainText(" adipiscing "),
                                    new Italic("elit")
                                )
                            ),
                            new PlainText(".")
                        )
                    )
                )
            );

            Assert.Equal("X# Main Title\r\n\r\n## Section Title\r\n\r\nLorem ipsum **dolor sit amet, ~~consectetur~~ adipiscing *elit***.\r\n", document.ToString());
        }
    }
}
