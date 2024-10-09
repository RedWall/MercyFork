using MercyFork.Data.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace MercyFork.ApiService
{
    public class RepoSearchCriteriaBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var query = bindingContext.HttpContext.Request.QueryString.Value;

            if (string.IsNullOrWhiteSpace(query))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (RepoSearchCriteria.FromQueryString(query, out var searchCriteria))
            {
                bindingContext.Result = ModelBindingResult.Success(searchCriteria);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }

    public class RepoSearchCriteriaBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Metadata.ModelType == typeof(RepoSearchCriteria))
            {
                return new BinderTypeModelBinder(typeof(RepoSearchCriteriaBinder));
            }

            return null;
        }
    }
}
