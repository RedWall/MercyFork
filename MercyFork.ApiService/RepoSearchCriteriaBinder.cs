using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MercyFork.Data.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace MercyFork.ApiService
{
    public class RepoSearchCriteriaBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = bindingContext.HttpContext.Request.QueryString.Value;

            if (query is null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (RepoSearchCriteria.TryParse(query, out var searchCriteria))
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
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(RepoSearchCriteria))
            {
                return new BinderTypeModelBinder(typeof(RepoSearchCriteriaBinder));
            }
            return null;
        }
    }

    public class RepoSearchRangeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = bindingContext.HttpContext.Request.QueryString.Value;

            if (query is null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (RepoSearchRange.TryParse(query, out var searchCriteria))
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

    public class RepoSearchRangeBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(RepoSearchRange))
            {
                return new BinderTypeModelBinder(typeof(RepoSearchRangeBinder));
            }
            return null;
        }
    }
}
