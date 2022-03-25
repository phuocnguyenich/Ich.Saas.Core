using System;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
// ReSharper disable IdentifierTypo

namespace Ich.Saas.Core.Code.Pagination
{
    // Taghelper that renders pagination control

    [HtmlTargetElement("a", Attributes = PageAttributeName)]
    public class PagerTagHelper : TagHelper
    {
        private const string PageAttributeName = "page";

        [HtmlAttributeName(PageAttributeName)]
        public ModelExpression Page { get; set; }

        private readonly IStringLocalizer<SharedResources> _localizer;

        public PagerTagHelper(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var model = ((dynamic)(ViewContext.View)).RazorPage.Model as PagedModel;

            if (model == null)
            {
                throw new InvalidCastException("<a page=''></a> TagHelper requires that the @model derives from PagedModel");
            }

            int page = int.Parse(Page.Model.ToString());

            var tagBuilder = new TagBuilder("a");
            tagBuilder.AddCssClass("btn btn-sm btn-light round-corners");
            tagBuilder.MergeAttribute("href", "javascript:void(0);");

            if (page == -1)
            {
                tagBuilder.AddCssClass("disabled");
            }
            else
            {
                tagBuilder.MergeAttribute("data-page", page.ToString());
            }

            output.MergeAttributes(tagBuilder);
        }
    }
}