namespace EZAccess.Blazor.Forms;

public class EZValidation : ComponentBase
{
    private ValidationMessageStore? messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    /// <summary>
    /// bla
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    protected override void OnInitialized()
    {
        if (CurrentEditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(EZValidation)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(EZValidation)} " +
                $"inside an {nameof(EditForm)}.");
        }

        messageStore = new(CurrentEditContext);

        CurrentEditContext.OnValidationRequested += (s, e) =>
            messageStore?.Clear();
        CurrentEditContext.OnFieldChanged += (s, e) =>
            messageStore?.Clear(e.FieldIdentifier);
    }

    public void DisplayErrors(Dictionary<string, List<string>> errors)
    {
        if (CurrentEditContext is not null)
        {
            foreach (var err in errors)
            {
                messageStore?.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    public void ClearErrors()
    {
        messageStore?.Clear();
        CurrentEditContext?.NotifyValidationStateChanged();
    }

    public void ClearErrors(in FieldIdentifier fieldIdentifier)
    {
        messageStore?.Clear(fieldIdentifier);
        CurrentEditContext?.NotifyValidationStateChanged();
    }

}
