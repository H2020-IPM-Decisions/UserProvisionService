namespace H2020.IPMDecisions.UPR.Core.Helpers
{
    public interface IPagedList
    {
        /// <summary>
		/// One-based index of this subset within the superset.
		/// </summary>
		/// <value>
		/// One-based index of this subset within the superset.
		/// </value>
        int CurrentPage { get; }
        /// <summary>
		/// Total number of subsets within the superset.
		/// </summary>
		/// <value>
		/// Total number of subsets within the superset.
		/// </value>
        int TotalPages { get; }
        /// <summary>
		/// Maximum size any individual subset.
		/// </summary>
		/// <value>
		/// Maximum size any individual subset.
		/// </value>
        int PageSize { get; }
        /// <summary>
		/// Total number of objects contained within the superset.
		/// </summary>
		/// <value>
		/// Total number of objects contained within the superset.
		/// </value>
        int TotalCount { get; }
        /// <summary>
		/// Returns true if this is NOT the first subset within the superset.
		/// </summary>
		/// <value>
		/// Returns true if this is NOT the first subset within the superset.
		/// </value>
        bool HasPrevious { get; }
        /// <summary>
		/// Returns true if this is NOT the last subset within the superset.
		/// </summary>
		/// <value>
		/// Returns true if this is NOT the last subset within the superset.
		/// </value>
        bool HasNext { get; }
        /// <summary>
		/// Returns true if this is the first subset within the superset.
		/// </summary>
		/// <value>
		/// Returns true if this is the first subset within the superset.
		/// </value>
        bool IsFirstPage { get; }
        /// <summary>
		/// Returns true if this is the last subset within the superset.
		/// </summary>
		/// <value>
		/// Returns true if this is the last subset within the superset.
		/// </value>
        bool IsLastPage { get; }
    }
}