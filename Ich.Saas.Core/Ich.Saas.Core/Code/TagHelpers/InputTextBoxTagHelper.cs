using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Ich.Saas.Core.Code.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

namespace Ich.Saas.Core.Code.TagHelpers
{
    [HtmlTargetElement("inputTextBox", Attributes = ValueAttributeName)]
    public class InputTextBoxTagHelper : TagHelper
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

        protected IHtmlGenerator Generator { get; }
        private readonly IStringLocalizer<SharedResources> _localizer;

        public InputTextBoxTagHelper(IHtmlGenerator generator, IStringLocalizer<SharedResources> localizer)
        {
            Generator = generator;
            _localizer = localizer;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var encoder = HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs });

            string input = "";

            var helper = new InputTagHelper(Generator);
            helper.ViewContext = ViewContext;
            helper.For = Value;
            
            helper.Init(context);
            helper.Process(context, output);

            using (var writer = new StringWriter())
            {
                output.TagName = "input";
                output.Attributes.Add("class", "form-control");

                output.WriteTo(writer, HtmlEncoder.Default);
                input = writer.ToString();
            }

            output.Attributes.Clear();
            output.PostContent.Clear();
            output.Content.Clear();

            var label = encoder.Encode(_localizer[Label ?? Value?.Metadata?.DisplayName ?? Value?.Name ?? ""]);
            var name = encoder.Encode(Value.Name ?? "");
            var labelContent = $"<label for='{name}' class='col-sm-3 col-form-label'>{label}</label>";

            var validateBuilder = Generator.GenerateValidationMessage(
                    ViewContext,
                    Value.ModelExplorer,
                    Value.Name,
                    message: null,
                    tag: null,
                    htmlAttributes: null);

            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("col-sm-9");
            divBuilder.InnerHtml.AppendHtml(input);
            divBuilder.InnerHtml.AppendHtml(validateBuilder);

            output.TagName = "div";
            output.Attributes.Add("class", "form-group row");

            output.Content.AppendHtml(labelContent);
            output.Content.AppendHtml(divBuilder);
        }
    }
}