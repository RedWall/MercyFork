using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MercyFork.Data.Models
{
    public record RepoSearchCriteria
    {
        public string? SearchText { get; set; }
        public bool? Archived { get; set; }
        public RepoSearchRange? Stars { get; set; }
        public RepoSearchRange? Forks { get; set; }
        public RepoSearchRange? Followers { get; set; }
        public string? SortField { get; set; } = "Stars";
        public string? SortDirection { get; set; } = "Descending";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public static bool TryParse(string query, out RepoSearchCriteria? repoSearchCriteria)
            => FromQueryString(query, out repoSearchCriteria);

        public static bool FromQueryString([NotNullWhen(true)] string? queryString, [MaybeNullWhen(false)] out RepoSearchCriteria result) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(queryString))
                {
                    throw new ArgumentException("Invalid query string", nameof(queryString));
                }

                var keyValuePairs = queryString.Split('&');

                // Create a dictionary to store the parsed values
                var parsedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Parse each key-value pair and add it to the dictionary
                foreach (var keyValuePair in keyValuePairs)
                {
                    var parts = keyValuePair.Split('=');
                    var key = parts[0];
                    var value = parts[1];
                    parsedValues[key] = value;
                }

                var searchCriteria = new RepoSearchCriteria();

                if (parsedValues.TryGetValue("SearchText", out var searchText))
                {
                    searchCriteria.SearchText = searchText;
                }

                if (parsedValues.TryGetValue("Archived", out var archivedValue) && bool.TryParse(archivedValue, out var archived))
                {
                    searchCriteria.Archived = archived;
                }

                if (parsedValues.TryGetValue("SortField", out var sortField))
                {
                    searchCriteria.SortField = sortField;
                }

                if (parsedValues.TryGetValue("SortDirection", out var sortDirection))
                {
                    searchCriteria.SortDirection = sortDirection;
                }

                if (parsedValues.TryGetValue("Page", out var pageValue) && int.TryParse(pageValue, out var page))
                {
                    searchCriteria.Page = page;
                }

                if (parsedValues.TryGetValue("PageSize", out var pageSizeValue) && int.TryParse(pageSizeValue, out var pageSize))
                {
                    searchCriteria.PageSize = pageSize;
                }

                if (parsedValues.TryGetValue("Stars", out var starsValue) && RepoSearchRange.FromQueryString(starsValue, out var starsRange))
                {
                    searchCriteria.Stars = starsRange with { Field = "Stars" };
                }

                if (parsedValues.TryGetValue("Forks", out var forksValue) && RepoSearchRange.FromQueryString(forksValue, out var forksRange))
                {
                    searchCriteria.Forks = forksRange with { Field = "Forks" };
                }

                if (parsedValues.TryGetValue("Followers", out var followersValue) && RepoSearchRange.FromQueryString(followersValue, out var followersRange))
                {
                    searchCriteria.Followers = followersRange with { Field = "Followers" };
                }

                result = searchCriteria;

                return true;
            }
            catch
            {
                result = null;

                return false;
            }
        }
    }

    /// <summary>
    /// Helper class in generating the range values for a qualifier e.g. In or Size qualifiers
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public record RepoSearchRange
    {
        public enum SearchQualifierOperator
        {
            /// <summary>
            /// Exactly "size"
            /// </summary>
            [Display(Name = "=")]
            Exactly,

            /// <summary>
            /// Between min and max "min..max"
            /// </summary>
            [Display(Name = "x..y")]
            Between,

            /// <summary>
            /// Greater than "&gt;"
            /// </summary>
            [Display(Name = ">")]
            GreaterThan,

            /// <summary>
            /// Less than "&lt;"
            /// </summary>
            [Display(Name = "<")]
            LessThan,

            /// <summary>
            /// Less than or equal to. "&lt;="
            /// </summary>
            [Display(Name = "<=")]
            LessThanOrEqualTo,

            /// <summary>
            /// Greater than or equal to. "&gt;="
            /// </summary>
            [Display(Name = ">=")]
            GreaterThanOrEqualTo
        }

        public string? Field { get; set; }
        public SearchQualifierOperator SearchQualifier { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Size { get; set; }

        internal string DebuggerDisplay
        {
            get { return string.Format("Query: {0}", ToString()); }
        }

        public override string ToString()
        {
            return SearchQualifier switch
            {
                SearchQualifierOperator.Exactly => $"{Field}:{Size}",
                SearchQualifierOperator.Between => $"{Field}:{Min}..{Max}",
                SearchQualifierOperator.GreaterThan => $"{Field}:>{Size}",
                SearchQualifierOperator.LessThan => $"{Field}:<{Size}",
                SearchQualifierOperator.LessThanOrEqualTo => $"{Field}:<={Size}",
                SearchQualifierOperator.GreaterThanOrEqualTo => $"{Field}:>={Size}",
                _ => ""
            };
        }

        public string ToQueryString()
        {
            return $"{Field}=" + ToQueryStringValue();
        }

        public string ToQueryStringValue()
        {
            return SearchQualifier switch
            {
                SearchQualifierOperator.Exactly => $"e{Size}",
                SearchQualifierOperator.Between => $"{Min}..{Max}",
                SearchQualifierOperator.GreaterThan => $"g{Size}",
                SearchQualifierOperator.LessThan => $"l{Size}",
                SearchQualifierOperator.LessThanOrEqualTo => $"le{Size}",
                SearchQualifierOperator.GreaterThanOrEqualTo => $"ge{Size}",
                _ => ""
            };
        }

        public static bool TryParse(string query, out RepoSearchRange? repoSearchRange)
            => FromQueryString(query, out repoSearchRange);

        public static bool FromQueryString([NotNullWhen(true)] string? queryString, [MaybeNullWhen(false)] out RepoSearchRange result) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(queryString))
                {
                    throw new ArgumentException("Invalid query string", nameof(queryString));
                }

                string? field = null;
                string? rangeData;

                var parts = queryString.Split('=');

                if (parts.Length == 2)
                {
                    field = parts[0];
                    rangeData = parts[1];
                }
                else
                {
                    rangeData = queryString;
                }

                if (rangeData.Contains(".."))
                {
                    var range = rangeData.Split("..");
                    var min = int.Parse(range[0]);
                    var max = int.Parse(range[1]);

                    result = new RepoSearchRange
                    {
                        Field = field,
                        SearchQualifier = SearchQualifierOperator.Between,
                        Min = min,
                        Max = max
                    };
                }
                else
                {
                    var operatorIndex = rangeData.LastIndexOfAny(['g', 'l', 'e']);

                    var op = rangeData[..(operatorIndex + 1)];
                    var size = int.Parse(rangeData[(operatorIndex + 1)..]);

                    result = new RepoSearchRange
                    {
                        Field = field,
                        SearchQualifier = op switch
                        {
                            "g" => SearchQualifierOperator.GreaterThan,
                            "l" => SearchQualifierOperator.LessThan,
                            "e" => SearchQualifierOperator.Exactly,
                            "le" => SearchQualifierOperator.LessThanOrEqualTo,
                            "ge" => SearchQualifierOperator.GreaterThanOrEqualTo,
                            _ => throw new System.Exception($"Invalid operator: {op}")
                        },
                        Size = size
                    };
                }

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
