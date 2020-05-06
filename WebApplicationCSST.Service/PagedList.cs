using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationCSST.Service
{
	public class PagedList<T> : List<T>
	{
		#region ...
		public int CurrentPage { get; private set; }
		public int TotalPages { get; private set; }
		public int PageSize { get; private set; }
		public int TotalCount { get; private set; }

		public bool HasPrevious => CurrentPage > 1;
		public bool HasNext => CurrentPage < TotalPages;
		#endregion
		public PagedList(List<T> items, int count, int pageNumber, int pageSize)
		{
			TotalCount = count;
			PageSize = pageSize;
			CurrentPage = pageNumber;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);

			AddRange(items);
		}

		public static async Task<PagedList<T>> ToPagedList<U>(IMapper _mapper, IQueryable<U> source, int pageNumber, int pageSize)
		{
			var count = source.Count();
			var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync<U>();

			return new PagedList<T>(_mapper.Map<List<T>>(items), count, pageNumber, pageSize);
		}
	}
}
