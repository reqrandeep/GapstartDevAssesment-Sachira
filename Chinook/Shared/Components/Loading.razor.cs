using Microsoft.AspNetCore.Components;

namespace Chinook.Shared.Components
{
    public class LoadingModel : ComponentBase
    {
        [Parameter] 
        public string Message { get; set; }
    }
}
