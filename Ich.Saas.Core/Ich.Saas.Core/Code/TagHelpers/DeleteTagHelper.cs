using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ich.Saas.Core.Code.TagHelpers
{
    // Delete link button

    [HtmlTargetElement("a", Attributes = CountAttributeName)]
    [HtmlTargetElement("a", Attributes = MessageAttributeName)]
    public class DeleteTagHelper : TagHelper
    {
        private const string CountAttributeName = "related-count";
        private const string MessageAttributeName = "related-message";

        [HtmlAttributeName(CountAttributeName)]
        public string Count { get; set; }

        [HtmlAttributeName(MessageAttributeName)]
        public string Message { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("related-count");
            output.Attributes.RemoveAll("related-message");

            if (string.IsNullOrEmpty(Count) || Count == "0")  // Can be deleted
            {
                
            }
            else // cannot be deleted
            {
                output.Attributes.SetAttribute("href", "javascript:void(0);");
                output.Attributes.Add("data-toggle", "popover");
                output.Attributes.Add("data-content", Message);
            }
        }
    }
}