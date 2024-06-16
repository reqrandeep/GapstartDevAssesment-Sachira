using Microsoft.AspNetCore.Components;

namespace Chinook.Shared.Components
{
    public class ModalPageModel : ComponentBase
    {
        [Parameter]
        public RenderFragment? Title { get; set; }
        [Parameter]
        public RenderFragment? Body { get; set; }
        [Parameter]
        public RenderFragment? Footer { get; set; }
        public string modalDisplay = "none;";
        public string modalClass = "";
        public bool showBackdrop = false;

        public Guid Guid = Guid.NewGuid();

        public void Open()
        {
            modalDisplay = "block;";
            modalClass = "show";
            showBackdrop = true;
            StateHasChanged();
        }

        public void Close()
        {
            modalDisplay = "none";
            modalClass = "";
            showBackdrop = false;
        }
    }
}
