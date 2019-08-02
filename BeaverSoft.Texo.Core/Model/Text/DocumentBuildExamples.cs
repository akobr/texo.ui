namespace BeaverSoft.Texo.Core.Model.Text
{
    public static class DocumentBuildExamples
    {
        /// <example>
        /// Document document = new Document(
        ///     new Header("Main Title"));
        ///
        ///    document.AddChild(new Section(
        ///         new Header("Section Title"),
        ///         new Paragraph(
        ///             new Span(
        ///                 new PlainText("Lorem ipsum "),
        ///                 new Strong(
        ///                     new Span(
        ///                         new PlainText("dolor sit amet, "),
        ///                         new Strikethrough("consectetur"),
        ///                         new PlainText(" adipiscing "),
        ///                         new Italic("elit"),
        ///                         new PlainText(".")
        ///                     )
        ///                 )
        ///             )
        ///         )
        /// ));
        /// </example>
        public static IDocument PureBuilder()
        {
            Document document = new Document(
                new Header("Main Title"));

            document.AddChild(new Section(
                new Header("Section Title"),
                new Paragraph(
                    new Span(
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
                )
            ));

            return document;
        }

        /// <example>
        /// ISpanBuilder inlineBuilder = new SpanBuilder();
        /// inlineBuilder.Write("Lorem ipsum ");
        /// 
        /// using (inlineBuilder.StartStrongContext())
        /// {
        ///     inlineBuilder
        ///         .Write("dolor sit amet, ")
        ///         .Strikethrough("consectetur")
        ///         .Write(" adipiscing ")
        ///         .Italic("elit")
        ///         .Write(".");
        /// }
        /// 
        /// return inlineBuilder.Write(" Duis felis tortor, sodales quis tincidunt id, convallis eget nisl.");
        /// </example>
        public static ISpanBuilder WithControlledContext()
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

            return inlineBuilder.Write(" Duis felis tortor, sodales quis tincidunt id, convallis eget nisl.");
        }

        /// <example>
        /// SpanBuilder
        ///     .Create()
        ///     .Write("Lorem ipsum ")
        ///     .StartStrongContext()
        ///     .Write("dolor sit amet, ")
        ///     .Strikethrough("consectetur")
        ///     .Write(" adipiscing ")
        ///     .Italic("elit")
        ///     .EndContext()
        ///     .Write(".");
        /// </example>
        public static ISpanBuilder FluentWay()
        {
            return SpanBuilder
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