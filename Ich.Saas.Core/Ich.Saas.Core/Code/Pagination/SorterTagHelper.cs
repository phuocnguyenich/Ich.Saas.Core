using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.Pagination
{
    // Taghelper for sorting. Used in table headers. 

    [HtmlTargetElement("a", Attributes = SortAttributeName)]
    public class SorterTagHelper : TagHelper
    {
        private const string SortAttributeName = "sort";

        [HtmlAttributeName(SortAttributeName)]
        public string Sort { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private readonly IStringLocalizer<SharedResources> _localizer;

        public SorterTagHelper(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var model = ((dynamic)(ViewContext.View)).RazorPage.Model as PagedModel;

            if (model == null)
            {
                throw new InvalidCastException("The <a sort=''></a> TagHelper requires that @model derives from PagedModel");
            }

            var tagBuilder = new TagBuilder("a");

            if (model.UnsignedSort.Equals(Sort, StringComparison.CurrentCultureIgnoreCase))
                tagBuilder.AddCssClass("selected-" + (model.Sort.StartsWith("-") ? "desc" : "asc"));
            else
                tagBuilder.AddCssClass("selected-none");  // prevents the column width from changing

            var sort = Reverse(Sort, model.Sort);
            tagBuilder.MergeAttribute("data-sort", sort);
            tagBuilder.MergeAttribute("href", "javascript:void(0);");

            // translate content

            var childContent = await output.GetChildContentAsync();

            output.Attributes.Clear();
            output.Content.Clear();

            using (var writer = new StringWriter())
            {
                childContent.WriteTo(writer, HtmlEncoder.Default);
                var content = writer.ToString();

                var translated = _localizer[content];
                output.Content.Append(translated); 
            }

            output.MergeAttributes(tagBuilder);
        }

        private string Reverse(string sort, string originalSort)
        {
            return originalSort.StartsWith("-") ? sort : "-" + sort;
        }
    }
}