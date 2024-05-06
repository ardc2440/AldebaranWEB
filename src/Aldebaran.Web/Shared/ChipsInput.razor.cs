using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Aldebaran.Web.Shared
{
    public partial class ChipsInput
    {
        /// <summary>
        /// The list of the "applied" chips
        /// </summary>
        [Parameter]
        public List<string> Chips { get; set; } = new List<string>();

        /// <summary>
        /// The callback when the chips list has changed
        /// </summary>
        [Parameter]
        public EventCallback<List<string>> OnChipsChanged { get; set; }

        /// <summary>
        /// Indicates whether or not chips are readonly
        /// </summary>
        [Parameter]
        public bool ReadonlyChips { get; set; } = false;

        /// <summary>
        /// A list of allowed chip/tag values
        /// </summary>
        [Parameter]
        public List<string>? AllowedValues { get; set; } = null;

        /// <summary>
        /// The validation message to use when the chip/tag value is not present in the AllowedValues list
        /// </summary>
        [Parameter]
        public string AllowedValueValidationMessage { get; set; } = "El valor no está presente en la lista de valores permitidos";

        /// <summary>
        /// The css class placed on every chip
        /// </summary>
        [Parameter]
        public string ChipClass { get; set; } = "";

        /// <summary>
        /// The css class placed on the list tag of the chips
        /// </summary>
        [Parameter]
        public string ChipsListClass { get; set; } = "";

        /// <summary>
        /// The css class placed on the container of the chips input
        /// </summary>
        [Parameter]
        public string ChipsContainerClass { get; set; } = "";

        /// <summary>
        /// Indicates whether or not backspace will remove the last chip
        /// </summary>
        [Parameter]
        public bool EnableBackspaceRemove { get; set; } = true;

        /// <summary>
        /// Indicates whether or not readonly items can be deleted
        /// </summary>
        [Parameter]
        public bool AllowDeleteOfReadonlyItems { get; set; } = false;

        /// <summary>
        /// Indicates whether or not to show validation errors
        /// </summary>
        [Parameter]
        public bool ShowValidationErrors { get; set; } = false;

        /// <summary>
        /// The maximum number of chips
        /// </summary>
        [Parameter]
        public int? MaxValueCount { get; set; } = null;

        /// <summary>
        /// The validation message to use when the MaxValueCount rule is not respected
        /// </summary>
        [Parameter]
        public string MaxValueCountValidationMessage { get; set; } = $"Se ha alcanzado la cantidad máxima de valores de ficha";

        /// <summary>
        /// Callback to perform custom validation
        /// </summary>
        [Parameter]
        public EventCallback<ChipValidationArgs> CustomValidation { get; set; }

        /// <summary>
        /// Custom template for prepending an icon to the chip
        /// </summary>
        [Parameter]
        public RenderFragment? PrependIconTemplate { get; set; }

        /// <summary>
        /// Custom template for showing validation errors, make sure to set 'ShowValidationErrors' to true in order for validation errors to render
        /// </summary>
        [Parameter]
        public RenderFragment<string>? ValidationErrorTemplate { get; set; }

        [Parameter]
        public string AddChipKeyCode { get; set; } = "Enter";

        /// <summary>
        /// Custom attributes for the text input
        /// </summary>
        [Parameter]
        public Dictionary<string, object> InputAttributes { get; set; } = new Dictionary<string, object>();

        private string currentInputValue = "";
        private string prevInputValue = "";
        private readonly List<string> validationErrors = new List<string>();

        protected override void OnInitialized()
        {
            if (ReadonlyChips && !InputAttributes.ContainsKey("readonly")) InputAttributes.Add("readonly", "");
        }

        private void OnInput(ChangeEventArgs args)
        {
            prevInputValue = currentInputValue;
            currentInputValue = args.Value.ToString().Trim();
        }

        private void OnKeyDown(KeyboardEventArgs args)
        {
            if (currentInputValue.Length == 0 && args.Key == "Backspace")
            {
                prevInputValue = currentInputValue;
            }
        }

        private void OnKeyUp(KeyboardEventArgs args)
        {
            validationErrors.Clear();

            if (EnableBackspaceRemove && args.Key == "Backspace" && Chips.Count > 0 && prevInputValue.Length == 0 && !ReadonlyChips) RemoveChip(Chips.Last());
            if (args.Code != AddChipKeyCode) return;
            if (CustomValidation.HasDelegate) CustomValidation.InvokeAsync(new ChipValidationArgs(Chips, currentInputValue, validationErrors));
            if (MaxValueCount != null && Chips.Count == MaxValueCount) validationErrors.Add(MaxValueCountValidationMessage);
            if (AllowedValues != null && AllowedValues.Count > 0 && !AllowedValues.Contains(currentInputValue, StringComparer.OrdinalIgnoreCase)) validationErrors.Add(AllowedValueValidationMessage);
            if (validationErrors.Count > 0) return;

            Chips.Add(currentInputValue);
            currentInputValue = "";
            OnChipsChanged.InvokeAsync(Chips);
        }

        private void RemoveChip(string chip)
        {
            Chips.Remove(chip);
            OnChipsChanged.InvokeAsync(Chips);
        }
    }
}
