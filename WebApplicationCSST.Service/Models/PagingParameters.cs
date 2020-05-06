using System;

namespace WebApplicationCSST.Service.Models
{
	public class PagingParameters
	{
		private const int MAX_PAGE_SIZE = 25;
		private int _pageSize = 20;

		public int PageNumber { get; set; } = 1;

		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = Math.Min(value, MAX_PAGE_SIZE);
			}
		}
	}
}
