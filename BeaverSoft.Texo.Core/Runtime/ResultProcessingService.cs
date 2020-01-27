using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Model.Text;
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

        public async Task<IImmutableList<IItem>> TransfortAsync(ICommandResult result, CancellationToken cancellation = default)
        {
            await result?.ExecuteResultAsync();

            if (result?.Content is null)
            {
                return ImmutableList<IItem>.Empty;
            }

            return await FromContentTypeAsync((object)result.Content, cancellation);
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

        private async Task<IImmutableList<IItem>> FromContentTypeAsync(object content, CancellationToken cancellation)
        {
            switch (content)
            {
                case IImmutableList<IItem> resultImmutableItems:
                    return resultImmutableItems;

                case IItem resultItem:
                    return ImmutableList<IItem>.Empty.Add(resultItem);

                case string resultText:
                    return ImmutableList<IItem>.Empty.Add(new Item(resultText));

                case IReportableStream textStream:
                    return ImmutableList<IItem>.Empty.Add(new StreamedItem(textStream));

                // The order of IElement and IEnumerable<IElement> is important
                case IElement resultModel:
                    return ImmutableList<IItem>.Empty.Add(new ModeledItem(resultModel));

                //case IAsyncEnumerable<string> asyncTexts:
                //    return asyncTexts.Select(t => new Item(t));

                case IEnumerable<IItem> resultItems:
                    return ImmutableList<IItem>.Empty.AddRange(resultItems);

                case IEnumerable<string> resultTexts:
                    return ImmutableList<IItem>.Empty.AddRange(resultTexts.Select(t => new Item(t)));

                case IEnumerable<IElement> resultModels:
                    return ImmutableList<IItem>.Empty.AddRange(resultModels.Select(m => new ModeledItem(m)));

                case ICommandResult genericResult:
                    return await TransfortAsync(genericResult, cancellation);

                default:
                    return await Task.Run(() => TransformByMap(content), cancellation);
            }
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