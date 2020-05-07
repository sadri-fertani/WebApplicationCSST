using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebApplicationCSST.Service;

namespace WebApplicationCSST.API.Filters
{
    public class AllowPagingHeaderAttribute : ResultFilterAttribute
    {
        public AllowPagingHeaderAttribute()
        {
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if ((((ObjectResult)context.Result).StatusCode != StatusCodes.Status500InternalServerError) &&
                (((ObjectResult)context.Result).StatusCode != StatusCodes.Status404NotFound))
            {
                var objectResult = ((ObjectResult)context.Result).Value;

                var typeDefinition = objectResult.GetType().GetGenericTypeDefinition();

                if (typeDefinition.ContainsGenericParameters && typeDefinition.Name.StartsWith(typeof(PagedList<>).Name))
                {
                    var pagingInfos = objectResult as dynamic;

                    if (pagingInfos.TotalPages is int && pagingInfos.TotalPages != 1)
                    {
                        context.HttpContext.Response.Headers.Add(
                            "X-Pagination",
                            JsonConvert.SerializeObject(
                                    new
                                    {
                                        pagingInfos.TotalCount,
                                        pagingInfos.PageSize,
                                        pagingInfos.CurrentPage,
                                        pagingInfos.TotalPages,
                                        pagingInfos.HasNext,
                                        pagingInfos.HasPrevious
                                    })
                                );
                    }
                }
            }

            base.OnResultExecuting(context);
        }        
    }
}
