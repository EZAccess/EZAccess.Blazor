namespace EZAccess.Blazor.Layout.AdminLte;

public class FieldCssProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        if (editContext.IsModified(fieldIdentifier))
        {
            return isValid ? "form-control form-control-sm is-valid" : "form-control form-control-sm is-invalid";
        }
        else
        {
            return isValid ? "form-control form-control-sm" : "form-control form-control-sm is-invalid";
        }
    }
}
