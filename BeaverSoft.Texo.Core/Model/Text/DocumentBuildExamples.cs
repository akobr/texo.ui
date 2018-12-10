namespace BeaverSoft.Texo.Core.Model.Text
{
    public class DocumentBuildExamples
    {
        public void PureBuilder()
        {
            Document document = new Document(
                new Header("Main Title"));

            document.AddChild(new Section(
                new Header("Section Title"),
                new Paragraph(
                    new PlainText("Lorem ipsum "),
                    new Strong(
                        new Span(
                            new PlainText("dolor sit amet, "),
                            new Strikethrough("consectetur"),
                            new PlainText(" adipiscing "),
                            new Italic("elit"),
                            new PlainText(".")
                        )
                    )
                )
            ));
        }

        public void WithControlledContext()
        {
            ISpanBuilder inlineBuilder = new SpanBuilder();

            inlineBuilder.Write("Lorem ipsum ");

            using (inlineBuilder.StartStrongContext())
            {
                inlineBuilder
                    .Write("dolor sit amet, ")
                    .Strikethrough("consectetur")
                    .Write(" adipiscing ")
                    .Italic("elit")
                    .Write(".");
            }

            inlineBuilder.Write(" Duis felis tortor, sodales quis tincidunt id, convallis eget nisl.");
        }

        public void FluentWay()
        {
            SpanBuilder
                .Create()
                .Write("Lorem ipsum ")
                .StartStrongContext()
                .Write("dolor sit amet, ")
                .Strikethrough("consectetur")
                .Write(" adipiscing ")
                .Italic("elit")
                .EndContext()
                .Write(".");
        }
    }
}