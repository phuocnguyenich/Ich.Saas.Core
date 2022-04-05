using System.Text.Encodings.Web;
using System.Text.Unicode;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.TagHelpers
{
    [HtmlTargetElement("outputField", Attributes = ValueAttributeName)]
    public class OutputFieldTagHelper : TagHelper
    {
        private const string LabelAttributeName = "label";
        private const string ValueAttributeName = "value";
 
        [HtmlAttributeName(LabelAttributeName)]
        public string Label { get; set; }

        [HtmlAttributeName(ValueAttributeName)]
        public ModelExpression Value { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private readonly IStringLocalizer<SharedResources> _localizer;

        public OutputFieldTagHelper(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var encoder = HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs });

            var value = encoder.Encode(Value?.Model?.ToString() ?? "");
            var label = encoder.Encode(_localizer[Label ?? Value?.Metadata?.DisplayName ?? Value?.Name ?? ""]);

            var labelContent = $"<label class='col-sm-3 col-form-label'>{label}</label>";
            var valueContent = $"<div class='col-sm-9 col-form-text'>{value}</div>";

            output.TagName = "div";
            output.Attributes.Add("class", "form-group row");

            output.Content.AppendHtml(labelContent);
            output.Content.AppendHtml(valueContent);
        }
    }
}