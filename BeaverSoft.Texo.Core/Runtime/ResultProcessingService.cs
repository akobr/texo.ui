using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Result;
using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Runtime
{
    public class ResultProcessingService : IResultProcessingService
    {
        private readonly ILogService logger;
        private readonly Dictionary<Type, IItemMappingService> mapping;

        public ResultProcessingService(ILogService logger)
        {
            this.logger = logger;
            mapping = new Dictionary<Type, IItemMappingService>();
        }

        public IImmutableList<IItem> Transfort(ICommandResult result)
        {
            if ((object)result?.Content == null)
            {
                return ImmutableList<IItem>.Empty;
            }

            switch (result)
            {
                case ErrorTextResult errorResult:
                    return ImmutableList<IItem>.Empty.Add(Item.Markdown(errorResult.Content));

                case ModeledResult modeledResult:
                    return ImmutableList<IItem>.Empty.Add(new ModeledItem(modeledResult.Content));

                case MarkdownResult markdownResult:
                    return ImmutableList<IItem>.Empty.Add(Item.Markdown(markdownResult.Content));

                case MarkdownItemsResult markdownItemsResult:
                    return ImmutableList<IItem>.Empty.AddRange(markdownItemsResult.Content.Select(t => new Item(t)));

                case TextStreamResult textStreamResult:
                    return ImmutableList<IItem>.Empty.Add(new StreamedItem(textStreamResult.Content));
            }

            switch ((object)result.Content)
            {
                case IImmutableList<IItem> resultImmutableItems:
                    return resultImmutableItems;

                case IEnumerable<IItem> resultItems:
                    return ImmutableList<IItem>.Empty.AddRange(resultItems);

                case IEnumerable<string> resultTexts:
                    return ImmutableList<IItem>.Empty.AddRange(resultTexts.Select(t => new Item(t)));

                case IItem resultItem:
                    return ImmutableList<IItem>.Empty.Add(resultItem);

                case string resultText:
                    return ImmutableList<IItem>.Empty.Add(new Item(resultText));

                case IReportableStream textStream:
                    return ImmutableList<IItem>.Empty.Add(new StreamedItem(textStream));

                default:
                    return TransformByMap((object)result.Content);
            }
        }


        public void RegisterMappingService<TContent>(IItemMappingService<TContent> service)
        {
            mapping[typeof(TContent)] = service;
        }

        public void UnregisterMappingService<TContent>()
        {
            mapping.Remove(typeof(TContent));
        }

        public IItemMappingService<TContent> GetMappingService<TContent>()
        {
            mapping.TryGetValue(typeof(TContent), out IItemMappingService mappingService);
            return (IItemMappingService<TContent>) mappingService;
        }

        private IImmutableList<IItem> TransformByMap(object content)
        {
            if (content == null)
            {
                return ImmutableList<IItem>.Empty;
            }

            Type contentType = content.GetType();
            MethodInfo serviceMethod = GetType().GetMethod(nameof(GetMappingService));

            if (serviceMethod == null)
            {
                return ImmutableList<IItem>.Empty;
            }

            MethodInfo genericServiceMethod = serviceMethod.MakeGenericMethod(contentType);
            IItemMappingService mappingService = (IItemMappingService) genericServiceMethod.Invoke(this, null);

            if (mappingService == null)
            {
                logger.Warn($"No mapping service for result content of type: {contentType.Name}.", contentType, content);
                return ImmutableList<IItem>.Empty;
            }

            Type genericMappingServiceType = typeof(IItemMappingService<>);
            Type targetMappingServiceType = genericMappingServiceType.MakeGenericType(contentType);
            MethodInfo targetMethod = targetMappingServiceType.GetMethod(nameof(IItemMappingService<string>.Map));

            if (targetMethod == null)
            {
                return ImmutableList<IItem>.Empty;
            }

            try
            {
                object items = targetMethod.Invoke(mappingService, new[] {content});
                return (IImmutableList<IItem>) items;
            }
            catch (Exception e)
            {
                logger.Error("Error during mapping from result content to items.", e);
                return ImmutableList<IItem>.Empty;
            }
        }
    }
}